using System;
using Newtonsoft.Json.Linq;

namespace Authorizer.Extentions.Json
{
	public static class JsonExtensions
	{
		public static JToken GetTokenByPath(this JObject jsonObject, string propertyPath)
		{
			if (jsonObject is null) throw new ArgumentNullException(nameof(jsonObject));

			var pathSegments = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
			JToken token = jsonObject;

			foreach (var segment in pathSegments)
			{
				if (token is JObject || token is JArray)
				{
					token = token.SelectToken(segment);
				}
				else
				{
					return null; // Invalid property path
				}
			}

			return token;
		}
	}
}
