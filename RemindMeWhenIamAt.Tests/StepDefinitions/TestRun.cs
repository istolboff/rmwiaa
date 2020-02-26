using System;
using System.Diagnostics;
using RemindMeWhenIamAt.Tests.Sut;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal static class TestRun
    {
        public static ApplicationUnderTest ApplicationUnderTest
        {
            get => applicationUnderTest
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                ?? throw new InvalidOperationException("Test suite logic exception: _applicationUnderTest should have been initialized during TestRun setup.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        [BeforeTestRun]
        public static void SetupTestRun()
        {
            serviceProcess = Service.Start();
            applicationUnderTest = new ApplicationUnderTest();
        }

        [AfterTestRun]
        public static void TeardownTestRun()
        {
            serviceProcess.Kill();
        }

        private static Process serviceProcess = new Process();
        private static ApplicationUnderTest? applicationUnderTest;
    }
}
