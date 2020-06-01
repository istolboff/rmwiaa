using System;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal static class WebElementExtensions
    {
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