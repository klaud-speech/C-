using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace WebCrawler
{

    static class Program
    {
        /*
        static void ExecuteScript(IJavaScriptExecutor driver, string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script);
        }
        */
        static IJavaScriptExecutor Script(this IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }

        // 스크립트 클릭 함수
        static void ClickScript(this IWebDriver driver, IWebElement element)
        {
            // 스크립트 클릭
            driver.Script().ExecuteScript("arguments[0].click();", element);
        }

        static void setAttribute(this IWebDriver driver, IWebElement element, String attName, String attValue)
        {
            driver.Script().ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);",  element, attName, attValue);
        }


        static void Main(string[] args)
        {
            // ChomeDriver 인스턴스 생성                        
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--test-type");
            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("--headless");    // 웹브라우저 띄우지 않고...


            using ( IWebDriver driver = new ChromeDriver(options) )
            {
                // 블로그 URL로 접속 
                string strResult = driver.Url = "https://www.geoje.go.kr/index.geoje?contentsSid=12979";   // main URL..... 0-th level...
                Console.WriteLine(strResult);

                string parentWindow = driver.CurrentWindowHandle;
                

                if (driver.Title.IndexOf("missing.html") == -1)
                {

                    // 대기 설정. (find로 객체를 찾을 때까지 검색이 되지 않으면 대기하는 시간 초단위)
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    


                    IList<IWebElement> links = driver.FindElements(By.TagName("a"));

                    Console.WriteLine("1-depth : " + links.Count());
                    //links.First(element => element.Text == "English").Click();




                    int k = 0;
                    bool bNewWindow = false;
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    foreach (var link in links)            //  foreach  1-level.....
                    {
                        if (bNewWindow == true) {
                            //if(driver.WindowHandles.Count() == 2)
                            Console.WriteLine("pWindow " + parentWindow);
                            driver.SwitchTo().Window(parentWindow);  // Forcus return to MainWindow
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        }
                        else
                        {
                            if( k > 0)
                            {                                
                                js.ExecuteScript("window.history.go(-1)");
                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            }
                        }

                        Console.WriteLine("1-de pth: " + k + " of " + links.Count()  );

                        {
                            //Console.WriteLine(link);
                            Console.WriteLine(link.Text);
                            if (link.Text == "Korea" || link.Text=="LANGUAGE" || link.Text == "English" || link.Text == "中文" || link.Text == "日文")
                                continue;


                            Console.WriteLine(link.GetAttribute("href"));
                            if( link.GetAttribute("href").IndexOf("geoje.go.kr") == -1)
                            {
                                continue;
                            }

                            if (link.GetAttribute("href").IndexOf("#") != -1){
                                continue;
                            }


                            if ( link.GetAttribute("href").Contains("llsollu") == false  )
                            {

                                //모든 URL을 '새탭'으로 
                                if (link.GetAttribute("target").IndexOf("_blank") != -1)
                                {
                                    bNewWindow = true;
                                }
                                else
                                {
                                    bNewWindow = false;
                                    setAttribute(driver, link, "target", "_blank");        
                                    bNewWindow = true;
                                }

                                //link.Click();
                                driver.ClickScript(link);           /// **** CLICK!!!!  CLICK!!!!  1-level...

                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                
                                




                                if (bNewWindow == true )  // 새창...WindowHandle    new tab or new window
                                {
                                    String theLastName = "";
                                    int nWindow = 0;
                                    foreach (string window in driver.WindowHandles)
                                    {
                                        Console.WriteLine("     [" + nWindow + "]" + window);
                                        theLastName = window;
                                        nWindow++;
                                    }
                                    Console.WriteLine("p     " +  parentWindow );
                                    driver.SwitchTo().Window(theLastName);                    // Forcus to the 'new tab'
                                    Console.WriteLine("c     " + driver.CurrentWindowHandle);
                                }
                                else
                                {
                                    driver.SwitchTo().Window( driver.CurrentWindowHandle );
                                }



                                IList<IWebElement> links2 = driver.FindElements(By.TagName("a"));
                                Console.WriteLine("     2-depth : " + links2.Count());

                                int l = 0;
                                foreach (var link2 in links2)                               // 2-level...
                                {
                                    if( l < 3)
                                    {
                                        Console.WriteLine("     " + link2.Text);
                                        Console.WriteLine("     " + link2.GetAttribute("href"));
                                        l++;
                                    }

                                }

                                Console.WriteLine("     2-depth : " + "DONE!!!");

                            }

                    
                        }
                        if (bNewWindow == true) {
                            if (driver.WindowHandles.Count() >= 2)
                            {
                                foreach (string window in driver.WindowHandles)
                                {
                                    Console.WriteLine("p     " + parentWindow);
                                    if (window != parentWindow)
                                    {
                                        driver.SwitchTo().Window(window);
                                        driver.Close();
                                    }
                                }
                            }
                        }

                        k++;

                        //if (driver.WindowHandles.Count() >= 2)
                        //    return;

                    }

                    

                }
                driver.SwitchTo().Window(parentWindow);
                driver.Close();
                driver.Quit();
            }
           
            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

    }
}


