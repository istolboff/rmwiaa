using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Android.Enums;
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
                    {
                        var mainBrowserPane = _windowsDriver.FindElement(By.XPath("/Pane"));
                        return "Couldn't find " + filterExplanation + " in " +
                            mainBrowserPane.WrappedDriver.PageSource + Environment.NewLine +
                            "Page HTML: " + $"{Environment.NewLine}\t{_webDriver.PageSource}" + Environment.NewLine +
                            "Browser logs: " + $"{Environment.NewLine}\t" + string.Join($"{Environment.NewLine}\t", _webDriver.GetBrowserLogs()) + Environment.NewLine +
                            GetConsoleScreenshot(mainBrowserPane);
                    }),
                timeout);

        private string GetConsoleScreenshot(WindowsElement mainBrowserPane)
        {
            mainBrowserPane.SendKeys(Keys.Control + Keys.Shift + "i");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var consoleTab = _windowsDriver.FindElements(By.XPath("/Pane/Document/Group/Tab/TabItem")).SingleOrDefault(item => item.Text == "Console");
            if (consoleTab == null)
            {
                return "Could not find 'Console' tab in Developer's Instruments. Following is the the whole Window Source after pressing Ctrl + Shift + i: " +
                        Environment.NewLine +
                        mainBrowserPane.WrappedDriver.PageSource;
            }

            consoleTab.Click();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return "Screenshot: " + Environment.NewLine + _windowsDriver.GetScreenshot().AsBase64EncodedString;
        }

        private readonly WindowsDriver<WindowsElement> _windowsDriver;
        private readonly IWebDriver _webDriver;
    }
}