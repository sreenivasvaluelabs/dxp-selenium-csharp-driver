using OpenQA.Selenium;
using Selenium.Axe;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace IhopSeleniumTests.Helpers
{
    public class AccessibilityHelper
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;

        public AccessibilityHelper(IWebDriver driver)
        {
            _driver = driver;
            _logger = Log.ForContext<AccessibilityHelper>();
        }

        public AxeResult RunAxeAccessibilityScan()
        {
            try
            {
                var axeBuilder = new AxeBuilder(_driver);
                var results = axeBuilder.Analyze();
                
                _logger.Information($"Accessibility scan completed. Violations: {results.Violations.Count()}, " +
                                  $"Passes: {results.Passes.Count()}, Incomplete: {results.Incomplete.Count()}");
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to run Axe accessibility scan");
                throw;
            }
        }

        public bool ValidateImageAltTags()
        {
            try
            {
                var images = _driver.FindElements(By.TagName("img"));
                int imagesWithoutAlt = 0;
                int totalImages = images.Count;

                foreach (var image in images)
                {
                    try
                    {
                        string altText = image.GetAttribute("alt");
                        if (string.IsNullOrEmpty(altText))
                        {
                            imagesWithoutAlt++;
                            string src = image.GetAttribute("src");
                            _logger.Warning($"Image without alt text found: {src}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Error checking alt text for image");
                        imagesWithoutAlt++;
                    }
                }

                bool allImagesHaveAlt = imagesWithoutAlt == 0;
                _logger.Information($"Image alt tag validation - Total images: {totalImages}, " +
                                  $"Images without alt: {imagesWithoutAlt}, All have alt: {allImagesHaveAlt}");
                
                return allImagesHaveAlt;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating image alt tags");
                return false;
            }
        }

        public bool ValidateKeyboardNavigation()
        {
            try
            {
                // Find all interactive elements
                var interactiveElements = _driver.FindElements(By.CssSelector(
                    "a, button, input, select, textarea, [tabindex], [role='button'], [role='link']"));

                int accessibleElements = 0;
                int totalElements = interactiveElements.Count;

                foreach (var element in interactiveElements.Take(10)) // Test first 10 elements
                {
                    try
                    {
                        // Check if element is focusable
                        element.SendKeys("");
                        
                        // Check if element is visible and enabled
                        if (element.Displayed && element.Enabled)
                        {
                            accessibleElements++;
                        }
                    }
                    catch (Exception)
                    {
                        // Element might not be focusable or interactable
                        continue;
                    }
                }

                bool keyboardAccessible = accessibleElements > 0;
                _logger.Information($"Keyboard navigation validation - Accessible elements: {accessibleElements}/{Math.Min(totalElements, 10)}");
                
                return keyboardAccessible;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating keyboard navigation");
                return false;
            }
        }

        public bool ValidateFormLabels()
        {
            try
            {
                var inputElements = _driver.FindElements(By.CssSelector("input, select, textarea"));
                int inputsWithoutLabels = 0;
                int totalInputs = inputElements.Count;

                foreach (var input in inputElements)
                {
                    try
                    {
                        bool hasLabel = false;
                        
                        // Check for associated label by 'for' attribute
                        string inputId = input.GetAttribute("id");
                        if (!string.IsNullOrEmpty(inputId))
                        {
                            var associatedLabel = _driver.FindElements(By.CssSelector($"label[for='{inputId}']"));
                            if (associatedLabel.Count > 0)
                            {
                                hasLabel = true;
                            }
                        }

                        // Check for aria-label
                        if (!hasLabel)
                        {
                            string ariaLabel = input.GetAttribute("aria-label");
                            if (!string.IsNullOrEmpty(ariaLabel))
                            {
                                hasLabel = true;
                            }
                        }

                        // Check for aria-labelledby
                        if (!hasLabel)
                        {
                            string ariaLabelledBy = input.GetAttribute("aria-labelledby");
                            if (!string.IsNullOrEmpty(ariaLabelledBy))
                            {
                                hasLabel = true;
                            }
                        }

                        // Check for placeholder (less ideal but acceptable)
                        if (!hasLabel)
                        {
                            string placeholder = input.GetAttribute("placeholder");
                            if (!string.IsNullOrEmpty(placeholder))
                            {
                                hasLabel = true;
                            }
                        }

                        if (!hasLabel)
                        {
                            inputsWithoutLabels++;
                            string inputType = input.GetAttribute("type") ?? "unknown";
                            string inputName = input.GetAttribute("name") ?? "unnamed";
                            _logger.Warning($"Input without label found - Type: {inputType}, Name: {inputName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Error checking label for input element");
                        inputsWithoutLabels++;
                    }
                }

                bool allInputsHaveLabels = inputsWithoutLabels == 0;
                _logger.Information($"Form label validation - Total inputs: {totalInputs}, " +
                                  $"Inputs without labels: {inputsWithoutLabels}, All have labels: {allInputsHaveLabels}");
                
                return allInputsHaveLabels;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating form labels");
                return false;
            }
        }

        public List<string> GetAccessibilityViolations()
        {
            try
            {
                var results = RunAxeAccessibilityScan();
                var violations = new List<string>();

                foreach (var violation in results.Violations)
                {
                    string violationDescription = $"Rule: {violation.Id} - {violation.Description} " +
                                                $"(Impact: {violation.Impact}, Nodes: {violation.Nodes.Count()})";
                    violations.Add(violationDescription);
                    
                    _logger.Warning($"Accessibility violation: {violationDescription}");
                }

                return violations;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting accessibility violations");
                return new List<string>();
            }
        }

        public bool ValidateColorContrast()
        {
            try
            {
                var results = RunAxeAccessibilityScan();
                var colorContrastViolations = results.Violations
                    .Where(v => v.Id.Contains("color-contrast"))
                    .ToList();

                bool hasColorContrastIssues = colorContrastViolations.Any();
                _logger.Information($"Color contrast validation - Issues found: {hasColorContrastIssues}, " +
                                  $"Violations: {colorContrastViolations.Count}");
                
                return !hasColorContrastIssues;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating color contrast");
                return false;
            }
        }

        public AccessibilityReport GenerateAccessibilityReport()
        {
            try
            {
                var report = new AccessibilityReport
                {
                    ImageAltTagsValid = ValidateImageAltTags(),
                    KeyboardNavigationValid = ValidateKeyboardNavigation(),
                    FormLabelsValid = ValidateFormLabels(),
                    ColorContrastValid = ValidateColorContrast(),
                    AxeViolations = GetAccessibilityViolations(),
                    TestTimestamp = DateTime.Now
                };

                _logger.Information($"Accessibility report generated - Overall compliance: {report.IsCompliant}");
                return report;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error generating accessibility report");
                throw;
            }
        }
    }

    public class AccessibilityReport
    {
        public bool ImageAltTagsValid { get; set; }
        public bool KeyboardNavigationValid { get; set; }
        public bool FormLabelsValid { get; set; }
        public bool ColorContrastValid { get; set; }
        public List<string> AxeViolations { get; set; } = new List<string>();
        public DateTime TestTimestamp { get; set; }

        public bool IsCompliant => ImageAltTagsValid && KeyboardNavigationValid && 
                                  FormLabelsValid && ColorContrastValid && 
                                  !AxeViolations.Any();

        public string GetSummary()
        {
            return $"Accessibility Report ({TestTimestamp:yyyy-MM-dd HH:mm:ss})\n" +
                   $"- Image Alt Tags: {(ImageAltTagsValid ? "PASS" : "FAIL")}\n" +
                   $"- Keyboard Navigation: {(KeyboardNavigationValid ? "PASS" : "FAIL")}\n" +
                   $"- Form Labels: {(FormLabelsValid ? "PASS" : "FAIL")}\n" +
                   $"- Color Contrast: {(ColorContrastValid ? "PASS" : "FAIL")}\n" +
                   $"- Axe Violations: {AxeViolations.Count}\n" +
                   $"- Overall Compliance: {(IsCompliant ? "PASS" : "FAIL")}";
        }
    }
}
