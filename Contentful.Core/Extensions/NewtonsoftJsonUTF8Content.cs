using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentful.Core.Extensions;

internal sealed class NewtonsoftJsonUtf8Content : HttpContent
{
    private static readonly MediaTypeHeaderValue Utf8JsonMediaTypeHeaderValue = new("application/json");
    private readonly object _body;
    private readonly JsonSerializer _jsonSerializer;

    public NewtonsoftJsonUtf8Content(in object body, in JsonSerializer jsonSerializer, in MediaTypeHeaderValue mediaTypeHeaderValue = null)
    {
        _body = body;
        _jsonSerializer = jsonSerializer;
        Headers.ContentType = mediaTypeHeaderValue ?? Utf8JsonMediaTypeHeaderValue;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.defaultbuffersize
        const int systemTextJsonJsonSerializerOptionsDefaultBufferSize = 16384;
        // using will flush the writer when disposing
        using StreamWriter streamWriter = new(stream, encoding: Encoding.UTF8, leaveOpen: true,
            bufferSize: systemTextJsonJsonSerializerOptionsDefaultBufferSize // needs it to have leaveOpen...
        );
        _jsonSerializer.Serialize(streamWriter, _body);
        return Task.CompletedTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        // This content doesn't support pre-computed length and
        // the request will NOT contain Content-Length header.
        // It defeats the purpose of streaming the content directly to the response
        length = 0;
        return false;
    }
}