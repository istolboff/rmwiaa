using System.Globalization;
using OpenQA.Selenium;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions;

namespace RemindMeWhenIamAt.Tests.Sut.Pages
{
    internal class NewReminderPage
    {
        public NewReminderPage(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            WaitUntilPageLoadedCompletely(webDriver);
        }

        public DefaultPage AddReminder(Reminder reminder)
        {
            _webDriver.FindElement(By.Id("_latitude")).SendKeys(reminder.Location.Latitude.ToString(CultureInfo.InvariantCulture));
            _webDriver.FindElement(By.Id("_longitude")).SendKeys(reminder.Location.Longitude.ToString(CultureInfo.InvariantCulture));
            _webDriver.FindElement(By.Id("_message")).SendKeys(reminder.Message);
            var submitCommand = _webDriver.FindElement(By.Id("_doAdd"));
            submitCommand.Click();
            submitCommand.WaitUntilItBecomesStaleBecauseNewPageHasLoaded();
            return new DefaultPage(_webDriver);
        }

        private static void WaitUntilPageLoadedCompletely(IWebDriver webDriver)
        {
            webDriver.WaitForElementPresence(By.Id("_doAdd"), pageInfo: "/AddReminder");
        }

        private readonly IWebDriver _webDriver;
    }
}