using System;
using System.Runtime.InteropServices;
using System.Security;

//http://stackoverflow.com/questions/1953424/is-there-a-way-to-start-open-the-console-from-a-winforms-wpf-application
namespace ServerViewWPF
{
    static class DebugConsole
    {

        public static int CreateConsole()
        {
            if (AllocConsole())
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning! Closing the console will also close the program.");
                Console.ResetColor();
                return 0;
            }
            else
                return Marshal.GetLastWin32Error();
        }

        public static int Destroy()
        {
            if (FreeConsole())
                return 0;
            else
                return Marshal.GetLastWin32Error();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();
    }
}
