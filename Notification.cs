using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorizer
{
	public class Notification
	{
		public string Value { get; set; }

		public NotificationSettings Settings { get; set; }
	}

	public class NotificationSettings
	{
		public ScreenPosition Position { get; set; }
	}

	public class ScreenPosition
	{
		public int X { get; set; }

		public int Y { get; set; }
	}
}