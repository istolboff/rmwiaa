using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal sealed class BrowserWindowDriver : IDisposable
    {
        private BrowserWindowDriver(WindowsDriver<WindowsElement> windowsDriver, IWebDriver? webDriver)
        {
            _windowsDriver = windowsDriver;
            _webDriver = webDriver;
            TopmostPane = WaitForElement(By.XPath("/Pane"));
        }

        public WindowsElement TopmostPane { get; }

        public WindowsElement WaitForElement(
            By by,
            Expression<Func<WindowsElement, bool>>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            WaitForElement(d => d.FindElements(by).SingleOrDefault(extraElementFilter?.Compile() ?? (_ => true)), $"{by} && {extraElementFilter}", timeout);

        public WindowsElement WaitForElement(
            IReadOnlyCollection<By> byAny,
            Expression<Func<WindowsElement, bool>>? extraElementFilter = null,
            TimeSpan? timeout = null)
        =>
            WaitForElement(
                d => byAny.Select(by => d.FindElements(by).SingleOrDefault(extraElementFilter?.Compile() ?? (_ => true))).FirstOrDefault(e => e != null),
                "Any of " + string.Join("; ", byAny) + $" && {extraElementFilter}",
                timeout);

        public void Dispose()
        {
            _windowsDriver.Dispose();
        }

        public static BrowserWindowDriver AttachToWindow(string browserWindowHandle, IWebDriver? webDriver = default)
        {
            var windowDriver = WinAppDriver.GetTopLevelWindowDriver(browserWindowHandle);
            var windowTitle = windowDriver.FindElement(By.XPath("/Pane")).Text;
            var result = new BrowserWindowDriver(windowDriver, webDriver);
            Trace.WriteLine($"Attached to appTopLevelWindow [{windowTitle}]");
            return result;
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
                    var result = new StringBuilder();
                    result.Append($"Couldn't find {filterExplanation} in {TryGetTopmostPanePageSource()}");
                    if (_webDriver != null)
                    {
                        result.AppendLine();
                        result.AppendLine($"Page HTML: {Environment.NewLine}\t{_webDriver.PageSource}");
                        result.Append("Browser logs: " + $"{Environment.NewLine}\t" + string.Join($"{Environment.NewLine}\t", _webDriver.GetBrowserLogs()));
                    }

                    return result.ToString();
                }),
                timeout,
                filterExplanation);

        private string TryGetTopmostPanePageSource()
        {
            try
            {
                return TopmostPane.WrappedDriver.PageSource;
            }
            catch (WebDriverException)
            {
                return "Could not get WrappedDriver's PageSource";
            }
        }

        private readonly WindowsDriver<WindowsElement> _windowsDriver;
        private readonly IWebDriver? _webDriver;
    }
}