using System;
using System.Net;
using System.Net.Http;


namespace MockServer
{
	public static class RedirectIssue
	{
		// repro for github.com/NuGet/Home/issues/3680
		public static void ReproIssue()
		{
			using (var server = new MockServer())
			{
				server.Get.Add("/redirect", r => "OK");
				server.Put.Add("/redirect", r =>
					new Action<HttpListenerResponse>(
						res =>
						{
							res.Redirect(server.Uri + "nuget");
						}));
				server.Put.Add("/nuget", r => "redirect OK");
				server.Start();

				var client = new HttpClient();
				var serverUrl = HttpRequest.ServerBaseUrl + "redirect";
				var result = client.SendAsync(HttpRequest.GetPutRequest(serverUrl)).Result;
			}
		}	
	}
}

