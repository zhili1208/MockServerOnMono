using System;
using System.Net;

namespace MockServer
{
	public class TestCredentials : ICredentials
	{
		private NetworkCredential _credentials;

		public NetworkCredential Credentials
		{
			get
			{
				return _credentials;
			}

			set
			{
				_credentials = value;
			}
		}
		NetworkCredential ICredentials.GetCredential(Uri uri, string authType)
		{
			return _credentials;
		}
	}
}

