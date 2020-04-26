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
            _searchContext = applicationUnderTest.NavigateTo(applicationUnderTest.RootUrl);
            WaitUntilPageLoadedCompletely(_searchContext);
        }

        public DefaultPage(ISearchContext searchContext)
        {
            _searchContext = searchContext;
            WaitUntilPageLoadedCompletely(_searchContext);
        }

        public NewReminderPage RequestAddingNewReminder()
        {
            var addReminderLink = _searchContext.FindElement(By.LinkText("Add Reminder"));  /*.FindElement(By.Id("_addReminder"));*/
            addReminderLink.Click();
            addReminderLink.WaitUntilItBecomesStaleBecauseNewPageHasLoaded();
            return new NewReminderPage(_searchContext);
        }

        private static void WaitUntilPageLoadedCompletely(ISearchContext searchContext)
        {
            searchContext.WaitForElementPresence(
                By.Id("_addReminder"),
                errorDescription: "There seems to be an error in the page flow: we are supposed to be at the default application page, while it is some other page.");
        }

        private readonly ISearchContext _searchContext;
    }
}
