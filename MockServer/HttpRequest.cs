using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

namespace MockServer
{
	public static class HttpRequest
	{
		private static string _testFile = "../../" +
			"zhi-test-pack.2.0.0.nupkg";

		public static string ServerBaseUrl = "http://localhost:50231/";


		public static HttpRequestMessage GetPutRequest(string serverUrl)
		{
			var fileStream = new FileStream(_testFile, FileMode.Open, FileAccess.Read, FileShare.Read);

			var request = new HttpRequestMessage(HttpMethod.Put, serverUrl);
			var content = new MultipartFormDataContent();

			var packageContent = new StreamContent(fileStream);
			packageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
			//"package" and "package.nupkg" are random names for content deserializing
			//not tied to actual package name.
			content.Add(packageContent, "package", "package.nupkg");
			request.Content = content;

			// Send the data in chunks so that it can be canceled if auth fails.
			// Otherwise the whole package needs to be sent to the server before the PUT fails.
			request.Headers.TransferEncodingChunked = true;

			return request;
		}
	}
}

