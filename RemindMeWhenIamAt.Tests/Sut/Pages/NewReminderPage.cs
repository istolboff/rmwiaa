using System.Globalization;
using OpenQA.Selenium;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions;

namespace RemindMeWhenIamAt.Tests.Sut.Pages
{
    internal class NewReminderPage
    {
        public NewReminderPage(ISearchContext searchContext)
        {
            _searchContext = searchContext;
            WaitUntilPageLoadedCompletely(searchContext);
        }

        public DefaultPage AddReminder(Reminder reminder)
        {
            _searchContext.FindElement(By.Id("_x")).SendKeys(reminder.Location.X.ToString(CultureInfo.InvariantCulture));
            _searchContext.FindElement(By.Id("_y")).SendKeys(reminder.Location.Y.ToString(CultureInfo.InvariantCulture));
            _searchContext.FindElement(By.Id("_message")).SendKeys(reminder.Message);
            var submitCommand = _searchContext.FindElement(By.Id("_doAdd"));
            submitCommand.Click();
            submitCommand.WaitUntilItBecomesStaleBecauseNewPageHasLoaded();
            return new DefaultPage(_searchContext);
        }

        private static void WaitUntilPageLoadedCompletely(ISearchContext searchContext)
        {
            searchContext.WaitForElementPresence(
                By.Id("_doAdd"),
                errorDescription: "There seems to be an error in the page flow: we are supposed to be at the default AddReminder page, while it is some other page.");
        }

        private readonly ISearchContext _searchContext;
    }
}