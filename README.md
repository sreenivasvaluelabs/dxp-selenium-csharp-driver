# Website Selenium C# Test Suite

This is a comprehensive end-to-end automated test suite for the  website  built using Selenium WebDriver with C# and NUnit framework.

## 🚀 Features

- **Comprehensive Test Coverage**: UI, Functional, Integration, and Accessibility tests
- **Page Object Model (POM)**: Clean, maintainable test structure
- **Cross-browser Support**: Chrome WebDriver with easy extension to other browsers
- **Accessibility Testing**: Integration with Axe-Core for WCAG compliance
- **Detailed Logging**: Serilog integration for comprehensive test logging
- **Screenshot Capture**: Automatic screenshots on test failures
- **Responsive Testing**: Browser resizing to test different screen sizes
- **Error Handling**: Robust exception handling and recovery mechanisms

## 📁 Project Structure

```
SeleniumTests/
├── Helpers/
│   ├── WebDriverHelper.cs          # WebDriver management and utilities
│   └── AccessibilityHelper.cs      # Accessibility testing utilities
├── Pages/
│   ├── BasePage.cs                 # Base page object class
│   ├── HomePage.cs                 # Homepage page object
│   ├── LocationPage.cs             # Location finder page object
│   └── MenuPage.cs                 # Menu page object
├── Tests/
│   ├── BaseTest.cs                 # Base test class with setup/teardown
│   ├── UITests.cs                  # UI test cases
│   ├── FunctionalTests.cs          # Functional test cases
│   ├── IntegrationTests.cs         # Integration test cases
│   ├── AccessibilityTests.cs       # Accessibility test cases
│   └── TestSuiteRunner.cs          # Test suite organization
├── appsettings.json                # Configuration settings
├── SeleniumTests.csproj        # Project file
└── README.md                       # This file
```

## 🛠️ Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or Visual Studio Code
- Google Chrome browser
- Internet connection for WebDriverManager to download ChromeDriver

## 📦 Dependencies

- **Selenium.WebDriver** (4.15.0) - WebDriver implementation
- **Selenium.WebDriver.ChromeDriver** (118.0.5993.7000) - Chrome driver
- **WebDriverManager** (2.16.2) - Automatic driver management
- **NUnit** (3.13.3) - Testing framework
- **Serilog** (3.0.1) - Logging framework
- **Selenium.Axe** (4.3.0) - Accessibility testing

## 🚀 Getting Started

### 1. Clone and Setup

```bash
cd path/to/project
dotnet restore
```

### 2. Build the Project

```bash
dotnet build
```

### 3. Run All Tests

```bash
dotnet test
```

### 4. Run Specific Test Categories

```bash
# Run only UI tests
dotnet test --filter Category=UI

# Run only Functional tests
dotnet test --filter Category=Functional

# Run only Integration tests
dotnet test --filter Category=Integration

# Run only Accessibility tests
dotnet test --filter Category=Accessibility

# Run Smoke tests
dotnet test --filter Category=Smoke
```

### 5. Run Specific Test Methods

```bash
# Run a specific test
dotnet test --filter Name=VerifyHomepageLoadsWithMajorElements

# Run tests containing specific text
dotnet test --filter Name~Order
```

## 📋 Test Categories

### 🎨 UI Tests (`UITests.cs`)
- Homepage element validation (header, footer, navigation, hero banner)
- Order Now button visibility and functionality
- Menu categories display validation
- Responsive design testing
- Navigation menu validation
- Footer elements and social media links
- Page load performance validation

### ⚙️ Functional Tests (`FunctionalTests.cs`)
- Location search by ZIP code (valid/invalid)
- Order Now redirection validation
- Menu section navigation
- Newsletter subscription (valid/invalid email)
- Menu search functionality
- Dietary filters testing
- Add to cart functionality
- Location details and directions

### 🔗 Integration Tests (`IntegrationTests.cs`)
- Complete user journey simulation
- Embedded iframe validation
- External links behavior
- Location to ordering integration
- Session persistence testing
- API integration and data loading
- Error handling and recovery
- Mobile app integration validation

