using System;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal sealed class ChromeDeveloperTools : IDisposable
    {
        public ChromeDeveloperTools(BrowserWindowDriver browserWindowDriver)
        {
            _browserWindowDriver = browserWindowDriver;
        }

        public void SetCurrentGeoLocation(GeoLocation value)
        {
            _disposedChecker.Check();

            _browserWindowDriver
                .WaitForElement(
                    By.XPath("/Pane/Document/Group/MenuItem"),
                    item => item.Text == "Customize and control DevTools")
                .Click();

            var devToolsDocument = _browserWindowDriver.WaitForElement(
                By.XPath("/Pane/Document[@ClassName=\"Chrome_RenderWidgetHostHWND\"]"));

            devToolsDocument.SendKeys(6.Times(Keys.ArrowDown));
            devToolsDocument.SendKeys(Keys.ArrowRight);
            devToolsDocument.SendKeys(12.Times(Keys.ArrowDown));
            devToolsDocument.SendKeys(Keys.Return);
            devToolsDocument.SendKeys(Keys.Tab);
            devToolsDocument.SendKeys(Keys.Return);
            devToolsDocument.SendKeys(10.Times(Keys.ArrowDown));
            devToolsDocument.SendKeys(Keys.Return);
            devToolsDocument.SendKeys(value.X.ToString(CultureInfo.InvariantCulture));
            devToolsDocument.SendKeys(Keys.Tab);
            devToolsDocument.SendKeys(value.X.ToString(CultureInfo.InvariantCulture));
            devToolsDocument.SendKeys(Keys.Tab);
            devToolsDocument.SendKeys(Keys.Tab);
        }

        public void Dispose()
        {
            if (!_disposedChecker.Dispose())
            {
                return;
            }

            try
            {
                _browserWindowDriver.TopmostPane.SendKeys(Keys.Control + Keys.Shift + "i");
            }
            catch (WebDriverException exception)
            {
                Trace.WriteLine($"Failed to close Chrome Development Tools: {exception.Message}");
            }

            _browserWindowDriver.Dispose();
        }

        private readonly BrowserWindowDriver _browserWindowDriver;
        private readonly ObjectDisposedChecker _disposedChecker = new ObjectDisposedChecker("ChromeDeveloperTools");
    }
}