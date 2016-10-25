using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MockServer
{
	public class TestCredentialsHandler : DelegatingHandler
	{

		private HttpClientHandler _handler;
		private TestCredentials _crednetial;

		public TestCredentialsHandler(HttpClientHandler handler)
			: base(handler)
		{
			_handler = handler;
			_crednetial = new TestCredentials();
			_handler.Credentials = _crednetial;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var credentialList = new NetworkCredential[3];
			credentialList[0] = new NetworkCredential("a", "b");
			credentialList[1] = new NetworkCredential("c", "d");
			credentialList[2] = new NetworkCredential("testuser", "testpassword");
			HttpResponseMessage response = null;
			int retry = 3;
			while (retry > 0)
			{
				_crednetial.Credentials = credentialList[3 - retry];
				response = await base.SendAsync(request, cancellationToken);
				retry--;
			}

			return response;
		}
	}
}

