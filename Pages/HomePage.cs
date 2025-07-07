using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IhopSeleniumTests.Helpers;
using Serilog;

namespace IhopSeleniumTests.Pages
{
    public class HomePage : BasePage
    {
        // Page URL
        public const string PAGE_URL = "https://www.ihop.com/en";

        // Locators - These are placeholders and should be updated with actual selectors from the website
        private By HeroBannerLocator => By.CssSelector(".hero-banner, .hero, .main-banner");
        private By OrderNowButtonLocator => By.CssSelector("[data-testid='order-now'], .order-now, a[href*='order']");
        private By MenuButtonLocator => By.CssSelector(".menu-button, a[href*='menu'], [data-testid='menu']");
        private By FindLocationButtonLocator => By.CssSelector(".find-location, a[href*='location'], [data-testid='find-location']");
        private By NewsletterEmailInputLocator => By.CssSelector("input[type='email'], .newsletter input, .email-signup input");
        private By NewsletterSubmitButtonLocator => By.CssSelector(".newsletter button, .email-signup button, button[type='submit']");
        private By LogoLocator => By.CssSelector(".logo, .brand-logo, img[alt*='IHOP']");
        
        // Social Media Links
        private By FacebookLinkLocator => By.CssSelector("a[href*='facebook']");
        private By TwitterLinkLocator => By.CssSelector("a[href*='twitter']");
        private By InstagramLinkLocator => By.CssSelector("a[href*='instagram']");
        
        // Menu categories
        private By PancakesMenuLocator => By.CssSelector("a[href*='pancakes'], .menu-item[data-category='pancakes']");
        private By CombosMenuLocator => By.CssSelector("a[href*='combos'], .menu-item[data-category='combos']");
        private By BurgersMenuLocator => By.CssSelector("a[href*='burgers'], .menu-item[data-category='burgers']");

        public HomePage(IWebDriver driver, WebDriverHelper webDriverHelper) : base(driver, webDriverHelper)
        {
        }

        public void NavigateToHomePage()
        {
            Logger.Information($"Navigating to IHOP homepage: {PAGE_URL}");
            Driver.Navigate().GoToUrl(PAGE_URL);
            WaitForPageToLoad();
        }

        public bool IsHeroBannerDisplayed()
        {
            try
            {
                var heroBanner = WebDriverHelper.WaitForElement(HeroBannerLocator, 10);
                bool isDisplayed = heroBanner.Displayed;
                Logger.Information($"Hero banner displayed: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Hero banner not found or not displayed");
                return false;
            }
        }

        public bool IsOrderNowButtonVisible()
        {
            try
            {
                var orderButton = WebDriverHelper.WaitForElement(OrderNowButtonLocator, 10);
                bool isDisplayed = orderButton.Displayed && orderButton.Enabled;
                Logger.Information($"Order Now button visible and clickable: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Order Now button not found or not clickable");
                return false;
            }
        }

        public void ClickOrderNowButton()
        {
            try
            {
                var orderButton = WebDriverHelper.WaitForElementToBeClickable(OrderNowButtonLocator);
                WebDriverHelper.ScrollToElement(orderButton);
                orderButton.Click();
                Logger.Information("Clicked Order Now button");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to click Order Now button");
                throw;
            }
        }

        public bool IsMenuButtonVisible()
        {
            try
            {
                var menuButton = WebDriverHelper.WaitForElement(MenuButtonLocator, 10);
                bool isDisplayed = menuButton.Displayed;
                Logger.Information($"Menu button visible: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Menu button not found");
                return false;
            }
        }

        public void ClickMenuButton()
        {
            try
            {
                var menuButton = WebDriverHelper.WaitForElementToBeClickable(MenuButtonLocator);
                menuButton.Click();
                Logger.Information("Clicked Menu button");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to click Menu button");
                throw;
            }
        }

        public bool IsLogoDisplayed()
        {
            try
            {
                var logo = WebDriverHelper.WaitForElement(LogoLocator, 10);
                bool isDisplayed = logo.Displayed;
                Logger.Information($"Logo displayed: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Logo not found or not displayed");
                return false;
            }
        }

        public void SubscribeToNewsletter(string email)
        {
            try
            {
                var emailInput = WebDriverHelper.WaitForElement(NewsletterEmailInputLocator);
                var submitButton = WebDriverHelper.WaitForElement(NewsletterSubmitButtonLocator);
                
                WebDriverHelper.ScrollToElement(emailInput);
                emailInput.Clear();
                emailInput.SendKeys(email);
                
                submitButton.Click();
                Logger.Information($"Subscribed to newsletter with email: {email}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to subscribe to newsletter with email: {email}");
                throw;
            }
        }

        public bool AreMenuCategoriesDisplayed()
        {
            try
            {
                bool pancakesVisible = WebDriverHelper.IsElementPresent(PancakesMenuLocator);
                bool combosVisible = WebDriverHelper.IsElementPresent(CombosMenuLocator);
                bool burgersVisible = WebDriverHelper.IsElementPresent(BurgersMenuLocator);
                
                bool allVisible = pancakesVisible && combosVisible && burgersVisible;
                Logger.Information($"Menu categories displayed - Pancakes: {pancakesVisible}, Combos: {combosVisible}, Burgers: {burgersVisible}");
                return allVisible;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error checking menu categories visibility");
                return false;
            }
        }

        public void NavigateToMenuCategory(string category)
        {
            try
            {
                By categoryLocator = category.ToLower() switch
                {
                    "pancakes" => PancakesMenuLocator,
                    "combos" => CombosMenuLocator,
                    "burgers" => BurgersMenuLocator,
                    _ => throw new ArgumentException($"Unknown menu category: {category}")
                };

                var categoryElement = WebDriverHelper.WaitForElementToBeClickable(categoryLocator);
                WebDriverHelper.ScrollToElement(categoryElement);
                categoryElement.Click();
                Logger.Information($"Navigated to {category} menu category");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to navigate to {category} menu category");
                throw;
            }
        }

        public void ClickSocialMediaLink(string platform)
        {
            try
            {
                By socialLinkLocator = platform.ToLower() switch
                {
                    "facebook" => FacebookLinkLocator,
                    "twitter" => TwitterLinkLocator,
                    "instagram" => InstagramLinkLocator,
                    _ => throw new ArgumentException($"Unknown social media platform: {platform}")
                };

                var socialLink = WebDriverHelper.WaitForElementToBeClickable(socialLinkLocator);
                WebDriverHelper.ScrollToElement(socialLink);
                
                // Store current window handle
                string originalWindow = Driver.CurrentWindowHandle;
                
                socialLink.Click();
                Logger.Information($"Clicked {platform} social media link");
                
                // Wait for new tab to open and switch back to original
                Thread.Sleep(2000);
                Driver.SwitchTo().Window(originalWindow);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to click {platform} social media link");
                throw;
            }
        }

        public bool ValidateExternalLinkOpensInNewTab(string platform)
        {
            try
            {
                int originalTabCount = Driver.WindowHandles.Count;
                ClickSocialMediaLink(platform);
                
                // Wait for new tab to potentially open
                Thread.Sleep(3000);
                
                int newTabCount = Driver.WindowHandles.Count;
                bool openedNewTab = newTabCount > originalTabCount;
                
                Logger.Information($"{platform} link opened new tab: {openedNewTab}");
                return openedNewTab;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error validating {platform} link behavior");
                return false;
            }
        }
    }
}
