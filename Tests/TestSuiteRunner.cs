using NUnit.Framework;
using OpenQA.Selenium;
using IhopSeleniumTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IhopSeleniumTests.Tests
{
    /// <summary>
    /// Test suite runner and organization class
    /// </summary>
    [TestFixture]
    public class TestSuiteRunner
    {
        [Test, Category("Smoke")]
        [Description("Quick smoke test to verify basic functionality")]
        public void SmokeTest()
        {
            // This test runs a simple smoke test to verify that Edge driver initializes correctly
            var webDriverHelper = new WebDriverHelper();
            IWebDriver? driver = null;

            try
            {
                // Test that we can initialize Edge driver
                driver = webDriverHelper.InitializeDriver();
                Assert.IsNotNull(driver, "Driver should be initialized");
                
                // Test that we can navigate to IHOP
                driver!.Navigate().GoToUrl("https://www.ihop.com/en");
                
                // Wait for page load
                System.Threading.Thread.Sleep(3000);
                
                // Basic check that page loaded
                Assert.IsTrue(driver.Url.Contains("ihop.com"), "Should navigate to IHOP website");
                Assert.IsNotNull(driver.Title, "Page should have a title");
                
                // Test passed successfully
            }
            catch (Exception ex)
            {
                Assert.Fail($"Smoke test failed: {ex.Message}");
            }
            finally
            {
                webDriverHelper.QuitDriver();
            }
        }
    }

    /// <summary>
    /// Custom test attributes for better test organization
    /// </summary>
    public static class TestCategories
    {
        public const string UI = "UI";
        public const string Functional = "Functional";
        public const string Integration = "Integration";
        public const string Accessibility = "Accessibility";
        public const string Smoke = "Smoke";
        public const string Regression = "Regression";
        public const string Critical = "Critical";
    }

    /// <summary>
    /// Test data provider for parameterized tests
    /// </summary>
    public static class TestDataProvider
    {
        public static IEnumerable<string> ValidZipCodes()
        {
            yield return "90210"; // Beverly Hills, CA
            yield return "10001"; // New York, NY
            yield return "60601"; // Chicago, IL
        }

        public static IEnumerable<string> InvalidZipCodes()
        {
            yield return "00000"; // Invalid
            yield return "99999"; // Invalid
            yield return "ABCDE"; // Invalid format
        }

        public static IEnumerable<string> MenuCategories()
        {
            yield return "pancakes";
            yield return "combos";
            yield return "burgers";
        }

        public static IEnumerable<string> SocialMediaPlatforms()
        {
            yield return "facebook";
            yield return "twitter";
            yield return "instagram";
        }
    }

    /// <summary>
    /// Test execution order attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestOrderAttribute : Attribute
    {
        public int Order { get; }

        public TestOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
