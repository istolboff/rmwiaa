using System;
using System.Threading;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class ExceptionExtensions
    {
        public static bool ShouldNotBeCaught(this Exception exception)
        {
            return

                // Boneheaded (see https://blogs.msdn.microsoft.com/ericlippert/2008/09/10/vexing-exceptions/)
                exception is NotImplementedException ||
                exception is NullReferenceException ||
                exception is ArgumentException ||
                exception is InvalidCastException ||
                exception is IndexOutOfRangeException ||
                exception is DivideByZeroException ||
                exception is AccessViolationException ||

                // Fatal
                exception is ThreadAbortException ||
                exception is OutOfMemoryException ||
                exception is DataMisalignedException ||
                exception is BadImageFormatException;
        }
    }
}