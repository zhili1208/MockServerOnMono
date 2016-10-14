using System;
using System.Net;
using System.IO;
using System.Net.Http;


namespace MockServer
{
	class MainClass
	{
		public static void Main(string[] args)
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

				//var client = new HttpClient();
				//var result = client.GetAsync(@"http://localhost:50231/redirect").Result;
			}
		}
	}
}
