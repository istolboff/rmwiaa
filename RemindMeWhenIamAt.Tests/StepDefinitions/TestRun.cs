using System.Diagnostics;
using RemindMeWhenIamAt.Tests.Sut;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal static class TestRun
    {
        [BeforeTestRun]
        public static void SetupTestRun()
        {
            serviceProcess = Service.Start();
        }

        [AfterTestRun]
        public static void TeardownTestRun()
        {
            serviceProcess.Kill();
        }

        private static Process serviceProcess = new Process();
    }
}
