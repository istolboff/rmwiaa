using System;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace PwaHomeScreenUtilities
{
    internal sealed class Pwa
    {
        public Pwa(string pwaName)
        {
            _pwaName = pwaName;
        }

        public void AddToHomeScreen()
        {
            using var chromeSession = AttachToPwaBrowserWindow(" - Google Chrome");
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
            Trace.WriteLine($"PWA '{_pwaName}' has been removed from Home Screen.");
        }

        private BrowserWindowDriver AttachToPwaBrowserWindow(string? titleSuffix = default) =>
            BrowserWindowDriver.AttachToWindow(WinAppDriver.FindAppWindow(_pwaName + titleSuffix));

        private readonly string _pwaName;
   }
}