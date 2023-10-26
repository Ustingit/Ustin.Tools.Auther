using System;

namespace Authorizer.Extentions.Common
{
	public static class StringExtensions
	{
		#region Casts

		public static bool TryCastToBool(this string source, out bool result)
		{
			try
			{
				result = Convert.ToBoolean(source);
				return true;
			}
			catch (Exception _)
			{
				result = false;
				return false;
			}
		} 

		#endregion
	}
}
