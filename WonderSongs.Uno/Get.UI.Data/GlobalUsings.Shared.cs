global using System;
global using System.Linq;
global using System.Diagnostics.CodeAnalysis;
global using System.Diagnostics;
global using static Get.Data.Properties.AutoTyper;
global using static Get.Data.XACL.QuickBindingExtension;
global using static Get.UI.Data.QuickCreate;
using System.Runtime.InteropServices;

namespace Get.UI.Data;
internal static class SelfNote
{
    [DoesNotReturn]
    public static void ThrowNotImplemented() => throw new NotImplementedException();
    [DoesNotReturn]
    public static T ThrowNotImplemented<T>() => throw new NotImplementedException();
    /// <summary>
    /// Notes that the following code has the code that is not allowed in UWP certification.
    /// </summary>
    public static void HasDisallowedPInvoke() { }
    public static void DebugBreakOnShift()
    {
#if DEBUG
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);
        if (GetAsyncKeyState(16) != 0)
            Debugger.Break();
#endif
    }
}