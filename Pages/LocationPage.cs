using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IhopSeleniumTests.Helpers;
using Serilog;

namespace IhopSeleniumTests.Pages
{
    public class LocationPage : BasePage
    {
        // Locators - These are placeholders and should be updated with actual selectors
        private By ZipCodeInputLocator => By.CssSelector("input[placeholder*='zip'], input[name*='zip'], .zip-input");
        private By SearchButtonLocator => By.CssSelector(".search-button, button[type='submit'], .find-location-btn");
        private By LocationResultsLocator => By.CssSelector(".location-results, .restaurant-list, .locations");
        private By FirstLocationLocator => By.CssSelector(".location-item:first-child, .restaurant:first-child");
        private By LocationNameLocator => By.CssSelector(".location-name, .restaurant-name, h3");
        private By LocationAddressLocator => By.CssSelector(".location-address, .restaurant-address, .address");
        private By GetDirectionsButtonLocator => By.CssSelector(".directions-btn, a[href*='directions']");
        private By ErrorMessageLocator => By.CssSelector(".error-message, .alert-error, .validation-error");
        private By LoadingIndicatorLocator => By.CssSelector(".loading, .spinner, .searching");

        public LocationPage(IWebDriver driver, WebDriverHelper webDriverHelper) : base(driver, webDriverHelper)
        {
        }

        public void SearchByZipCode(string zipCode)
        {
            try
            {
                var zipInput = WebDriverHelper.WaitForElement(ZipCodeInputLocator);
                var searchButton = WebDriverHelper.WaitForElement(SearchButtonLocator);

                WebDriverHelper.ScrollToElement(zipInput);
                zipInput.Clear();
                zipInput.SendKeys(zipCode);

                searchButton.Click();
                Logger.Information($"Searched for locations with ZIP code: {zipCode}");

                // Wait for loading to complete
                WaitForSearchToComplete();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to search by ZIP code: {zipCode}");
                throw;
            }
        }

        private void WaitForSearchToComplete()
        {
            try
            {
                // Wait for loading indicator to appear and then disappear
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => WebDriverHelper.IsElementPresent(LoadingIndicatorLocator));
                
                wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                wait.Until(driver => !WebDriverHelper.IsElementPresent(LoadingIndicatorLocator));
            }
            catch (WebDriverTimeoutException)
            {
                // Loading indicator might not be present or might disappear quickly
                Logger.Information("Loading indicator not detected or completed quickly");
            }
        }

        public bool AreLocationResultsDisplayed()
        {
            try
            {
                var results = WebDriverHelper.WaitForElement(LocationResultsLocator, 15);
                bool isDisplayed = results.Displayed;
                Logger.Information($"Location results displayed: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Location results not found or not displayed");
                return false;
            }
        }

        public int GetLocationResultsCount()
        {
            try
            {
                var locationItems = Driver.FindElements(By.CssSelector(".location-item, .restaurant"));
                int count = locationItems.Count;
                Logger.Information($"Found {count} location results");
                return count;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error counting location results");
                return 0;
            }
        }

        public string GetFirstLocationName()
        {
            try
            {
                var firstLocation = WebDriverHelper.WaitForElement(FirstLocationLocator);
                var locationName = firstLocation.FindElement(LocationNameLocator);
                string name = locationName.Text;
                Logger.Information($"First location name: {name}");
                return name;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to get first location name");
                return string.Empty;
            }
        }

        public string GetFirstLocationAddress()
        {
            try
            {
                var firstLocation = WebDriverHelper.WaitForElement(FirstLocationLocator);
                var locationAddress = firstLocation.FindElement(LocationAddressLocator);
                string address = locationAddress.Text;
                Logger.Information($"First location address: {address}");
                return address;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to get first location address");
                return string.Empty;
            }
        }

        public void ClickGetDirections()
        {
            try
            {
                var directionsButton = WebDriverHelper.WaitForElementToBeClickable(GetDirectionsButtonLocator);
                WebDriverHelper.ScrollToElement(directionsButton);
                directionsButton.Click();
                Logger.Information("Clicked Get Directions button");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to click Get Directions button");
                throw;
            }
        }

        public bool IsErrorMessageDisplayed()
        {
            try
            {
                bool hasError = WebDriverHelper.IsElementPresent(ErrorMessageLocator);
                Logger.Information($"Error message displayed: {hasError}");
                return hasError;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error checking for error message");
                return false;
            }
        }

        public string GetErrorMessage()
        {
            try
            {
                if (IsErrorMessageDisplayed())
                {
                    var errorElement = Driver.FindElement(ErrorMessageLocator);
                    string errorText = errorElement.Text;
                    Logger.Information($"Error message: {errorText}");
                    return errorText;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to get error message text");
                return string.Empty;
            }
        }

        public bool ValidateZipCodeSearch(string zipCode)
        {
            try
            {
                SearchByZipCode(zipCode);
                
                // Check if results are displayed or if there's an error
                Thread.Sleep(3000); // Allow time for results to load
                
                bool hasResults = AreLocationResultsDisplayed();
                bool hasError = IsErrorMessageDisplayed();
                
                Logger.Information($"ZIP code {zipCode} search validation - Results: {hasResults}, Error: {hasError}");
                
                return hasResults && !hasError;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error validating ZIP code search for: {zipCode}");
                return false;
            }
        }
    }
}
