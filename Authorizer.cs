using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authorizer.ClassesGeneration;
using Authorizer.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Authorizer
{
	internal class Authorizer : IAuthorizer
	{
		private readonly AppSettings _settings;

		public Authorizer(IOptions<AppSettings> settings)
		{
			_settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
		}

		public async Task<string> GetAuthorizationToken(CancellationToken cancellationToken)
		{
			try
			{
				var responseClass = _settings.Classes.BuildHierarchy();

				//throw new ArgumentNullException();

				using var client = new HttpClient();
				var request = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl);
				request.Headers.Add(_settings.ApiKeyHeaderName, _settings.ApiKey);
				request.Content = new StringContent(JsonConvert.SerializeObject(new SignInRequest
				{
					ClientId = _settings.ClientId,
					CompanyId = _settings.CompanyId,
					Domain = _settings.Domain,
					Email = _settings.Email
				}), Encoding.UTF8, "application/json");

				HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

				response.EnsureSuccessStatusCode();
				var content = await response.Content.ReadAsStringAsync(cancellationToken);

				//var deserializedObject = JsonConvert.DeserializeObject(content, responseClass);
				//var typedObject = Convert.ChangeType(deserializedObject, responseClass);

				dynamic data = JsonConvert.DeserializeObject(content);

				var test1 = data as JObject;
				var test2 = test1.Properties().FirstOrDefault(pr => pr.Name == "accessValue")?.Value as JObject;
				var test3 = test2.Properties().FirstOrDefault(pr => pr.Name == "value")?.Value as JToken;
				var final = test3.ToString();

				var test5 = data as JObject;
				JToken token = GetTokenByPath(test5, "accessValue.value");
				var final2 = token.ToString();

				return (JsonConvert.DeserializeObject<AuthInfo>(content))?.AccessValue?.Value ?? string.Empty;
			}
			catch (Exception e)
			{
				return string.Empty;
			}
		}

		private static JToken GetTokenByPath(JObject jsonObject, string propertyPath)
		{
			string[] pathSegments = propertyPath.Split('.');
			JToken token = jsonObject;

			foreach (string segment in pathSegments)
			{
				if (token is JObject || token is JArray)
				{
					token = token.SelectToken(segment);
				}
				else
				{
					// Invalid property path
					return null;
				}
			}

			return token;
		}
	}
}
