using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal sealed class BrowserWindowDriver : IDisposable
    {
        public BrowserWindowDriver(WindowsDriver<WindowsElement> windowsDriver, IWebDriver webDriver)
        {
            _windowsDriver = windowsDriver;
            _webDriver = webDriver;
        }

        public WindowsElement WaitForElement(
            By by,
            Func<WindowsElement, bool>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            WaitForElement(d => d.FindElements(by).SingleOrDefault(extraElementFilter ?? (_ => true)), by.ToString(), timeout);

        public void Dispose()
        {
            _windowsDriver.Dispose();
        }

        private WindowsElement WaitForElement(
            Func<WindowsDriver<WindowsElement>, WindowsElement?> findElement,
            string filterExplanation,
            TimeSpan? timeout = null)
        =>
            _windowsDriver.WaitForElement(
                findElement,
                D(() =>
                    "Couldn't find " + filterExplanation + " in " +
                    _windowsDriver.FindElement(By.XPath("/Pane")).WrappedDriver.PageSource + Environment.NewLine +
                    "Page HTML: " + $"{Environment.NewLine}\t{_webDriver.PageSource}" + Environment.NewLine +
                    "Additional logs: " + $"{Environment.NewLine}\t" + string.Join($"{Environment.NewLine}\t", _webDriver.GetBrowserLogs())),
                timeout);

        private readonly WindowsDriver<WindowsElement> _windowsDriver;
        private readonly IWebDriver _webDriver;
    }
}