### ♿ Accessibility Tests (`AccessibilityTests.cs`)
- Image alt tag validation
- Keyboard navigation testing
- Form label validation
- WCAG compliance with Axe-Core
- Color contrast validation
- ARIA labels and roles
- Heading structure validation
- Focus management testing
- Zoom level accessibility

## 🔧 Configuration

### Test Settings (`appsettings.json`)

The test suite can be configured through the `appsettings.json` file:

- **BaseUrl**: Target website URL
- **Timeouts**: Various timeout settings
- **Browser Settings**: Chrome options and window size
- **Test Data**: Valid/invalid test data for different scenarios

## 📝 Logging

The test suite uses Serilog for comprehensive logging:

- **Console Output**: Real-time test execution information
- **File Logging**: Detailed logs saved to `logs/test-log-{date}.txt`
- **Screenshot Capture**: Automatic screenshots on test failures saved to `Screenshots/`

## 🎯 Page Object Model

The test suite follows the Page Object Model pattern:

### BasePage
- Common functionality for all pages
- Element waiting utilities
- Basic page validation methods

### HomePage
- Homepage-specific elements and actions
- Navigation methods
- Hero banner and Order Now functionality

### LocationPage
- Location finder functionality
- ZIP code search and validation
- Location results handling

### MenuPage
- Menu navigation and item validation
- Category browsing
- Search and filter functionality

## 🚨 Important Notes

### Locator Strategy
- **Placeholder Locators**: The CSS selectors in this test suite are placeholder examples
- **Real Implementation**: You'll need to inspect the actual  website and update the locators with real element selectors
- **Dynamic Content**: Some elements may load dynamically, requiring wait strategies

### Updating Locators
To update locators for real website elements:

1. Open the  website in Chrome
2. Right-click on elements and "Inspect"
3. Copy the appropriate CSS selectors or XPath
4. Update the locator properties in the Page Object classes

Example:
```csharp
// Update this placeholder
private By OrderNowButtonLocator => By.CssSelector(".order-now");

// With actual selector from website
private By OrderNowButtonLocator => By.CssSelector("[data-analytics='cta-order-now']");
```

## 🔍 Test Execution Examples

### Run Tests with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Generate Test Report
```bash
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

### Run Tests in Parallel
```bash
dotnet test --parallel
```

## 🛠️ Troubleshooting

### Common Issues

1. **ChromeDriver Version Mismatch**
   - Solution: WebDriverManager automatically handles this, but ensure Chrome browser is updated

2. **Element Not Found Errors**
   - Solution: Update locators with actual website selectors
   - Check if elements load dynamically and adjust wait times

3. **Timeout Errors**
   - Solution: Increase timeout values in configuration
   - Check network connectivity

4. **Access Denied Errors**
   - Solution: Website may have bot detection; add user-agent or adjust Chrome options

### Debug Mode
To run tests with debugging:
```bash
dotnet test --verbosity diagnostic
```

## 📊 Test Results

After running tests, you'll find:
- **Console Output**: Real-time test results
- **Log Files**: Detailed execution logs in `logs/` directory
- **Screenshots**: Failure screenshots in `Screenshots/` directory
- **Accessibility Report**: Generated accessibility report file

## 🤝 Contributing

To extend the test suite:

1. **Add New Page Objects**: Create new page classes inheriting from `BasePage`
2. **Add New Test Categories**: Create new test classes inheriting from `BaseTest`
3. **Update Locators**: Replace placeholder selectors with actual website elements
4. **Add Test Data**: Extend `TestDataProvider` with new test data sets

## 📄 License

This test suite is provided as an example implementation for educational and testing purposes.

## 🔗 Useful Commands

```bash
# Install dependencies
dotnet restore

# Build project
dotnet build

# Run all tests
dotnet test

# Run with specific filter
dotnet test --filter "Category=UI|Category=Functional"

# Clean build artifacts
dotnet clean

# Watch mode for development
dotnet test --watch
```

---

**Note**: This test suite contains placeholder locators and will need to be updated with actual element selectors from the  website for full functionality. The framework and structure are production-ready and follow industry best practices.
