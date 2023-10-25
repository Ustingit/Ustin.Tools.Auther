using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Authorizer
{
	public class CustomNullLoggerProvider : ILoggerProvider
	{
		public void Dispose() { }

		public ILogger CreateLogger(string categoryName)
		{
			return NullLogger.Instance;
		}
	}
}
