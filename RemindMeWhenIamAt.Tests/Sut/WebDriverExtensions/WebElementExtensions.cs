using System;
using System.Linq;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions
{
    internal static class WebElementExtensions
    {
        public static void WaitForElementPresence(
            this IWebDriver @this,
            By by,
            TimeSpan? timeout = null,
            string? pageInfo = null)
        {
            Wait.Until(
                () => @this.FindElements(by),
                elements => elements.Any(),
                timeout ?? TimeSpan.FromSeconds(5),
                _ => new InvalidOperationException(DescribeProblem()));

            string DescribeProblem()
            {
                var requestDetails = pageInfo != null ? $"'{pageInfo}' " : string.Empty;
                return $"Could not locate expected element [{by}] on the page served for {requestDetails}request. " +
                       $"The page source is the following:{Environment.NewLine}{@this.PageSource}";
            }
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