using JetBrains.Annotations;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions;

namespace RemindMeWhenIamAt.Tests.Sut.Pages
{
    internal sealed class DefaultPage
    {
        [UsedImplicitly] // see ApplicationUnderTest.NavigateTo<TPage>
        public DefaultPage(ApplicationUnderTest applicationUnderTest)
        {
            _webDriver = applicationUnderTest.NavigateTo(applicationUnderTest.RootUrl);
            WaitUntilPageLoadedCompletely(_webDriver);
        }

        public DefaultPage(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            WaitUntilPageLoadedCompletely(webDriver);
        }

        public NewReminderPage RequestAddingNewReminder()
        {
            var addReminderLink = _webDriver.FindElement(By.LinkText("Add Reminder"));  /*.FindElement(By.Id("_addReminder"));*/
            addReminderLink.Click();
            addReminderLink.WaitUntilItBecomesStaleBecauseNewPageHasLoaded();
            return new NewReminderPage(_webDriver);
        }

        private static void WaitUntilPageLoadedCompletely(IWebDriver webDriver)
        {
            webDriver.WaitForElementPresence(By.Id("_addReminder"), pageInfo: "default page");
        }

        private readonly IWebDriver _webDriver;
    }
}
