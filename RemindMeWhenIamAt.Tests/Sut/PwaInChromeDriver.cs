using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Chrome;
using RemindMeWhenIamAt.Tests.Sut.WebDriverExtensions;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class PwaInChromeDriver : IDisposable
    {
        public PwaInChromeDriver(string pwaName, Uri pwaUri)
        {
            _pwaName = pwaName;
            (_chromeDriver, _rootChromeProcessCreatedByChromeDriver) = StartChromeDriver();
            _chromeDriver.Navigate().GoToUrl(pwaUri);
        }

        public IWebDriver WebDriver => _chromeDriver;

        public void AddPwaToHomeScreen()
        {
            using var chromeSession = CreatePwaWindowDriver(_pwaName + " - Google Chrome");
            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/Pane/Pane/Pane/Pane/Button"),
                element => element.Text.Contains(_pwaName, StringComparison.OrdinalIgnoreCase))
                .Click();
            EmulateClickingTheButtonBySendingEnterKey(
                chromeSession.WaitForElement(
                    By.XPath("/Pane/Pane/Pane/Pane/Pane/Button"),
                    e => e.Text == "Установить"));
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Trace.WriteLine($"PWA '{_pwaName}' has been added to Home Screen.");
            _chromeDriver.Navigate().Refresh(); // in order to let chrome realize that it's now in a Home Screen mode

            // .Click() doesn't work for this button for some reason.
            void EmulateClickingTheButtonBySendingEnterKey(WindowsElement button) => button.SendKeys(Keys.Enter);
        }

        public void RemoveFromHomeScreen()
        {
            using var chromeSession = CreatePwaWindowDriver(_pwaName);

            chromeSession.WaitForElement(By.XPath("/Pane/Pane/Pane/Pane/Pane/MenuItem"))
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/MenuBar/Pane/Menu/MenuItem"),
                menuItem => menuItem.Text.IndexOf(_pwaName, StringComparison.OrdinalIgnoreCase) >= 0)
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/Pane/Pane/Pane/CheckBox"),
                checkBox => checkBox.Text.Contains("удалить данные из Chrome", StringComparison.OrdinalIgnoreCase))
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/Pane/Pane/Pane/Button"),
                button => button.Text.Equals("Удалить", StringComparison.OrdinalIgnoreCase))
                .Click();

            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            Trace.WriteLine($"PWA '{_pwaName}' has been removed from Home Screen.");
        }

        public void Close()
        {
            try
            {
                _chromeDriver.Close();
            }
            catch (WebDriverException)
            {
            }

            _rootChromeProcessCreatedByChromeDriver.Kill();
        }

        public void Dispose()
        {
            _chromeDriver?.Dispose();
            _desktopSession?.Dispose();
        }

        private WindowsDriver<WindowsElement> CreatePwaWindowDriver(string browserWindowTitle)
        {
            var webElement = _desktopSession.WaitForElement(browserWindowTitle);
            var handle = int.Parse(webElement.GetAttribute("NativeWindowHandle"), CultureInfo.InvariantCulture);

            var chromeOptions = new AppiumOptions();
            chromeOptions.AddAdditionalCapability("platformName", "Windows");
            chromeOptions.AddAdditionalCapability("deviceName", "WindowsPC");
            chromeOptions.AddAdditionalCapability("appTopLevelWindow", handle.ToString("X", CultureInfo.InvariantCulture));

            return new WindowsDriver<WindowsElement>(new Uri(WinAppDriverUrl), chromeOptions);
        }

        private static WindowsDriver<WindowsElement> CreateWindowsDriver()
        {
            var options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability("app", "Root");
            return new WindowsDriver<WindowsElement>(new Uri(WinAppDriverUrl), options);
        }

        private static (ChromeDriver chromeDriver, Process rootChromeProcessCreatedByChromeDriver) StartChromeDriver()
        {
            var existingChromeProcesseIds = ListChromeProcessIds().Select(p => p.Id).ToArray();
            var chromeDriver = new ChromeDriver();
            var rootChromeProcessCreatedByChromeDriver = PickRootChromeProcessCreatedByChromeDriver(
                ListChromeProcessIds().Where(p => !existingChromeProcesseIds.Contains(p.Id)).ToArray());
            return (chromeDriver, rootChromeProcessCreatedByChromeDriver);

            IEnumerable<Process> ListChromeProcessIds() =>
                Process.GetProcesses().Where(p => p.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase));

            Process PickRootChromeProcessCreatedByChromeDriver(IReadOnlyCollection<Process> chromeProcessesCreatedByChromeDriver)
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT ProcessId, CommandLine FROM Win32_Process WHERE {string.Join(" or ", chromeProcessesCreatedByChromeDriver.Select(p => "ProcessId=" + p.Id))}");
                using ManagementObjectCollection objects = searcher.Get();
                var rootProcessId = int.Parse(
                    objects
                        .Cast<ManagementBaseObject>()
                        .Select(it => new { ProcessId = it["ProcessId"]?.ToString(), CommandLine = it["CommandLine"]?.ToString() })
                        .Single(it => it.CommandLine != null && !it.CommandLine.Contains("--type=", StringComparison.OrdinalIgnoreCase))
                        .ProcessId ?? string.Empty,
                    CultureInfo.InvariantCulture);
                return chromeProcessesCreatedByChromeDriver.Single(p => p.Id == rootProcessId);
            }
        }

        private readonly string _pwaName;
        private readonly WindowsDriver<WindowsElement> _desktopSession = CreateWindowsDriver();
        private readonly ChromeDriver _chromeDriver;
        private readonly Process _rootChromeProcessCreatedByChromeDriver;

        private const string WinAppDriverUrl = "http://127.0.0.1:4723";
    }
}