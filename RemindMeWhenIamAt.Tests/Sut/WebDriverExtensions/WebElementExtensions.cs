using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions
{
    internal static class WebElementExtensions
    {
        public static void WaitForElementPresence(
            this ISearchContext @this,
            By by,
            TimeSpan? timeout = null,
            string? errorDescription = null)
        {
            Wait.Until(
                () => @this.FindElements(by),
                elements => elements.Any(),
                timeout ?? TimeSpan.FromSeconds(5),
                errorDescription != null ? _ => new InvalidOperationException(errorDescription) : default(Func<ReadOnlyCollection<IWebElement>, Exception>));
        }

        public static void WaitUntilItBecomesStaleBecauseNewPageHasLoaded(this IWebElement @this) =>
            Wait.Until(@this.IsStale, TimeSpan.FromSeconds(30));

        private static bool IsStale(this IWebElement @this)
        {
            try
            {
                @this.FindElement(By.Id("dummy-element-id"));
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }
    }
}
