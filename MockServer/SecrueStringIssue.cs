using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Security;
using System.Linq;

namespace MockServer
{
	public static class SecrueStringIssue
	{
		public static void ReproIssue()
		{
			using (var server = new MockServer())
			{
				server.Listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
				server.Put.Add("/nuget", r => new Action<HttpListenerResponse>(res =>
					 {
						 var h = r.Headers["Authorization"];
						 var credential = Encoding.Default.GetString(Convert.FromBase64String(h.Substring(6)));

						 if (credential.Equals("testuser:password", StringComparison.OrdinalIgnoreCase))
						 {
							 res.StatusCode = (int)HttpStatusCode.Accepted;
						 }
						 else
						 {
							 res.AddHeader("WWW-Authenticate", "Basic ");
							 res.StatusCode = (int)HttpStatusCode.Unauthorized;
						 }
					 }));

				server.Start();

				var handler = new HttpClientHandler();
				var password = new SecureString();

				foreach (var c in "password")
				{
					password.AppendChar(c);
				}

				var credentials = new NetworkCredential
				{
					UserName = "testuser",
					SecurePassword = password
				};
				handler.Credentials = credentials;
				var client = new HttpClient(handler);
				var serverUrl = HttpRequest.ServerBaseUrl + "nuget";
				var response = client.SendAsync(HttpRequest.GetPutRequest(serverUrl)).Result;
			}
		}
	}
}

