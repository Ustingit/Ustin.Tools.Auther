using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authorizer
{
	internal interface IAuthorizer
	{
		public Task<string> GetAuthorizationToken(CancellationToken cancellationToken);
	}
}
