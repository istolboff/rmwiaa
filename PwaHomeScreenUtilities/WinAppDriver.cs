using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace PwaHomeScreenUtilities
{
    internal static class WinAppDriver
    {
        public static WindowsDriver<WindowsElement> GetTopLevelWindowDriver(string browserWindowHandle) =>
            new WindowsDriver<WindowsElement>(
                    ServiceUrl,
                    CreateOptions("appTopLevelWindow", int.Parse(browserWindowHandle, CultureInfo.InvariantCulture).ToString("X", CultureInfo.InvariantCulture)));

        public static string FindAppWindow(string windowTitle) =>
            Wait.Until(
                () => FindAppWindowCore(windowTitle),
                windowHandle => windowHandle != "0",
                TimeSpan.FromSeconds(5));

        private static bool IsRunning => Process.GetProcessesByName("WinAppDriver").Any();

        private static string FindAppWindowCore(string windowTitle)
        {
            try
            {
                using var rootDriver = CreateRootDriver();
                // ReSharper disable AccessToDisposedClosure
                // Couldn't find a way to fix this with [InstantHandle]
                var webElement = rootDriver.WaitForElement(
                    d => d.TryFindElementByName(windowTitle),
                    () => $"Couldn't find browser window with title '{windowTitle}'");
                // ReSharper enable AccessToDisposedClosure
                return webElement.GetAttribute("NativeWindowHandle");
            }
            catch (TimeoutException exception)
            {
                throw new InvalidOperationException($"Failed waiting for Chrome Window with title [{windowTitle}]", exception);
            }
        }

        private static WindowsDriver<WindowsElement> CreateRootDriver()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(SuppressCa1303("Appium WinAppDriver.exe is supposed to be running."));
            }

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