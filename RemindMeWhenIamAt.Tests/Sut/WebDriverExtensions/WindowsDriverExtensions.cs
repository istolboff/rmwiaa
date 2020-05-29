using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions
{
    internal static class WindowsDriverExtensions
    {
        public static WindowsElement WaitForElement(
            this WindowsDriver<WindowsElement> @this,
            string name,
            TimeSpan? timeout = null)
        =>
            @this.WaitForElement(d => d.TryFindElementByName(name), $" element with the name '{name}'", timeout);

        public static WindowsElement WaitForElement(
            this WindowsDriver<WindowsElement> @this,
            By by,
            Func<WindowsElement, bool>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            @this.WaitForElement(d => d.FindElements(by).SingleOrDefault(extraElementFilter ?? (_ => true)), by.ToString(), timeout);

        private static WindowsElement WaitForElement(
            this WindowsDriver<WindowsElement> @this,
            Func<WindowsDriver<WindowsElement>, WindowsElement?> findElement,
            string filterExplanation,
            TimeSpan? timeout = null)
        =>
            EnsureNotNull(
                Wait.Until(
                    () => findElement(@this),
                    r => r != null,
                    timeout ?? TimeSpan.FromSeconds(10),
                    _ => new InvalidOperationException(
                        "Couldn't find " + filterExplanation + " in " +
                        @this.FindElement(By.XPath("/Pane")).WrappedDriver.PageSource)));

        private static WindowsElement? TryFindElementByName(this WindowsDriver<WindowsElement> @this, string name)
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