using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IhopSeleniumTests.Helpers;
using Serilog;

namespace IhopSeleniumTests.Pages
{
    public class MenuPage : BasePage
    {
        // Locators - These are placeholders and should be updated with actual selectors
        private By MenuCategoriesLocator => By.CssSelector(".menu-categories, .category-nav, .menu-nav");
        private By PancakesCategoryLocator => By.CssSelector("a[href*='pancakes'], .category[data-category='pancakes']");
        private By CombosCategoryLocator => By.CssSelector("a[href*='combos'], .category[data-category='combos']");
        private By BurgersCategoryLocator => By.CssSelector("a[href*='burgers'], .category[data-category='burgers']");
        private By DrinksCategoryLocator => By.CssSelector("a[href*='drinks'], .category[data-category='drinks']");
        private By MenuItemsLocator => By.CssSelector(".menu-item, .product-item, .dish");
        private By MenuItemNameLocator => By.CssSelector(".item-name, .product-name, h3");
        private By MenuItemPriceLocator => By.CssSelector(".item-price, .product-price, .price");
        private By MenuItemImageLocator => By.CssSelector(".item-image img, .product-image img");
        private By AddToCartButtonLocator => By.CssSelector(".add-to-cart, .order-button, button[data-action='add']");
        private By SearchBoxLocator => By.CssSelector(".menu-search, input[placeholder*='search menu']");
        private By FilterButtonsLocator => By.CssSelector(".filter-buttons, .dietary-filters");
        private By VegetarianFilterLocator => By.CssSelector(".filter-vegetarian, [data-filter='vegetarian']");
        private By GlutenFreeFilterLocator => By.CssSelector(".filter-gluten-free, [data-filter='gluten-free']");

        public MenuPage(IWebDriver driver, WebDriverHelper webDriverHelper) : base(driver, webDriverHelper)
        {
        }

        public bool AreMenuCategoriesDisplayed()
        {
            try
            {
                var categories = WebDriverHelper.WaitForElement(MenuCategoriesLocator, 10);
                bool isDisplayed = categories.Displayed;
                Logger.Information($"Menu categories displayed: {isDisplayed}");
                return isDisplayed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Menu categories not found or not displayed");
                return false;
            }
        }

        public void NavigateToCategory(string categoryName)
        {
            try
            {
                By categoryLocator = categoryName.ToLower() switch
                {
                    "pancakes" => PancakesCategoryLocator,
                    "combos" => CombosCategoryLocator,
                    "burgers" => BurgersCategoryLocator,
                    "drinks" => DrinksCategoryLocator,
                    _ => throw new ArgumentException($"Unknown category: {categoryName}")
                };

                var categoryElement = WebDriverHelper.WaitForElementToBeClickable(categoryLocator);
                WebDriverHelper.ScrollToElement(categoryElement);
                categoryElement.Click();
                
                WaitForPageToLoad();
                Logger.Information($"Navigated to {categoryName} category");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to navigate to {categoryName} category");
                throw;
            }
        }

        public int GetMenuItemsCount()
        {
            try
            {
                var menuItems = Driver.FindElements(MenuItemsLocator);
                int count = menuItems.Count;
                Logger.Information($"Found {count} menu items");
                return count;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error counting menu items");
                return 0;
            }
        }

