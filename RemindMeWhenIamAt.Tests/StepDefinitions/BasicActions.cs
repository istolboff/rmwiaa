using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using RemindMeWhenIamAt.Tests.Sut;
using RemindMeWhenIamAt.Tests.Sut.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal sealed class BasicActions
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
            using var developerTools = _pwaInChromeDriver.OpenDeveloperTools();
            developerTools.SetCurrentGeoLocation(location);
            MakeCompilerHappy.Use(userName);
        }

        private readonly ApplicationUnderTest _applicationUnderTest;
        private readonly PwaInChrome _pwaInChromeDriver;
    }
}