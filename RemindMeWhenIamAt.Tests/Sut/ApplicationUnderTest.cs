using System;
using OpenQA.Selenium;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut
{
    #pragma warning disable CA1812 // Yourclass is an internal class that is apparently never instantiated on Derived class
    internal sealed class ApplicationUnderTest
    #pragma warning restore CA1812 // Yourclass is an internal class that is apparently never instantiated on Derived class
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
