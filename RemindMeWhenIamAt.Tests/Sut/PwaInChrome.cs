using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Chrome;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class PwaInChrome : IDisposable
    {
        public PwaInChrome(string pwaName, Uri pwaUri)
        {
            _pwaName = pwaName;
            _pwaUri = pwaUri;
            Trace.WriteLine("Chrome version: " + FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe").FileVersion);
            (_chromeDriver, _rootChromeProcessCreatedByChromeDriver) = StartChromeDriver();
            Trace.WriteLine($"Navigating to PWA '{pwaName}' at {pwaUri}...");
            _chromeDriver.Navigate().GoToUrl(pwaUri);
        }

        public IWebDriver WebDriver => _chromeDriver;

        public void AddPwaToHomeScreen()
        {
            using var chromeSession = AttachToPwaBrowserWindow();
            ClickInstallButtonAftewrShortDelay(
                chromeSession.WaitForElement(
                new[]
                {
                    By.XPath("/Pane/Pane/Pane/Pane/Pane/Group/Pane/Button"),
                    By.XPath("/Pane/Pane/Pane/Pane/Pane/Pane/Button")
                },
                element => element.Text.Contains(_pwaName, StringComparison.OrdinalIgnoreCase)));

            EmulateClickingTheButtonBySendingEnterKey(
                chromeSession.WaitForElement(
                    By.XPath("/Pane/Pane/Pane/Pane/Pane/Button"),
                    e => e.Text == "Install"));

            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Trace.WriteLine($"PWA '{_pwaName}' has been added to Home Screen.");
            _chromeDriver.Navigate().Refresh(); // in order to let Chrome realize that it's now in a Home Screen mode
            _pwaHasBeenAddedToHomeScreen = true;

            // for some reason, Click() on this button not always works,
            // perhaps because Chrome "animates" it, by moving it to the right soon after it appears.
            void ClickInstallButtonAftewrShortDelay(WindowsElement button)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                button.Click();
            }

            // .Click() doesn't work for this button for some reason.
            void EmulateClickingTheButtonBySendingEnterKey(WindowsElement button) => button.SendKeys(Keys.Enter);
        }

        public void RemoveFromHomeScreen()
        {
            using var chromeSession = AttachToPwaBrowserWindow();

            chromeSession.WaitForElement(By.XPath("/Pane/Pane/Pane/Pane/Pane/MenuItem"))
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/MenuBar/Pane/Menu/MenuItem"),
                menuItem => menuItem.Text.IndexOf(_pwaName, StringComparison.OrdinalIgnoreCase) >= 0)
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/Pane/Pane/Pane/CheckBox"),
                checkBox => checkBox.Text.Contains("also clear data from chrome", StringComparison.OrdinalIgnoreCase))
                .Click();

            chromeSession.WaitForElement(
                By.XPath("/Pane/Pane/Pane/Pane/Pane/Button"),
                button => button.Text.Equals("Remove", StringComparison.OrdinalIgnoreCase))
                .Click();

            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            _pwaHasBeenAddedToHomeScreen = false;
            Trace.WriteLine($"PWA '{_pwaName}' has been removed from Home Screen.");
        }

        public ChromeDeveloperTools OpenDeveloperTools()
        {
            if (_pwaHasBeenAddedToHomeScreen)
            {
                using (var chromeSession = AttachToPwaBrowserWindow())
                {
                    chromeSession.TopmostPane.SendKeys(Keys.Control + Keys.Shift + "i");
                }

                var devToolsWindow = BrowserWindowDriver.AttachToWindow(WinAppDriver.FindAppWindow($"DevTools - {_pwaUri.CutSchemeOff()}"));
                return new ChromeDeveloperTools(devToolsWindow);
            }
            else
            {
                var chromeSession = AttachToPwaBrowserWindow();
                chromeSession.TopmostPane.SendKeys(Keys.Control + Keys.Shift + "i");
                return new ChromeDeveloperTools(chromeSession);
            }
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
        }

        private BrowserWindowDriver AttachToPwaBrowserWindow() =>
            BrowserWindowDriver.AttachToWindow(
                WinAppDriver.FindAppWindow(
                    WebDriver.Title + (_pwaHasBeenAddedToHomeScreen ? default : " - Google Chrome"),
                    WebDriver),
                WebDriver);

        private static (ChromeDriver chromeDriver, Process rootChromeProcessCreatedByChromeDriver) StartChromeDriver()
        {
            var existingChromeProcesseIds = ListChromeProcessIds().Select(p => p.Id).ToArray();
            var options = new ChromeOptions();
            options.AddArguments("--lang=en");
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            var chromeDriver = new ChromeDriver(options);
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
        private readonly Uri _pwaUri;
        private readonly ChromeDriver _chromeDriver;
        private readonly Process _rootChromeProcessCreatedByChromeDriver;
        private bool _pwaHasBeenAddedToHomeScreen;
    }
}