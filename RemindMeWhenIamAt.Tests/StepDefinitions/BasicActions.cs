using RemindMeWhenIamAt.SharedCode;
using RemindMeWhenIamAt.Tests.Sut.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal static class BasicActions
    {
        [Given(@"user (.*) sets up the following reminders?")]
        public static void GivenUserSetsUpTheFollowingReminder(string userName, Table remindersToSetup)
        {
            var indexPage = TestRun.ApplicationUnderTest.NavigateTo<DefaultPage>();
            foreach (var reminder in remindersToSetup.CreateSet<Reminder>())
            {
                var newReminderPage = indexPage.RequestAddingNewReminder();
                indexPage = newReminderPage.AddReminder(reminder);
            }

            MakeCompilerHappy.Use(userName);
        }
    }
}