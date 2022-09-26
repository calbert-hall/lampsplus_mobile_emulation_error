using Xunit;
using Applitools;
using System;
using System.Collections.Specialized;
using System.Configuration;
//using OpenQA.Selenium.Appium;
//using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium;
using Applitools.Selenium;
using System.Net;
using OpenQA.Selenium.Chrome;
using Configuration = Applitools.Selenium.Configuration;

namespace Applitools_troubleshooting
{
    public class LampsPlus
    {
        //Base initialization class
        public class TestsBase : IDisposable
        {
            public string testingEnv = string.Empty;

            public TestsBase()
            {
                eyes = new Eyes();
            }

            public IWebDriver Driver { get; set; }
            public Applitools.Selenium.Eyes eyes { get; set; }

            public void TestInialization()
            {
                Configuration config = new Configuration();
                config.IgnoreDisplacements = true;
 
                eyes.SetConfiguration(config);
                eyes.SaveDiffs = true;//Overrides baseline with the same name
                eyes.SendDom = false;//Mobile tests failed without this command (solution was provided by Applitools support).
                
                eyes.StitchMode = StitchModes.CSS;
                eyes.ApiKey = System.Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");

                BatchInfo batchInfo = new BatchInfo("SdkTroubleshooting_3.19.21");
                batchInfo.Id = "SdkTroubleshooting_3.19.21";
                eyes.Batch = batchInfo;

                var options = new ChromeOptions
                {
                    AcceptInsecureCertificates = true
                };

                options.EnableMobileEmulation("iPhone 6/7/8"); // THIS CAUSES THE ERROR: "Cannot read proprerty 'startsWith'
               
                Driver = new ChromeDriver(options);
                Driver.Manage().Window.Maximize();
                Driver.Navigate().GoToUrl("https://www.lampsplus.com/products/havanese-dog-18-inch-square-throw-pillow__30n73.html");
      
                eyes.Open(Driver, "LampsPlus", "testSDK");
            }

            public void Navigate(string url)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    Driver.Navigate().GoToUrl(url);
                }
            }

            public void ApplitoolsCapture()
            {
                eyes.CheckWindow($"TestSdkError");
            }

            public void Dispose()
            {
                Driver?.Close();
                Driver?.Quit();

                if (eyes != null)
                {
                    TestResults result = eyes?.Close(false); //If Close() argument bool is "true", comparison test will fail if it fails on Applitools.

                    if (testingEnv == "A")//Delete baseline to have combined test result on Applitools dashboard for the test.
                    {
                        result.Delete();
                    }

                    eyes.AbortIfNotClosed(); //Eyes method: If you call it after the test has been succesfully closed, then the call is ignored.         
                }
            }
        }

        public class VisualTest : TestsBase
        {

            [SkippableTheory]
            [InlineData("A")]
            [InlineData("B")]
            public void Test(string instance)
            {
                testingEnv = instance;
                TestInialization();
                ApplitoolsCapture();
            }
        }
    }
}
