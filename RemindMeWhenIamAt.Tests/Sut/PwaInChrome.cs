using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using RemindMeWhenIamAt.Server;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class PwaInChrome : IDisposable
    {
        public PwaInChrome(string pwaName, Uri pwaUri)
        {
            _pwaName = pwaName;
            Trace.WriteLine("Chrome version: " + FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe").FileVersion);
            (WebDriver, _rootChromeProcessCreatedByChromeDriver) = StartChromeDriver();
            DevToolsSession = WebDriver.CreateDevToolsSession();
            Trace.WriteLine($"Navigating to PWA '{pwaName}' at {pwaUri}...");
            WebDriver.Navigate().GoToUrl(pwaUri);
        }

        public ChromeDriver WebDriver { get; }

        public DevToolsSession DevToolsSession { get; }

        public void AddPwaToHomeScreen()
        {
            PwaHomeScreenUtilities.Run(_pwaName, "--add");
            WebDriver.Navigate().Refresh(); // in order to let Chrome realize that it's now in a Home Screen mode
        }

        public void RemoveFromHomeScreen()
        {
            PwaHomeScreenUtilities.Run(_pwaName, "--remove");
        }

        public void Close()
        {
            try
            {
                WebDriver.Close();
            }
            catch (WebDriverException)
            {
            }

            _rootChromeProcessCreatedByChromeDriver.Kill();
        }

        public void Dispose()
        {
            DevToolsSession?.Dispose();
            WebDriver?.Dispose();
        }

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
        private readonly Process _rootChromeProcessCreatedByChromeDriver;

        private static class PwaHomeScreenUtilities
        {
            internal static void Run(string pwaName, string addOrRemove)
            {
                var assemblyDirectory = Directory.GetParent(typeof(Startup).Assembly.Location);
                var targetFramework = assemblyDirectory.Name;
                var pwaUtilitiesAssemblyPath =
                    Path.GetFullPath(
                        Path.Combine(
                            assemblyDirectory.FullName,
                            @$"..\Selenium3Dependencies\{targetFramework}\PwaHomeScreenUtilities.exe"));
                using var pwaUtilitiesProcess = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = pwaUtilitiesAssemblyPath,
                                WindowStyle = ProcessWindowStyle.Normal,
                                RedirectStandardOutput = false,
                                RedirectStandardError = true,
                                UseShellExecute = false
                            }
                        };
                pwaUtilitiesProcess.StartInfo.ArgumentList.Add("--pwa-name");
                pwaUtilitiesProcess.StartInfo.ArgumentList.Add(pwaName);
                pwaUtilitiesProcess.StartInfo.ArgumentList.Add(addOrRemove);

                Trace.WriteLine($"Launching {pwaUtilitiesProcess.StartInfo.FileName} {string.Join(" ", pwaUtilitiesProcess.StartInfo.ArgumentList)}");
                pwaUtilitiesProcess.Start();

                var errors = pwaUtilitiesProcess.StandardError.ReadToEnd();
                pwaUtilitiesProcess.WaitForExit();

                if (pwaUtilitiesProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException($"PwaHomeScreenUtilities.exe failed.{Environment.NewLine}Errors: {errors}");
                }
            }
        }
    }
}