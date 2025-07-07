using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IhopSeleniumTests.Helpers;
using Serilog;

namespace IhopSeleniumTests.Pages
{
    public class BasePage
    {
        protected IWebDriver Driver;
        protected WebDriverHelper WebDriverHelper;
        protected readonly ILogger Logger;

        public BasePage(IWebDriver driver, WebDriverHelper webDriverHelper)
        {
            Driver = driver;
            WebDriverHelper = webDriverHelper;
            Logger = Log.ForContext(GetType());
        }

        // Common page elements
        protected By HeaderLocator => By.CssSelector("header");
        public By FooterLocator => By.CssSelector("footer");
        protected By NavigationLocator => By.CssSelector("nav, .navigation, .navbar");
        protected By LoadingSpinnerLocator => By.CssSelector(".loading, .spinner, [data-loading]");

        public virtual void WaitForPageToLoad()
        {
            WebDriverHelper.WaitForPageLoad();
            WaitForLoadingToComplete();
            Logger.Information($"Page loaded: {GetType().Name}");
        }

        protected void WaitForLoadingToComplete()
        {
            try
            {
                // Wait for any loading spinners to disappear
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => !WebDriverHelper.IsElementPresent(LoadingSpinnerLocator));
            }
            catch (WebDriverTimeoutException)
            {
                // Loading spinner might not be present, continue
            }
        }

        public bool IsHeaderPresent()
        {
            return WebDriverHelper.IsElementPresent(HeaderLocator);
        }

        public bool IsFooterPresent()
        {
            return WebDriverHelper.IsElementPresent(FooterLocator);
        }

        public bool IsNavigationPresent()
        {
            return WebDriverHelper.IsElementPresent(NavigationLocator);
        }

        public string GetPageTitle()
        {
            return Driver.Title;
        }

        public string GetCurrentUrl()
        {
            return Driver.Url;
        }
    }
}
