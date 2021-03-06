using System;
using System.Linq;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Miscellaneous;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal static class WebDriverExtensions
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
    }
}