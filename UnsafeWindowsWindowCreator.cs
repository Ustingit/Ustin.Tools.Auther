using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Authorizer
{
	public static class UnsafeWindowsWindowCreator
	{
		public const uint WM_USER = 0x0400;
		public const uint WS_POPUP = 0x80000000;
		public const uint TTS_ALWAYSTIP = 0x01;
        public const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //static void Main(string[] args)
        //{
        //    string tooltipText = "Custom Tooltip Text";
        //
        //    // Create a tooltip window
        //    IntPtr hWnd = CreateWindowEx(
        //        0, "tooltips_class32", null,
        //        WS_POPUP | TTS_ALWAYSTIP,
        //        0, 0, 0, 0,
        //        IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        //
        //    if (hWnd != IntPtr.Zero)
        //    {
        //        // Set the tooltip text
        //        SendMessage(hWnd, 0x418, IntPtr.Zero, tooltipText); // TTM_ADDTOOL
        //
        //        // Position the tooltip window
        //        SetWindowPos(hWnd, IntPtr.Zero, 100, 100, 0, 0, SWP_NOACTIVATE);
        //
        //        // Show the tooltip
        //        SendMessage(hWnd, 0x404, (IntPtr)1, 0.ToString()); // TTM_TRACKACTIVATE
        //
        //        // Wait for user input (you can perform other operations here)
        //        Console.ReadLine();
        //
        //        // Hide and destroy the tooltip window
        //        SendMessage(hWnd, 0x405, IntPtr.Zero, 0.ToString()); // TTM_TRACKACTIVATE
        //        DestroyWindow(hWnd);
        //    }
        //}

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);
    }
}