        public List<string> GetMenuItemNames()
        {
            try
            {
                var menuItems = Driver.FindElements(MenuItemsLocator);
                var itemNames = new List<string>();

                foreach (var item in menuItems)
                {
                    try
                    {
                        var nameElement = item.FindElement(MenuItemNameLocator);
                        if (!string.IsNullOrEmpty(nameElement.Text))
                        {
                            itemNames.Add(nameElement.Text);
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        // Skip items without name elements
                        continue;
                    }
                }

                Logger.Information($"Retrieved {itemNames.Count} menu item names");
                return itemNames;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error retrieving menu item names");
                return new List<string>();
            }
        }

        public bool AreMenuItemImagesLoaded()
        {
            try
            {
                var images = Driver.FindElements(MenuItemImageLocator);
                int loadedImages = 0;

                foreach (var image in images.Take(5)) // Check first 5 images
                {
                    try
                    {
                        WebDriverHelper.ScrollToElement(image);
                        Thread.Sleep(500); // Wait for image to load

                        var naturalWidth = ((IJavaScriptExecutor)Driver)
                            .ExecuteScript("return arguments[0].naturalWidth;", image);
                        
                        if (Convert.ToInt32(naturalWidth) > 0)
                        {
                            loadedImages++;
                        }
                    }
                    catch (Exception)
                    {
                        // Skip this image
                        continue;
                    }
                }

                bool allLoaded = loadedImages == Math.Min(images.Count, 5);
                Logger.Information($"Menu item images loaded: {loadedImages}/{Math.Min(images.Count, 5)}");
                return allLoaded;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error checking menu item images");
                return false;
            }
        }

        public void SearchMenuItem(string searchTerm)
        {
            try
            {
                var searchBox = WebDriverHelper.WaitForElement(SearchBoxLocator);
                WebDriverHelper.ScrollToElement(searchBox);
                
                searchBox.Clear();
                searchBox.SendKeys(searchTerm);
                searchBox.SendKeys(Keys.Enter);
                
                WaitForPageToLoad();
                Logger.Information($"Searched for menu item: {searchTerm}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to search for menu item: {searchTerm}");
                throw;
            }
        }

        public void ApplyDietaryFilter(string filterType)
        {
            try
            {
                By filterLocator = filterType.ToLower() switch
                {
                    "vegetarian" => VegetarianFilterLocator,
                    "gluten-free" => GlutenFreeFilterLocator,
                    _ => throw new ArgumentException($"Unknown filter type: {filterType}")
                };

                var filterElement = WebDriverHelper.WaitForElementToBeClickable(filterLocator);
                WebDriverHelper.ScrollToElement(filterElement);
                filterElement.Click();
                
                WaitForPageToLoad();
                Logger.Information($"Applied {filterType} filter");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to apply {filterType} filter");
                throw;
            }
        }

        public void AddFirstItemToCart()
        {
            try
            {
                var firstItem = Driver.FindElements(MenuItemsLocator).FirstOrDefault();
                if (firstItem == null)
                {
                    throw new Exception("No menu items found");
                }

                var addToCartButton = firstItem.FindElement(AddToCartButtonLocator);
                WebDriverHelper.ScrollToElement(addToCartButton);
                addToCartButton.Click();
                
                Logger.Information("Added first menu item to cart");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to add first item to cart");
                throw;
            }
        }

        public bool ValidateMenuItemDetails(int itemIndex = 0)
        {
            try
            {
                var menuItems = Driver.FindElements(MenuItemsLocator);
                if (itemIndex >= menuItems.Count)
                {
                    Logger.Warning($"Item index {itemIndex} is out of range. Total items: {menuItems.Count}");
                    return false;
                }

                var item = menuItems[itemIndex];
                WebDriverHelper.ScrollToElement(item);

                // Check if item has name
                bool hasName = false;
                try
                {
                    var nameElement = item.FindElement(MenuItemNameLocator);
                    hasName = !string.IsNullOrEmpty(nameElement.Text);
                }
                catch (NoSuchElementException) { }

                // Check if item has price
                bool hasPrice = false;
                try
                {
                    var priceElement = item.FindElement(MenuItemPriceLocator);
                    hasPrice = !string.IsNullOrEmpty(priceElement.Text);
                }
                catch (NoSuchElementException) { }

                // Check if item has image
                bool hasImage = false;
                try
                {
                    var imageElement = item.FindElement(MenuItemImageLocator);
                    hasImage = imageElement.Displayed;
                }
                catch (NoSuchElementException) { }

                bool isValid = hasName && hasPrice;
                Logger.Information($"Menu item {itemIndex} validation - Name: {hasName}, Price: {hasPrice}, Image: {hasImage}");
                
                return isValid;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error validating menu item details for item {itemIndex}");
                return false;
            }
        }

        public bool ValidateAllMenuCategories()
        {
            try
            {
                string[] categories = { "pancakes", "combos", "burgers" };
                bool allCategoriesValid = true;

                foreach (string category in categories)
                {
                    try
                    {
                        NavigateToCategory(category);
                        Thread.Sleep(2000); // Wait for category to load
                        
                        int itemCount = GetMenuItemsCount();
                        bool hasItems = itemCount > 0;
                        
                        Logger.Information($"Category '{category}' has {itemCount} items");
                        
                        if (!hasItems)
                        {
                            allCategoriesValid = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Error validating category: {category}");
                        allCategoriesValid = false;
                    }
                }

                return allCategoriesValid;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error validating all menu categories");
                return false;
            }
        }
    }
}
