using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace wfwinhello
{
    class JsonRequest
    {
        private static Uri serverBaseUri = new Uri("http://localhost:59992/");

        private const int E_WINHTTP_TIMEOUT = unchecked((int)0x80072ee2);
        private const int E_WINHTTP_NAME_NOT_RESOLVED = unchecked((int)0x80072ee7);
        private const int E_WINHTTP_CANNOT_CONNECT = unchecked((int)0x80072efd);
        private const int E_WINHTTP_CONNECTION_ERROR = unchecked((int)0x80072efe);

        private JsonObject message = new JsonObject();

        public static JsonRequest Create()
        {
            return new JsonRequest();
        }

        public JsonRequest AddString(string key, string value)
        {
            message.Add(key, JsonValue.CreateStringValue(value));
            return this;
        }

        public async Task<JsonValue> PostAsync(string relativeUri)
        {
            HttpStringContent content = new HttpStringContent(message.Stringify(), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await httpClient.PostAsync(new Uri(serverBaseUri, relativeUri), content);
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case E_WINHTTP_TIMEOUT:
                    // The connection to the server timed out.
                    case E_WINHTTP_NAME_NOT_RESOLVED:
                    case E_WINHTTP_CANNOT_CONNECT:
                    case E_WINHTTP_CONNECTION_ERROR:
                    // Unable to connect to the server. Check that you have Internet access.
                    default:
                        // "Unexpected error connecting to server: ex.Message
                        return null;
                }
            }

            // We assume that if the server responds at all, it responds with valid JSON.
            return JsonValue.Parse(await httpResponse.Content.ReadAsStringAsync());
        }
    }
}
