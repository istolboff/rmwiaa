param (
	[string]   $certificateWindowTitle = 'Security Warning',
	[string]   $yesWord = 'Yes',
	[timespan] $timeout = [TimeSpan]::FromSeconds(45))

Set-StrictMode -Version Latest

class AppiumDriver
{
	hidden [OpenQA.Selenium.Appium.Windows.WindowsDriver[OpenQA.Selenium.Appium.Windows.WindowsElement]] $_desktopSession

	AppiumDriver()
	{
		$this._desktopSession = $this.CreateWindowsDriver(@{ app = 'Root' })
		Write-Host "Connected to Root Appium driver successfully."
	}
	
	[OpenQA.Selenium.Appium.Windows.WindowsDriver[OpenQA.Selenium.Appium.Windows.WindowsElement]] TryFindWindowDriver([string] $windowTitle)
	{
		try
		{
			$windowsElement = $this._desktopSession.FindElementByName($windowTitle)
			return ($null -eq $windowsElement) ? $null : $this.CreateWindowsDriver(@{ appTopLevelWindow = [int]::Parse($windowsElement.GetAttribute('NativeWindowHandle')).ToString('X') })
		}
		catch [OpenQA.Selenium.WebDriverException]
		{
			return $null
		}
	}
	
	hidden [OpenQA.Selenium.Appium.Windows.WindowsDriver[OpenQA.Selenium.Appium.Windows.WindowsElement]] CreateWindowsDriver([hashtable] $additionalCapabilities)
	{
		$options = [OpenQA.Selenium.Appium.AppiumOptions]::new()
		(@{ platformName = 'Windows'; deviceName = 'WindowsPC' } + $additionalCapabilities).GetEnumerator() | ForEach-Object { $options.AddAdditionalCapability($_.Name, $_.Value) }
		return [OpenQA.Selenium.Appium.Windows.WindowsDriver[OpenQA.Selenium.Appium.Windows.WindowsElement]]::new([Uri]::new('http://127.0.0.1:4723'), $options)
	}
}

Write-Host "Trying to make localhost SSL Certificate trusted."
Write-Host "Current state:"
dotnet dev-certs https --check --verbose | Out-Host 

$desktopSession = [AppiumDriver]::new()

Write-Host "Starting command `'dotnet dev-certs https --trust --verbose`'..."
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$dotnetProcess = Start-Process -FilePath 'dotnet' -ArgumentList 'dev-certs', 'https', '--trust', '--verbose' -NoNewWindow -PassThru 

[bool] $certificateWindowFound = $false
while (-not $dotnetProcess.HasExited)
{
    if ($stopwatch.Elapsed.Ticks -ge $timeout.Ticks)
    {
		$certificateSetupDriver = $desktopSession.TryFindWindowDriver($certificateWindowTitle) # refresh driver
		if ($null -ne $certificateSetupDriver)
		{
			'PageSource:', $certificateSetupDriver.FindElements([OpenQA.Selenium.By]::XPath("/Window")).WrappedDriver.PageSource,
			'[Security Warning] window ScreenShot:', $certificateSetupDriver.GetScreenshot().AsBase64EncodedString,
			'Whole Desktop Screenshot:', $desktopSession._desktopSession.GetScreenshot().AsBase64EncodedString | `
				Write-Host
		}
		else 
		{
			"[$certificateWindowTitle] system popup did not show up.",
			'Whole Desktop Screenshot:', $desktopSession._desktopSession.GetScreenshot().AsBase64EncodedString | `
				Write-Host
		}

        Stop-Process -Id $dotnetProcess.Id
		Write-Error "Timouted waiting for locating and pressing button [$yesWord] in the [$certificateWindowTitle] system popup. See popup's Window PageSource and Screenshot above."
        return
    }
    
	if (-not $certificateWindowFound)
	{
		$certificateSetupDriver = $desktopSession.TryFindWindowDriver($certificateWindowTitle)
		if ($null -ne $certificateSetupDriver)
		{
			$certificateWindowFound = $true
			Write-Host "Appium Driver was successfullly attached to [$certificateWindowTitle] system popup."
			$yesButton = $certificateSetupDriver.FindElements([OpenQA.Selenium.By]::XPath("/Window/Button")) | Where-Object { $_.Text -eq $yesWord }
			Write-Host "Clicking on [$yesWord] button."
			$yesButton.Click()
		}
	}
	
    Wait-Process -Id $dotnetProcess.Id -Timeout 1 -ErrorAction SilentlyContinue
}

dotnet dev-certs https --check --verbose | Out-Host