using System;
using System.Diagnostics;
using BoDi;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using RemindMeWhenIamAt.Tests.Sut;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.StepDefinitions
{
    [Binding]
    internal sealed class TestRun
    {
        [BeforeTestRun]
        public static void SetupTestRun()
        {
            try
            {
                _serviceProcess = Service.Start();
                _pwaInChromeDriver = new PwaInChrome("Remind Me When I am At...", Service.RootUrl);
                _pwaInChromeDriver.AddPwaToHomeScreen();
            }
            catch (Exception exception) when (!exception.ShouldNotBeCaught())
            {
                Trace.WriteLine($"Encountered exception during Testrun setup: {Environment.NewLine}{exception}{Environment.NewLine}Service log: {Service.ReadOutput()}");
                CloseChromeDriverAndServer();
                throw;
            }
        }

        [AfterTestRun]
        public static void TeardownTestRun()
        {
            if (_pwaInChromeDriver != null)
            {
                try
                {
                    _pwaInChromeDriver.RemoveFromHomeScreen();
                }
                catch (Exception exception) when (!exception.ShouldNotBeCaught())
                {
                    Trace.WriteLine($"Encountered exception during Testrun Teardown: {exception}");
                }
            }

            CloseChromeDriverAndServer();
        }

        public TestRun(IObjectContainer diContainer)
        {
            _diContainer = diContainer;
        }

        [BeforeScenario]
        public void InitializeWebDriver()
        {
            Debug.Assert(_pwaInChromeDriver != null, "Test logic error: _pwaInChromeDriver should have been initialized in SetupTestRun.");
            _diContainer.RegisterInstanceAs(Service.RootUrl);
            _diContainer.RegisterInstanceAs(_pwaInChromeDriver);
            _diContainer.RegisterInstanceAs(_pwaInChromeDriver.WebDriver);
        }

        private static void CloseChromeDriverAndServer()
        {
            if (_pwaInChromeDriver != null)
            {
                _pwaInChromeDriver.Close();
                _pwaInChromeDriver.Dispose();
            }

            if (!string.IsNullOrEmpty(_serviceProcess.StartInfo.FileName))
            {
                _serviceProcess.Kill();
            }
        }

        private static Process _serviceProcess = new Process();
        private static PwaInChrome? _pwaInChromeDriver;
        private readonly IObjectContainer _diContainer;
    }
}