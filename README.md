# contentful.net
.NET SDK for [Contentful's][1] Content Delivery API.

# About

[Contentful][1] is a content management platform for web applications, 
mobile apps and connected devices. It allows you to create, edit & manage 
content in the cloud and publish it anywhere via a powerful API. Contentful offers 
tools for managing editorial teams and enabling cooperation between organizations.

# Setup 

The best and simplest way to add the package to your .Net solution is to use the NuGet package manager.
Run the following command from your NuGet package manager console.

    Install-Package contentful.csharp -prerelease


# Usage

The `ContentfulClient` is responsible for all communication with the Contentful API.

Creating a new client requires you to pass an `HttpClient` and your delivery API key and other options

    var httpClient = new HttpClient();
    var client = new ContentfulClient(httpClient, "authTokenForDeliveryAPI", "SpaceId");

or

    var httpClient = new HttpClient();
    var options = new ContentfulOptions()
    {
        DeliveryApiKey = "authTokenForDeliveryAPI",
        SpaceId = "SpaceId"
    }
    var client = new ContentfulClient(httpClient, options);

If you are running asp.net core and wish to take advantage of [the options pattern][2] you can do so 
by passing in an `IOptions<ContentfulOptions>` to the constructor. This means you can keep your authorization token in your 
application settings, in environment varibles or your own custom `Microsoft.Extensions.Configuration.IConfigurationSource` provider.

## Querying for content

Using a `ContentfulClient` created as in the example above we can now query for a single entry like this.

    var httpClient = new HttpClient();
    var client = new ContentfulClient(httpClient, "0b7f6x59a0", "developer_bookshelf")
    var entry = await client.GetEntryAsync<Entry<dynamic>>("5PeGS2SoZGSa4GuiQsigQu");

    Console.WriteLine(entry.Fields.author.ToString()); // => Contentful

Normally though, we'd like to serialize this response into our own class rather than into the generic `Entry<>` type. We can do that by 
providing a suitable type to seralize into. Imagine the following simple class.

    public class Book {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }

We could pass that to the `GetEntryAsync<>` method and the response would be serialized correctly.

    var book = await client.GetEntryAsync<Book>("5PeGS2SoZGSa4GuiQsigQu");
    
    Console.WriteLine(book.Name); // => How to manage content in a developer-friendly manner
    Console.WriteLine(book.Author); // => Contentful
    Console.WriteLine(book.Description); // => Make an API request, get JSON in return.

We could even combine the two approaches if we are interested in the system properties of the entry but 
still want to use our own class.

    var bookEntry = await client.GetEntryAsync<Entry<Book>>("2CfTFQGwogugS6QcOuwO6q");

    Console.WriteLine(entry.Fields.Author); // => Contentful
    Console.WriteLine(entry.SystemProperties.Id); // => 2CfTFQGwogugS6QcOuwO6q

# Further information

You can read the full documentation and explore the api at [https://contentful.github.io/contentful.net-docs/](https://contentful.github.io/contentful.net-docs/)



[1]: https://www.contentful.com
[2]: https://docs.asp.net/en/latest/fundamentals/configuration.html#options-config-objects