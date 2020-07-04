using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal static class WinAppDriver
    {
        public static WindowsDriver<WindowsElement> GetTopLevelWindowDriver(string browserWindowHandle) =>
            new WindowsDriver<WindowsElement>(
                    ServiceUrl,
                    CreateOptions("appTopLevelWindow", int.Parse(browserWindowHandle, CultureInfo.InvariantCulture).ToString("X", CultureInfo.InvariantCulture)));

        public static string FindAppWindow(string windowTitle, IWebDriver? webDriver = default) =>
            Wait.Until(
                () => FindAppWindowCore(windowTitle, webDriver),
                windowHandle => windowHandle != "0",
                TimeSpan.FromSeconds(5));

        private static bool IsRunning => Process.GetProcessesByName("WinAppDriver").Any();

        private static string FindAppWindowCore(string windowTitle, IWebDriver? webDriver = default)
        {
            try
            {
                using var rootDriver = CreateRootDriver();
                // ReSharper disable AccessToDisposedClosure
                // Couldn't find a way to fix this with [InstantHandle]
                var webElement = rootDriver.WaitForElement(
                    d => d.TryFindElementByName(windowTitle),
                    D(() => $"Couldn't find browser window with title '{windowTitle}'"));
                // ReSharper enable AccessToDisposedClosure
                return webElement.GetAttribute("NativeWindowHandle");
            }
            catch (TimeoutException exception)
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.Append($"Failed waiting for Chrome Window with title [{windowTitle}]");
                if (webDriver != null)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.Append($"Test-driven Chrome browser's current page source is: {webDriver.PageSource}");
                }

                throw new InvalidOperationException(messageBuilder.ToString(), exception);
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