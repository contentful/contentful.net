![header](https://images.ctfassets.net/gw484zixu8ng/1gSVGV8tICAkeaqkc8C6yG/753ebdf385fc756c7ed48baebd84236d/header-dotnet.png)
<p align="center">
  <a href="https://www.contentful.com/slack/">
    <img src="https://img.shields.io/badge/-Join%20Community%20Slack-2AB27B.svg?logo=slack&maxAge=31557600" alt="Join Contentful Community Slack">
  </a>
  &nbsp;
  <a href="https://www.contentfulcommunity.com/">
    <img src="https://img.shields.io/badge/-Join%20Community%20Forum-3AB2E6.svg?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MiA1OSI+CiAgPHBhdGggZmlsbD0iI0Y4RTQxOCIgZD0iTTE4IDQxYTE2IDE2IDAgMCAxIDAtMjMgNiA2IDAgMCAwLTktOSAyOSAyOSAwIDAgMCAwIDQxIDYgNiAwIDEgMCA5LTkiIG1hc2s9InVybCgjYikiLz4KICA8cGF0aCBmaWxsPSIjNTZBRUQyIiBkPSJNMTggMThhMTYgMTYgMCAwIDEgMjMgMCA2IDYgMCAxIDAgOS05QTI5IDI5IDAgMCAwIDkgOWE2IDYgMCAwIDAgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0UwNTM0RSIgZD0iTTQxIDQxYTE2IDE2IDAgMCAxLTIzIDAgNiA2IDAgMSAwLTkgOSAyOSAyOSAwIDAgMCA0MSAwIDYgNiAwIDAgMC05LTkiLz4KICA8cGF0aCBmaWxsPSIjMUQ3OEE0IiBkPSJNMTggMThhNiA2IDAgMSAxLTktOSA2IDYgMCAwIDEgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0JFNDMzQiIgZD0iTTE4IDUwYTYgNiAwIDEgMS05LTkgNiA2IDAgMCAxIDkgOSIvPgo8L3N2Zz4K&maxAge=31557600"
      alt="Join Contentful Community Forum">
  </a>
</p>

# contentful.net - Contentful .NET SDK

> .NET SDK for the Contentful [Content Delivery API](https://www.contentful.com/developers/docs/references/content-delivery-api/), [Content Preview API](https://www.contentful.com/developers/docs/references/content-preview-api/) and the [Contentful Management API](https://www.contentful.com/developers/docs/references/content-management-api/). It helps you to easily access your Content stored in Contentful with your .NET applications.

<p align="center">
  <img src="https://img.shields.io/badge/Status-Maintained-green.svg" alt="This repository is actively maintained" /> &nbsp;
  <a href="LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-brightgreen.svg" alt="MIT License" />
  </a>
  &nbsp;
  <a href="https://circleci.com/gh/contentful/contentful.net">
    <img src="https://circleci.com/gh/contentful/contentful.net.svg?style=shield" alt="Build Status" />
  </a>
</p>

**What is Contentful?**

[Contentful](https://www.contentful.com/) provides content infrastructure for digital teams to power websites, apps, and devices. Unlike a CMS, Contentful was built to integrate with the modern software stack. It offers a central hub for structured content, powerful management and delivery APIs, and a customizable web app that enable developers and content creators to ship their products faster.

<details>
<summary>Table of contents</summary>

<!-- TOC -->

- [contentful.net - Contentful .NET Delivery SDK](#contentfulnet---contentful-net-sdk)
  - [Core Features](#core-features)
  - [Getting started](#getting-started)
    - [Your first request](#your-first-request)
  - [Using the SDK with the Preview API](#using-the-sdk-with-the-preview-api)
  - [Authentication](#authentication)
  - [Further documentation](#further-documentation)
  - [Reach out to us](#reach-out-to-us)
    - [You have questions about how to use this library?](#you-have-questions-about-how-to-use-this-library)
    - [You found a bug or want to propose a feature?](#you-found-a-bug-or-want-to-propose-a-feature)
    - [You need to share confidential information or have other questions?](#you-need-to-share-confidential-information-or-have-other-questions)
  - [Get involved](#get-involved)
  - [License](#license)
  - [Code of Conduct](#code-of-conduct)

<!-- /TOC -->

</details>

## Core Features

- Content retrieval through the [Content Delivery API](https://www.contentful.com/developers/docs/references/content-delivery-api/) and [Content Preview API](https://www.contentful.com/developers/docs/references/content-preview-api/).
- Content management through the [Content Management API](https://www.contentful.com/developers/docs/references/content-management-api/)
- [Synchronization](https://www.contentful.com/developers/docs/concepts/sync/)
- [Localization support](https://www.contentful.com/developers/docs/concepts/locales/)
- [Link resolution](https://www.contentful.com/developers/docs/concepts/links/)
- Built in rate limiting with recovery procedures

## Getting started

We recommend you use the NuGet Package Manager to add the SDK to your .NET Application using one of the following options:

- In Visual Studio, open Package Manager Console window and run the following command:

  ```powershell
  PM> Install-Package contentful.csharp
  ```

- In a command-line, run the following .NET CLI command:

  ```console
  > dotnet add package contentful.csharp
  ```

## Usage

The `ContentfulClient` handles all communication with the Contentful Content Delivery API.

To create a new client you need to pass an `HttpClient`, your delivery API key and any other configuration options:

```csharp
var httpClient = new HttpClient();
var client = new ContentfulClient(httpClient, "<content_delivery_api_key>", "<content_preview_api_key>", "<space_id>");
```

or:

```csharp
var httpClient = new HttpClient();
var options = new ContentfulOptions
{
    DeliveryApiKey = "<content_delivery_api_key>",
    PreviewApiKey = "<content_preview_api_key>",
    SpaceId = "<space_id>"
}
var client = new ContentfulClient(httpClient, options);
```

If you are running asp.net core and wish to take advantage of [the options pattern][2] you can do so by passing an `IOptions<ContentfulOptions>` to the constructor. This lets you keep your authorization token in your application settings, in environment variables or your own custom `Microsoft.Extensions.Configuration.IConfigurationSource` provider.

### Your first request

After creating a `ContentfulClient`, you can now query for a single entry:

```csharp
var entry = await client.GetEntry<Product>("<entry_id>");

Console.WriteLine(entry.ProductName); // => Contentful
Console.WriteLine(entry.Price); // => 12.38
Console.WriteLine(entry.Description); // => A fantastic product.
```

```csharp
public class Product
{
    public string ProductName { get; set; }
    public string Price { get; set; }
    public string Description { get; set; }
}
```
The properties of your class will be automatically deserialized from fields with matching names.

If you're interested in the system properties of the entry, add a `SystemProperties` property to the class.

```csharp
public class Product
{
    public SystemProperties Sys { get; set; }
    public string ProductName { get; set; }
    public string Price { get; set; }
    public string Description { get; set; }
}
```

```csharp
var productEntry = await client.GetEntry<Product>("<entry_id>");

Console.WriteLine(productEntry.Price); // => 12.38
Console.WriteLine(productEntry.Sys.Id); // => 2CfTFQGwogugS6QcOuwO6q
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
contentType.SystemProperties = new SystemProperties()
{
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


await managementClient.CreateOrUpdateContentType(contentType);
```

## Using the SDK with the Preview API

This SDK can also be used with the Preview API. Make sure you have a preview API key configured and set `UsePreviewAPI` on your client.

```csharp
var httpClient = new HttpClient();
var options = new ContentfulOptions()
{
    DeliveryApiKey = "<content_delivery_api_key>",
    PreviewApiKey, "<content_preview_api_key>"
    SpaceId = "<space_id>",
    UsePreviewApi = true
}
var client = new ContentfulClient(httpClient, options);
```

## Authentication

To get your own content from Contentful, an app should authenticate with an OAuth bearer token.

You can create API keys using the [Contentful web interface](https://app.contentful.com). Go to the app, open the space that you want to access (top left corner lists all the spaces), and navigate to the APIs area. Open the API Keys section and create your first token. Done.

For more information, check the [Contentful REST API reference on Authentication](https://www.contentful.com/developers/docs/references/authentication/).

## Further documentation

You can read the full documentation at https://www.contentful.com/developers/docs/net/ and explore the api at <https://contentful.github.io/contentful.net-docs/>

[1]: https://www.contentful.com
[2]: https://docs.asp.net/en/latest/fundamentals/configuration.html#options-config-objects

## Reach out to us

### You have questions about how to use this library?
* Reach out to our community forum: [![Contentful Community Forum](https://img.shields.io/badge/-Join%20Community%20Forum-3AB2E6.svg?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MiA1OSI+CiAgPHBhdGggZmlsbD0iI0Y4RTQxOCIgZD0iTTE4IDQxYTE2IDE2IDAgMCAxIDAtMjMgNiA2IDAgMCAwLTktOSAyOSAyOSAwIDAgMCAwIDQxIDYgNiAwIDEgMCA5LTkiIG1hc2s9InVybCgjYikiLz4KICA8cGF0aCBmaWxsPSIjNTZBRUQyIiBkPSJNMTggMThhMTYgMTYgMCAwIDEgMjMgMCA2IDYgMCAxIDAgOS05QTI5IDI5IDAgMCAwIDkgOWE2IDYgMCAwIDAgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0UwNTM0RSIgZD0iTTQxIDQxYTE2IDE2IDAgMCAxLTIzIDAgNiA2IDAgMSAwLTkgOSAyOSAyOSAwIDAgMCA0MSAwIDYgNiAwIDAgMC05LTkiLz4KICA8cGF0aCBmaWxsPSIjMUQ3OEE0IiBkPSJNMTggMThhNiA2IDAgMSAxLTktOSA2IDYgMCAwIDEgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0JFNDMzQiIgZD0iTTE4IDUwYTYgNiAwIDEgMS05LTkgNiA2IDAgMCAxIDkgOSIvPgo8L3N2Zz4K&maxAge=31557600)](https://support.contentful.com/)
* Jump into our community slack channel: [![Contentful Community Slack](https://img.shields.io/badge/-Join%20Community%20Slack-2AB27B.svg?logo=slack&maxAge=31557600)](https://www.contentful.com/slack/)

### You found a bug or want to propose a feature?

* File an issue here on GitHub: [![File an issue](https://img.shields.io/badge/-Create%20Issue-6cc644.svg?logo=github&maxAge=31557600)](https://github.com/contentful/contentful.net/issues/new). Make sure to remove any credentials from your code before sharing it.

### You need to share confidential information or have other questions?

* File a support ticket at our Contentful Customer Support: [![File support ticket](https://img.shields.io/badge/-Submit%20Support%20Ticket-3AB2E6.svg?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MiA1OSI+CiAgPHBhdGggZmlsbD0iI0Y4RTQxOCIgZD0iTTE4IDQxYTE2IDE2IDAgMCAxIDAtMjMgNiA2IDAgMCAwLTktOSAyOSAyOSAwIDAgMCAwIDQxIDYgNiAwIDEgMCA5LTkiIG1hc2s9InVybCgjYikiLz4KICA8cGF0aCBmaWxsPSIjNTZBRUQyIiBkPSJNMTggMThhMTYgMTYgMCAwIDEgMjMgMCA2IDYgMCAxIDAgOS05QTI5IDI5IDAgMCAwIDkgOWE2IDYgMCAwIDAgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0UwNTM0RSIgZD0iTTQxIDQxYTE2IDE2IDAgMCAxLTIzIDAgNiA2IDAgMSAwLTkgOSAyOSAyOSAwIDAgMCA0MSAwIDYgNiAwIDAgMC05LTkiLz4KICA8cGF0aCBmaWxsPSIjMUQ3OEE0IiBkPSJNMTggMThhNiA2IDAgMSAxLTktOSA2IDYgMCAwIDEgOSA5Ii8+CiAgPHBhdGggZmlsbD0iI0JFNDMzQiIgZD0iTTE4IDUwYTYgNiAwIDEgMS05LTkgNiA2IDAgMCAxIDkgOSIvPgo8L3N2Zz4K&maxAge=31557600)](https://www.contentful.com/support/)

## Get involved

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?maxAge=31557600)](http://makeapullrequest.com)

We appreciate any help on our repositories. For more details about how to contribute see our [CONTRIBUTING.md](CONTRIBUTING.md) document.

## License

This repository is published under the [MIT](LICENSE) license.

## Code of Conduct

We want to provide a safe, inclusive, welcoming, and harassment-free space and experience for all participants, regardless of gender identity and expression, sexual orientation, disability, physical appearance, socioeconomic status, body size, ethnicity, nationality, level of experience, age, religion (or lack thereof), or other identity markers.

[Read our full Code of Conduct](https://github.com/contentful-developer-relations/community-code-of-conduct).
