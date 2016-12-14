# contentful.net

[https://www.contentful.com][1] is a content management platform for web applications, mobile apps and connected devices. It allows you to create, edit & manage content in the cloud and publish it anywhere via powerful API. Contentful offers tools for managing editorial teams and enabling cooperation between organizations.

This is the .NET SDK for [Contentful's][1] Content Delivery and Content Management APIs.

## Setup

We recommend you use the NuGet package manager to add Contentful to your .Net application using the following command in your NuGet package manager console.

```csharp
Install-Package contentful.csharp -prerelease
```

## Usage

The `ContentfulClient` handles all communication with the Contentful Content Delivery API.

To create a new client you need to pass an `HttpClient`, your delivery API key and any other configuration options:

```csharp
var httpClient = new HttpClient();
var client = new ContentfulClient(httpClient, "<content_delivery_api_key>", "<space_id>");
```

or:

```csharp
var httpClient = new HttpClient();
var options = new ContentfulOptions()
{
    DeliveryApiKey = "<content_delivery_api_key>",
    SpaceId = "<space_id>"
}
var client = new ContentfulClient(httpClient, options);
```

If you are running asp.net core and wish to take advantage of [the options pattern][2] you can do so by passing an `IOptions<ContentfulOptions>` to the constructor. This lets you keep your authorization token in your application settings, in environment variables or your own custom `Microsoft.Extensions.Configuration.IConfigurationSource` provider.

### Querying for content

After creating a `ContentfulClient`, you can now query for a single entry:

```csharp
var entry = await client.GetEntryAsync<Entry<dynamic>>("<entry_id>");

Console.WriteLine(entry.Fields.productName.ToString()); // => Contentful
```

Normally you serialize this response into your own class instead of the generic `Entry<>` type. You can do this by providing a suitable type to seralize into. Take the following class as an example:

```csharp
public class Product {
    public string ProductName { get; set; }
    public string Price { get; set; }
    public string Description { get; set; }
}
```

Pass this class to the `GetEntryAsync<>` method to serialize the response correctly.

```csharp
var product = await client.GetEntryAsync<Product>("<entry_id>");

Console.WriteLine(product.ProductName); // => Your product
Console.WriteLine(product.Price); // => 12.38
Console.WriteLine(product.Description); // => A fantastic product.
```

You can combine the two approaches if you're interested in the system properties of the entry but still want to use your own class.

```csharp
var productEntry = await client.GetEntryAsync<Entry<product>>("<entry_id>");

Console.WriteLine(entry.Fields.Price); // => 12.38
Console.WriteLine(entry.SystemProperties.Id); // => 2CfTFQGwogugS6QcOuwO6q
```

## Management API

To edit, update and delete content you use the `ContentfulManagementClient` class which uses the same familiar pattern as the regular client.

```csharp
var httpClient = new HttpClient();
var managementClient = new ContentfulManagementClient(httpClient, "<content_management_api_key>", "<space_id>");
```

You can then use the client to, for example, create a content type.

```csharp
var contentType = new ContentType();
contentType.SystemProperties = new SystemProperties() {
    Id = "new-content-type"
};
contentType.Name = "New contenttype";
contentType.Fields = new List<Field>()
{
    new Field()
    {
        Name = "Field1",
        Id = "field1",
        Type = "Text"
    },
    new Field()
    {
        Name = "Field2",
        Id = "field2",
        Type = "Integer"
    }
};


await managementClient.CreateOrUpdateContentTypeAsync(contentType);
```

## Further information

You can read the full documentation and explore the api at <https://contentful.github.io/contentful.net-docs/>

[1]: https://www.contentful.com
[2]: https://docs.asp.net/en/latest/fundamentals/configuration.html#options-config-objects
