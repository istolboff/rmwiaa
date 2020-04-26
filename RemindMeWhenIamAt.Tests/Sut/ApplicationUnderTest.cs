using System;
using OpenQA.Selenium;
using RemindMeWhenIamAt.Tests.Miscellaneous;

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
            => (TPage)MakeCompilerHappy.EnsureNotNull(Activator.CreateInstance(typeof(TPage), this));

        public ISearchContext NavigateTo(Uri url)
        {
            _webDriver.Navigate().GoToUrl(url);
            return _webDriver;
        }

        private readonly IWebDriver _webDriver;
    }
}
