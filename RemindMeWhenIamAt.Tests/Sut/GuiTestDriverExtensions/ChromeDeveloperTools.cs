using System.Globalization;
using OpenQA.Selenium;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal sealed class ChromeDeveloperTools : IBrowserDeveloperTools
    {
        public ChromeDeveloperTools(BrowserWindowDriver browserWindowDriver)
        {
            _browserWindowDriver = browserWindowDriver;
        }

        public void SetCurrentLocation(GeoLocation value)
        {
            var topmostPane = _browserWindowDriver.TopmostPane;
            topmostPane.SendKeys(Keys.Control + Keys.Shift + "i");
            _browserWindowDriver
                .WaitForElement(
                    By.XPath("/Pane/Document/Group/MenuItem"),
                    item => item.Text == "Customize and control DevTools")
                .Click();

            var devToolsDocument = _browserWindowDriver.WaitForElement(
                By.XPath($"/Pane[@ClassName=\"Chrome_WidgetWin_1\"][@Name=\"{topmostPane.Text}\"]/Document[@ClassName=\"Chrome_RenderWidgetHostHWND\"]"));

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

        private readonly BrowserWindowDriver _browserWindowDriver;
    }
}