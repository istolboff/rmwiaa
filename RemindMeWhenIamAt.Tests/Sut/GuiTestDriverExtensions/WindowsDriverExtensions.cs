using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal static class WindowsDriverExtensions
    {
        public static WindowsElement WaitForElement(
            this WindowsDriver<WindowsElement> @this,
            Func<WindowsDriver<WindowsElement>, WindowsElement?> findElement,
            ExceptionMessageFactory exceptionMessageFactory,
            TimeSpan? timeout = null)
        =>
            EnsureNotNull(
                Wait.Until(
                    () => findElement(@this),
                    r => r != null,
                    timeout ?? TimeSpan.FromSeconds(10),
                    _ => new InvalidOperationException(exceptionMessageFactory.CreateMessage())));

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