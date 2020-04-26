using System.Diagnostics;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RemindMeWhenIamAt.Tests.Sut;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal sealed class TestRun
    {
        [BeforeTestRun]
        public static void SetupTestRun() =>
            _serviceProcess = Service.Start();

        [AfterTestRun]
        public static void TeardownTestRun() =>
            _serviceProcess.Kill();

        public TestRun(IObjectContainer diContainer)
        {
            _diContainer = diContainer;
        }

        [BeforeScenario]
        public void InitializeWebDriver()
        {
#pragma warning disable CA2000 // https://specflow.org/documentation/Context-Injection/  If the injected objects implement IDisposable, they will be disposed after the scenario is executed.
            _diContainer.RegisterInstanceAs<IWebDriver>(new ChromeDriver());
#pragma warning restore CA2000
            _diContainer.RegisterInstanceAs(Service.RootUrl);
        }

        [AfterScenario]
        public void QuitWebDriver() =>
            _diContainer.Resolve<IWebDriver>().Quit();

        private static Process _serviceProcess = new Process();
        private readonly IObjectContainer _diContainer;
    }
}