using System;
using OpenQA.Selenium;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class ApplicationUnderTest
    {
        public ApplicationUnderTest(Uri rootUrl, IWebDriver webDriver)
        {
            RootUrl = rootUrl;
            _webDriver = webDriver;
        }

        public Uri RootUrl { get; }

        public TPage NavigateTo<TPage>()
            where TPage : class
            => (TPage)EnsureNotNull(Activator.CreateInstance(typeof(TPage), this));

        public IWebDriver NavigateTo(Uri url)
        {
            _webDriver.Navigate().GoToUrl(url);
            return _webDriver;
        }

        private readonly IWebDriver _webDriver;
    }
}
