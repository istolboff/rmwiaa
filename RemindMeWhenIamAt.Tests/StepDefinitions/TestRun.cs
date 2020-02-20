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
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = false,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Service.FullPath
            };

            serviceProcess = Process.Start(startInfo);
        }

        [AfterTestRun]
        public static void TeardownTestRun()
        {
            serviceProcess.Kill();
        }

        private static Process serviceProcess = new Process();
    }
}
