using NUnit.Framework;
using OpenQA.Selenium;
using IhopSeleniumTests.Helpers;
using IhopSeleniumTests.Pages;
using Serilog;
using System;
using System.Threading;

namespace IhopSeleniumTests.Tests
{
    [TestFixture]
    public class BaseTest
    {
        protected IWebDriver? Driver;
        protected WebDriverHelper? WebDriverHelper;
        protected HomePage? HomePage;
        protected LocationPage? LocationPage;
        protected MenuPage? MenuPage;
        protected AccessibilityHelper? AccessibilityHelper;
        protected ILogger? Logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/test-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Logger = Log.ForContext<BaseTest>();
            Logger.Information("Test suite starting...");
        }

        [SetUp]
        public void SetUp()
        {
            try
            {
                // Initialize WebDriver and helper
                WebDriverHelper = new WebDriverHelper();
                Driver = WebDriverHelper.InitializeDriver();

                // Initialize page objects
                HomePage = new HomePage(Driver, WebDriverHelper);
                LocationPage = new LocationPage(Driver, WebDriverHelper);
                MenuPage = new MenuPage(Driver, WebDriverHelper);
                AccessibilityHelper = new AccessibilityHelper(Driver);

                Logger?.Information($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, "Failed to set up test");
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                // Take screenshot on failure
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    WebDriverHelper?.TakeScreenshot($"FAILED_{TestContext.CurrentContext.Test.Name}");
                }

                Logger?.Information($"Test completed: {TestContext.CurrentContext.Test.Name} - " +
                                 $"Result: {TestContext.CurrentContext.Result.Outcome.Status}");
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, "Error during test teardown");
            }
            finally
            {
                // Clean up WebDriver
                WebDriverHelper?.QuitDriver();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Logger?.Information("Test suite completed");
            Log.CloseAndFlush();
        }

        // Helper methods for common assertions
        protected void AssertElementIsDisplayed(Func<bool> elementCheck, string elementDescription)
        {
            bool isDisplayed = elementCheck();
            Assert.IsTrue(isDisplayed, $"{elementDescription} should be displayed");
            Logger?.Information($"✓ {elementDescription} is displayed");
        }

        protected void AssertElementIsClickable(Action clickAction, string elementDescription)
        {
            try
            {
                clickAction();
                Logger?.Information($"✓ {elementDescription} is clickable");
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, $"✗ {elementDescription} is not clickable");
                Assert.Fail($"{elementDescription} should be clickable");
            }
        }

        protected void AssertPageLoaded(string expectedUrl, string pageName)
        {
            string currentUrl = Driver?.Url ?? "";
            Assert.IsTrue(currentUrl.Contains(expectedUrl), 
                $"{pageName} should be loaded. Expected URL to contain: {expectedUrl}, Actual: {currentUrl}");
            Logger?.Information($"✓ {pageName} loaded successfully");
        }

        protected void AssertElementCount(Func<int> countFunction, int expectedMinCount, string elementDescription)
        {
            int actualCount = countFunction();
            Assert.GreaterOrEqual(actualCount, expectedMinCount, 
                $"{elementDescription} count should be at least {expectedMinCount}, but was {actualCount}");
            Logger?.Information($"✓ {elementDescription} count: {actualCount} (expected min: {expectedMinCount})");
        }

        protected void WaitAndNavigateToHomePage()
        {
            HomePage?.NavigateToHomePage();
            Thread.Sleep(2000); // Allow page to fully load
        }

        protected void ValidateResponsiveDesign()
        {
            // Test different screen sizes
            int[] widths = { 1920, 1024, 768, 375 }; // Desktop, tablet, mobile
            
            foreach (int width in widths)
            {
                WebDriverHelper?.ResizeBrowser(width, 800);
                Thread.Sleep(1000);
                
                // Check if page elements are still accessible
                bool headerVisible = HomePage?.IsHeaderPresent() ?? false;
                bool footerVisible = HomePage?.IsFooterPresent() ?? false;
                
                Logger?.Information($"Responsive test at {width}px - Header: {headerVisible}, Footer: {footerVisible}");
                
                // At minimum, header should be visible at all sizes
                Assert.IsTrue(headerVisible, $"Header should be visible at {width}px width");
            }
            
            // Reset to default size
            WebDriverHelper?.ResizeBrowser(1920, 1080);
        }
    }
}
