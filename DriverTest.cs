using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;

namespace IhopSeleniumTests
{
    public class DriverTest
    {
        public static void TestDrivers()
        {
            Console.WriteLine("Testing available drivers...");
            
            // Test Firefox
            try
            {
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArgument("--headless");
                using var firefoxDriver = new FirefoxDriver(firefoxOptions);
                Console.WriteLine("✅ Firefox driver works!");
                firefoxDriver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Firefox driver failed: {ex.Message}");
            }
            
            // Test Chrome
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--headless");
                using var chromeDriver = new ChromeDriver(chromeOptions);
                Console.WriteLine("✅ Chrome driver works!");
                chromeDriver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chrome driver failed: {ex.Message}");
            }
        }
    }
}
