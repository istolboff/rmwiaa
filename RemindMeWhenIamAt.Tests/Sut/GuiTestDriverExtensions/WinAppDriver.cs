using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal static class WinAppDriver
    {
        public static BrowserWindowDriver AttachToBrowser(IWebDriver webDriver, string? titleSuffix = default)
        {
            var browserWindowHandle = FindBrowserWindow(webDriver.Title + titleSuffix, webDriver);
            return new BrowserWindowDriver(
                new WindowsDriver<WindowsElement>(
                    ServiceUrl,
                    CreateOptions("appTopLevelWindow", browserWindowHandle.ToString("X", CultureInfo.InvariantCulture))),
                webDriver);
        }

        private static bool IsRunning => Process.GetProcessesByName("WinAppDriver").Any();

        private static int FindBrowserWindow(string windowTitle, IWebDriver webDriver)
        {
            try
            {
                using var rootDriver = CreateRootDriver();
// ReSharper disable AccessToDisposedClosure
// Couldn't find a way to fix this with [InstantHandle]
                var webElement = rootDriver.WaitForElement(
                    d => d.TryFindElementByName(windowTitle),
                    D(() => $"Couldn't find browser window with title '{windowTitle}' in " +
                    rootDriver.FindElement(By.XPath("/Pane")).WrappedDriver.PageSource));
// ReSharper enable AccessToDisposedClosure
                return int.Parse(webElement.GetAttribute("NativeWindowHandle"), CultureInfo.InvariantCulture);
            }
            catch (TimeoutException exception)
            {
                throw new InvalidOperationException(
                    $"Failed waiting for Chrome Window with title [{windowTitle}]{Environment.NewLine}" +
                    $"Test-driven Chrome browser's current page source is: {webDriver.PageSource}",
                    exception);
            }
        }

        private static WindowsDriver<WindowsElement> CreateRootDriver()
        {
            Verify.That(IsRunning, "Appium WinAppDriver.exe is supposed to be running.");
            return new WindowsDriver<WindowsElement>(ServiceUrl, CreateOptions("app", "Root"));
        }

        private static AppiumOptions CreateOptions(string capabilityName, object capabilityValue)
        {
            var options = new AppiumOptions();
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability(capabilityName, capabilityValue);
            return options;
        }

        private static readonly Uri ServiceUrl = new Uri("http://127.0.0.1:4723");
    }
}