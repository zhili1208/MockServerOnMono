using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;

namespace MockServer
{
	public static class CredentialRetryIssue
	{
		// repro for github.com/NuGet/Home/issues/3762
		public static void ReproIssue()
		{
			using (var server = new MockServer())
			{
				server.Listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
				server.Put.Add("/nuget", r => new Action<HttpListenerResponse>(res =>
					 {
						 var h = r.Headers["Authorization"];
						 var credential = Encoding.Default.GetString(Convert.FromBase64String(h.Substring(6)));

					if (credential.Equals("testuser:testpassword", StringComparison.OrdinalIgnoreCase))
						 {
							 byte[] buffer = MockServer.GetPushedPackage(r);
							 using (var of = new FileStream("t1.nupkg", FileMode.Create))
							 {
								 of.Write(buffer, 0, buffer.Length);
							 }
							 res.StatusCode = (int)HttpStatusCode.Created;
						 }
						 else
						 {
							 res.AddHeader("WWW-Authenticate", "Basic ");
							 res.StatusCode = (int)HttpStatusCode.Unauthorized;
						 }
					 }));

				server.Start();

				var handler = new HttpClientHandler();
				var testHandler = new TestCredentialsHandler(handler);
				var client = new HttpClient(testHandler);
				var serverUrl = HttpRequest.ServerBaseUrl + "nuget";
				var response = client.SendAsync(HttpRequest.GetPutRequest(serverUrl)).Result;
			}
		}
	}
}

