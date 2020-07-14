using OpenQA.Selenium.DevTools.Emulation;
using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using RemindMeWhenIamAt.Tests.Sut;
using RemindMeWhenIamAt.Tests.Sut.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    #pragma warning disable CA1812 // Yourclass is an internal class that is apparently never instantiated on Derived class
    internal sealed class BasicActions
    #pragma warning disable CA1812 // Yourclass is an internal class that is apparently never instantiated on Derived class
    {
        public BasicActions(ApplicationUnderTest applicationUnderTest, PwaInChrome pwaInChromeDriver)
        {
            _applicationUnderTest = applicationUnderTest;
            _pwaInChromeDriver = pwaInChromeDriver;
        }

        [Given(@"user (.*) sets up the following reminders?")]
        public void UserSetsUpReminder(string userName, Table remindersToSetup)
        {
            var indexPage = _applicationUnderTest.NavigateTo<DefaultPage>();
            foreach (var reminder in remindersToSetup.CreateSet<Reminder>())
            {
                var newReminderPage = indexPage.RequestAddingNewReminder();
                indexPage = newReminderPage.AddReminder(reminder);
            }

            MakeCompilerHappy.Use(userName);
        }

        [When(@"user (.*) gets near the (.*) location")]
        public void UserGetsNearLocation(string userName, GeoLocation location)
        {
            _pwaInChromeDriver.DevToolsSession.SendCommand(
                new SetGeolocationOverrideCommandSettings
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Accuracy = 100
                })
                .Wait();
            MakeCompilerHappy.Use(userName);
        }

        private readonly ApplicationUnderTest _applicationUnderTest;
        private readonly PwaInChrome _pwaInChromeDriver;
    }
}