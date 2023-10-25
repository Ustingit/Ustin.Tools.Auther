using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Authorizer
{
	public class WindowsNotificationService : INotificationService
	{
		public async Task TryShowSimpleNotification(Notification notification)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// Create a tooltip window
				IntPtr hWnd = UnsafeWindowsWindowCreator.CreateWindowEx(
					0, "tooltips_class32", null,
					UnsafeWindowsWindowCreator.WS_POPUP | UnsafeWindowsWindowCreator.TTS_ALWAYSTIP,
					0, 0, 0, 0,
					IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

				// Set the tooltip text
				UnsafeWindowsWindowCreator.SendMessage(hWnd, 0x418, IntPtr.Zero, notification.Value); // TTM_ADDTOOL

				// Position the tooltip window
				UnsafeWindowsWindowCreator.SetWindowPos(hWnd, IntPtr.Zero, 100, 100, 0, 0, UnsafeWindowsWindowCreator.SWP_NOACTIVATE);

				// Show the tooltip
				UnsafeWindowsWindowCreator.SendMessage(hWnd, 0x404, (IntPtr)1, 0.ToString()); // TTM_TRACKACTIVATE

				// Hide and destroy the tooltip window
				UnsafeWindowsWindowCreator.SendMessage(hWnd, 0x405, IntPtr.Zero, 0.ToString()); // TTM_TRACKACTIVATE
				UnsafeWindowsWindowCreator.DestroyWindow(hWnd);
			}
		}
	}
}
