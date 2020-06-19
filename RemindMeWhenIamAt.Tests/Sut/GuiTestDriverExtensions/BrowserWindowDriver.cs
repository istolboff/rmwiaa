using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public WindowsElement TopmostPane => _windowsDriver.FindElement(By.XPath("/Pane"));

        public WindowsElement WaitForElement(
            By by,
            Func<WindowsElement, bool>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            WaitForElement(d => d.FindElements(by).SingleOrDefault(extraElementFilter ?? (_ => true)), by.ToString(), timeout);

        public WindowsElement WaitForElement(
            IReadOnlyCollection<By> byAny,
            Func<WindowsElement, bool>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            WaitForElement(
                d => byAny.Select(by => d.FindElements(by).SingleOrDefault(extraElementFilter ?? (_ => true))).FirstOrDefault(e => e != null),
                "Any of " + string.Join("; ", byAny),
                timeout);

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
                            TopmostPane.WrappedDriver.PageSource + Environment.NewLine +
                            "Page HTML: " + $"{Environment.NewLine}\t{_webDriver.PageSource}" + Environment.NewLine +
                            "Browser logs: " + $"{Environment.NewLine}\t" + string.Join($"{Environment.NewLine}\t", _webDriver.GetBrowserLogs()) + Environment.NewLine +
                            GetConsoleScreenshot()),
                timeout);

        private string GetConsoleScreenshot()
        {
            TopmostPane.SendKeys(Keys.Control + Keys.Shift + "i");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var consoleTab = _windowsDriver.FindElements(By.XPath("/Pane/Document/Group/Tab/TabItem")).SingleOrDefault(item => item.Text == "Console");
            var stringBuilder = new StringBuilder();
            if (consoleTab == null)
            {
                stringBuilder.AppendLine("Could not find 'Console' tab in Developer's Instruments. Following is the the whole Window Source after pressing Ctrl + Shift + i: ");
                stringBuilder.AppendLine(TopmostPane.WrappedDriver.PageSource);
            }
            else
            {
                consoleTab.Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            stringBuilder.AppendLine("Screenshot: ");
            stringBuilder.AppendLine(_windowsDriver.GetScreenshot().AsBase64EncodedString);

            return stringBuilder.ToString();
        }

        private readonly WindowsDriver<WindowsElement> _windowsDriver;
        private readonly IWebDriver _webDriver;
    }
}