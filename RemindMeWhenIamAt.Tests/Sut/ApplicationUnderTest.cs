using System;
using OpenQA.Selenium;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class ApplicationUnderTest
    {
        public ApplicationUnderTest(Uri rootUrl, IWebDriver webDriver, IBrowserDeveloperTools browserDeveloperTools)
        {
            RootUrl = rootUrl;
            _webDriver = webDriver;
            _browserDeveloperTools = browserDeveloperTools;
        }

        public Uri RootUrl { get; }

        public GeoLocation CurrentGeoLocation
        {
            get => _currentGeoLocation;
            set
            {
                _browserDeveloperTools.SetCurrentLocation(value);
                _currentGeoLocation = value;
            }
        }

        public TPage NavigateTo<TPage>()
            where TPage : class
            => (TPage)EnsureNotNull(Activator.CreateInstance(typeof(TPage), this));

        public IWebDriver NavigateTo(Uri url)
        {
            _webDriver.Navigate().GoToUrl(url);
            return _webDriver;
        }

        private readonly IWebDriver _webDriver;
        private readonly IBrowserDeveloperTools _browserDeveloperTools;
        private GeoLocation _currentGeoLocation;
    }
}
