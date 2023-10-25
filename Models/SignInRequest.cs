using Newtonsoft.Json;

namespace Authorizer.Models
{
	public class SignInRequest
	{
		[JsonProperty("clientId")]
		public string ClientId { get; set; }

		[JsonProperty("companyId")]
		public string CompanyId { get; set; }

		[JsonProperty("domain")]
		public string Domain { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }
	}
}
