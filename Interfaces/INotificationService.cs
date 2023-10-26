using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorizer
{
	public interface INotificationService
	{
		Task TryShowSimpleNotification(Notification notification);
	}
}
