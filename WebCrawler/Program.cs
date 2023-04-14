using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using OpenQA.Selenium.Chrome;
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


        static void Main(string[] args)
        {
            // ChomeDriver 인스턴스 생성                        
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--test-type");
            options.AddArguments("--ignore-certificate-errors");
            //options.AddArguments("--headless");    // 웹브라우저 띄우지 않고...


            using ( IWebDriver driver = new ChromeDriver(options) )
            {
                // 블로그 URL로 접속 
                string strResult = driver.Url = "https://www.geoje.go.kr/index.geoje";   // main URL
                Console.WriteLine(strResult);

                string parentWindow = driver.CurrentWindowHandle;
                

                if (driver.Title.IndexOf("missing.html") == -1)
                {

                    // 대기 설정. (find로 객체를 찾을 때까지 검색이 되지 않으면 대기하는 시간 초단위)
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                    Thread.Sleep(1000);


                    IList<IWebElement> links = driver.FindElements(By.TagName("a"));

                    Console.WriteLine("1-depth : " + links.Count());
                    //links.First(element => element.Text == "English").Click();




                    int k = 0;
                    foreach (var link in links)
                    {

                        driver.SwitchTo().Window(parentWindow);

                        
                        {
                            //Console.WriteLine(link);
                            Console.WriteLine(link.Text);
                            Console.WriteLine(link.GetAttribute("href"));

                            if( ( link.GetAttribute("href").IndexOf("#") == -1 ) || ( link.GetAttribute("href").Contains("etgi") == false ) )
                            {
                                //link.Click();
                                driver.ClickScript(link);           /// CLICK!!!!  CLICK!!!!

                                Thread.Sleep(5000);

                                String theLastName = "";
                                foreach (string window in driver.WindowHandles)
                                {
                                    Console.WriteLine("     " + window);
                                    theLastName = window;
                                    
                                }
                                driver.SwitchTo().Window(theLastName);

                                

                                IList<IWebElement> links2 = driver.FindElements(By.TagName("a"));
                                Console.WriteLine("     2-depth : " + links2.Count());

                                int l = 0;
                                foreach (var link2 in links2)
                                {
                                    if( l < 3)
                                    {
                                        Console.WriteLine("     " + link2.Text);
                                        Console.WriteLine("     " + link2.GetAttribute("href"));
                                        l++;
                                    }

                                }                                

                                

                            }

                            k++;
                        }

                        Thread.Sleep(5000);

                    }
                    

                   
                }
                driver.Quit();
            }
           
            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

    }
}


