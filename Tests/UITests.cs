using NUnit.Framework;
using System;
using System.Threading;

namespace IhopSeleniumTests.Tests
{
    [TestFixture]
    [Category("UI")]
    public class UITests : BaseTest
    {
        [Test]
        [Description("Verify that the homepage loads successfully with all major elements")]
        public void VerifyHomepageLoadsWithMajorElements()
        {
            // Arrange & Act
            WaitAndNavigateToHomePage();

            // Assert
            AssertElementIsDisplayed(() => HomePage.IsHeaderPresent(), "Header");
            AssertElementIsDisplayed(() => HomePage.IsFooterPresent(), "Footer");
            AssertElementIsDisplayed(() => HomePage.IsNavigationPresent(), "Navigation");
            AssertElementIsDisplayed(() => HomePage.IsHeroBannerDisplayed(), "Hero Banner");
            AssertElementIsDisplayed(() => HomePage.IsLogoDisplayed(), "IHOP Logo");

            // Verify page title
            string pageTitle = HomePage.GetPageTitle();
            Assert.IsTrue(pageTitle.Contains("IHOP") || pageTitle.Contains("International House"), 
                $"Page title should contain IHOP or International House. Actual: {pageTitle}");
            
            Logger.Information("✓ Homepage loaded successfully with all major elements");
        }

        [Test]
        [Description("Validate that Order Now button is visible and clickable")]
        public void ValidateOrderNowButtonVisibilityAndClickability()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act & Assert
            AssertElementIsDisplayed(() => HomePage.IsOrderNowButtonVisible(), "Order Now button");
            
            // Test clickability - this should navigate to ordering page
            string originalUrl = Driver.Url;
            AssertElementIsClickable(() => HomePage.ClickOrderNowButton(), "Order Now button");
            
            // Wait for navigation
            Thread.Sleep(3000);
            
            // Verify navigation occurred
            string newUrl = Driver.Url;
            Assert.AreNotEqual(originalUrl, newUrl, "Order Now button should navigate to a different page");
            
            Logger.Information($"✓ Order Now button navigated from {originalUrl} to {newUrl}");
        }

        [Test]
        [Description("Check all menu items and categories under Menu are displayed")]
        public void ValidateMenuCategoriesAreDisplayed()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act - Navigate to menu
            AssertElementIsClickable(() => HomePage.ClickMenuButton(), "Menu button");
            Thread.Sleep(2000);

            // Assert
            AssertElementIsDisplayed(() => MenuPage.AreMenuCategoriesDisplayed(), "Menu categories");
            
            // Validate specific menu categories
            bool allCategoriesValid = MenuPage.ValidateAllMenuCategories();
            Assert.IsTrue(allCategoriesValid, "All menu categories should be valid and contain items");
            
            Logger.Information("✓ All menu categories are displayed and accessible");
        }

        [Test]
        [Description("Test responsiveness of the site by resizing browser")]
        public void TestSiteResponsiveness()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act & Assert
            ValidateResponsiveDesign();
            
            Logger.Information("✓ Site responsiveness test completed successfully");
        }

        [Test]
        [Description("Validate navigation menu items are visible and accessible")]
        public void ValidateNavigationMenuItems()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act & Assert
            AssertElementIsDisplayed(() => HomePage.IsNavigationPresent(), "Navigation menu");
            AssertElementIsDisplayed(() => HomePage.IsMenuButtonVisible(), "Menu button in navigation");
            
            // Test that menu categories are accessible
            AssertElementIsDisplayed(() => HomePage.AreMenuCategoriesDisplayed(), "Menu categories in navigation");
            
            Logger.Information("✓ Navigation menu items are visible and accessible");
        }

        [Test]
        [Description("Verify footer elements and links are present")]
        public void ValidateFooterElements()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act - Scroll to footer
            WebDriverHelper.ScrollToElement(Driver.FindElement(HomePage.FooterLocator));
            Thread.Sleep(1000);

            // Assert
            AssertElementIsDisplayed(() => HomePage.IsFooterPresent(), "Footer");
            
            // Check for social media links
            bool facebookLinkOpensNewTab = HomePage.ValidateExternalLinkOpensInNewTab("facebook");
            bool twitterLinkOpensNewTab = HomePage.ValidateExternalLinkOpensInNewTab("twitter");
            bool instagramLinkOpensNewTab = HomePage.ValidateExternalLinkOpensInNewTab("instagram");
            
            // At least one social media link should work
            bool anySocialMediaWorking = facebookLinkOpensNewTab || twitterLinkOpensNewTab || instagramLinkOpensNewTab;
            Assert.IsTrue(anySocialMediaWorking, "At least one social media link should open in a new tab");
            
            Logger.Information("✓ Footer elements validated successfully");
        }

        [Test]
        [Description("Validate page load performance and timing")]
        public void ValidatePageLoadPerformance()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            WaitAndNavigateToHomePage();
            
            // Stop timing when page is fully loaded
            HomePage.WaitForPageToLoad();
            stopwatch.Stop();

            // Assert
            long loadTimeMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(loadTimeMs, 15000, $"Page should load within 15 seconds. Actual: {loadTimeMs}ms");
            
            Logger.Information($"✓ Page loaded in {loadTimeMs}ms (under 15 second threshold)");
        }

        [Test]
        [Description("Verify hero banner content and visibility")]
        public void ValidateHeroBannerContent()
        {
            // Arrange
            WaitAndNavigateToHomePage();

            // Act & Assert
            AssertElementIsDisplayed(() => HomePage.IsHeroBannerDisplayed(), "Hero banner");
            
            // Verify Order Now button is prominently displayed in hero section
            AssertElementIsDisplayed(() => HomePage.IsOrderNowButtonVisible(), "Order Now button in hero section");
            
            Logger.Information("✓ Hero banner content validated successfully");
        }

        [Test]
        [Description("Test browser back and forward navigation")]
        public void TestBrowserNavigation()
        {
            // Arrange
            WaitAndNavigateToHomePage();
            string homeUrl = Driver.Url;

            // Act - Navigate to menu
            HomePage.ClickMenuButton();
            Thread.Sleep(2000);
            string menuUrl = Driver.Url;

            // Test back navigation
            Driver.Navigate().Back();
            Thread.Sleep(2000);
            string backUrl = Driver.Url;

            // Test forward navigation
            Driver.Navigate().Forward();
            Thread.Sleep(2000);
            string forwardUrl = Driver.Url;

            // Assert
            Assert.AreEqual(homeUrl, backUrl, "Back navigation should return to homepage");
            Assert.AreEqual(menuUrl, forwardUrl, "Forward navigation should return to menu page");
            
            Logger.Information("✓ Browser navigation (back/forward) works correctly");
        }
    }
}
