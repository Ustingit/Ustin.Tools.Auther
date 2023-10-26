using System;

namespace Authorizer
{
	public class AuthInfo
	{
		public AccessValue AccessValue { get; set; }

		public RefreshValue RefreshValue { get; set; }
	}


	public class AccessValue
	{
		public string Value { get; set; }

		public DateTime ExpirationDate { get; set; }
	}

	public class RefreshValue
	{
		public string Value { get; set; }

		public string ClientId { get; set; }

		public string Domain { get; set; }

		public DateTime ExpirationDate { get; set; }
	}
}
