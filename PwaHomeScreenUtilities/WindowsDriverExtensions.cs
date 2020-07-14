using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace PwaHomeScreenUtilities
{
    internal static class WindowsDriverExtensions
    {
        public static WindowsElement WaitForElement(
            this WindowsDriver<WindowsElement> @this,
            Func<WindowsDriver<WindowsElement>, WindowsElement?> findElement,
            Func<string> exceptionMessageFactory,
            TimeSpan? timeout = null,
            string? waitingForWhat = default) =>
                EnsureNotNull(
                    Wait.Until(
                        () => findElement.TraceExceptions()(@this),
                        r => r != null,
                        timeout ?? TimeSpan.FromSeconds(10),
                        _ => new InvalidOperationException(exceptionMessageFactory()),
                        waitingForWhat));

        public static WindowsElement? TryFindElementByName(this WindowsDriver<WindowsElement> @this, string name)
        {
            try
            {
                return @this.FindElementByName(name);
            }
            catch (WebDriverException)
            {
                return null;
            }
        }
    }
}