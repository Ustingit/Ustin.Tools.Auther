using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authorizer.ClassesGeneration;
using Authorizer.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

				throw new ArgumentNullException();

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

				var deserializedObject = JsonConvert.DeserializeObject(content, responseClass);
				var typedObject = Convert.ChangeType(deserializedObject, responseClass);

				return (JsonConvert.DeserializeObject<AuthInfo>(content))?.AccessValue?.Value ?? string.Empty;
			}
			catch (Exception e)
			{
				return string.Empty;
			}
		}
	}
}
