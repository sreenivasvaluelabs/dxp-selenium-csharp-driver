using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

namespace IhopSeleniumTests.Helpers
{
    public class WebDriverHelper
    {
        private IWebDriver? _driver;
        private WebDriverWait? _wait;
        private readonly ILogger _logger;

        public WebDriverHelper()
        {
            _logger = Log.ForContext<WebDriverHelper>();
        }

        public IWebDriver InitializeDriver()
        {
            try
            {
                // Use Edge as primary driver (Chrome has version compatibility issues)
                _logger.Information("Initializing Microsoft Edge WebDriver...");
                return InitializeEdgeDriver();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize Edge WebDriver: {Message}", ex.Message);
                throw new InvalidOperationException($"Failed to initialize Edge driver. Error: {ex.Message}");
            }
        }

        private IWebDriver InitializeEdgeDriver()
        {
            try
            {
                var options = new EdgeOptions();
                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-extensions");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                
                _driver = new EdgeDriver(options);
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
                
                _logger.Information("Edge WebDriver initialized successfully");
                return _driver;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize Edge WebDriver");
                throw;
            }
        }

        public void QuitDriver()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
                _logger.Information("WebDriver quit successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while quitting WebDriver");
            }
        }

        public IWebElement WaitForElement(By locator, int timeoutSeconds = 30)
        {
            try
            {
                if (_driver == null) throw new InvalidOperationException("Driver not initialized");
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver => driver.FindElement(locator));
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.Error($"Element not found within {timeoutSeconds} seconds: {locator}");
                throw new Exception($"Element not found: {locator}", ex);
            }
        }

        public IWebElement WaitForElementToBeClickable(By locator, int timeoutSeconds = 30)
        {
            try
            {
                if (_driver == null) throw new InvalidOperationException("Driver not initialized");
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver => 
                {
                    try
                    {
                        var element = driver.FindElement(locator);
                        return element.Enabled && element.Displayed ? element : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                })!;
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.Error($"Element not clickable within {timeoutSeconds} seconds: {locator}");
                throw new Exception($"Element not clickable: {locator}", ex);
            }
        }

        public void WaitForPageLoad()
        {
            if (_driver == null) return;
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void ScrollToElement(IWebElement element)
        {
            if (_driver == null) return;
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(500); // Small delay to ensure scroll completion
        }

        public void TakeScreenshot(string fileName)
        {
            try
            {
                if (_driver == null) return;
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var filePath = Path.Combine(directory, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                screenshot.SaveAsFile(filePath);
                
                _logger.Information($"Screenshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to take screenshot");
            }
        }

        public void ResizeBrowser(int width, int height)
        {
            if (_driver == null) return;
            _driver.Manage().Window.Size = new System.Drawing.Size(width, height);
            Thread.Sleep(1000); // Wait for resize to complete
        }

        public bool IsElementPresent(By locator)
        {
            try
            {
                if (_driver == null) return false;
                _driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SwitchToTab(int tabIndex)
        {
            if (_driver == null) return;
            var tabs = _driver.WindowHandles;
            if (tabIndex < tabs.Count)
            {
                _driver.SwitchTo().Window(tabs[tabIndex]);
            }
        }

        public void SwitchToFrame(By frameLocator)
        {
            if (_driver == null) return;
            var frame = WaitForElement(frameLocator);
            _driver.SwitchTo().Frame(frame);
        }

        public void SwitchToDefaultContent()
        {
            if (_driver == null) return;
            _driver.SwitchTo().DefaultContent();
        }
    }
}
