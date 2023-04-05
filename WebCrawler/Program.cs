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
        

        static void Main(string[] args)
        {
            // ChomeDriver 인스턴스 생성                        
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--test-type");
            options.AddArguments("--ignore-certificate-errors");
                                    
            using ( IWebDriver driver = new ChromeDriver(options) )
            {
                // 블로그 URL로 접속 
                driver.Url = "https://www.etnews.com/20230331000229";

                // 대기 설정. (find로 객체를 찾을 때까지 검색이 되지 않으면 대기하는 시간 초단위)
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                Thread.Sleep(1000);



            

                IList<IWebElement> links = driver.FindElements(By.TagName("a"));
                Console.WriteLine(links[0].Text);
                //links.First(element => element.Text == "English").Click();
                foreach (var link in links)
                {
                    Console.WriteLine( link.Text );                    
                    if (link.Text == "English")
                    {
                        Console.WriteLine(link.GetAttribute("href"));                        
                        link.Click();
                        break;
                    }
                }


                // 5초 기다린다.
                Thread.Sleep(5000);

                // ezWEB1.5는 기존창 대신에 새창에서 열린다.
                string originWindow = driver.CurrentWindowHandle;
                Console.WriteLine(originWindow);
                foreach (string window in driver.WindowHandles)
                {
                    //새창을 찾는다.
                    if (originWindow != window)
                    {
                        Console.WriteLine(window);
                        driver.SwitchTo().Window(window);
                        break;
                    }
                }




                // iFrame 찾기
                List<IWebElement> frames = new List<IWebElement>(driver.FindElements(By.TagName("iframe")));
                Console.WriteLine("Number of Frames: " + frames.Count);
                for (int i = 0; i < frames.Count; i++)
                {
                    Console.WriteLine("frame[" + i + "]: " + frames[i].GetAttribute("id").ToString());
                    //Console.WriteLine("frame[" + i + "]: " + frames[i].GetAttribute("name").ToString());
                }
                driver.SwitchTo().Frame("CSLiResultPage");


                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //js.ExecuteScript("window.open()");
                js.ExecuteScript("document.getElementsByTagName(\"html\")[0].setAttribute(\"lang\",\"en\") ");




                string aaa = driver.PageSource.ToString();
                string bbb = aaa.Replace("https://trans.etnews.com", "http://sahngwoon.llsollu.io");
                Console.WriteLine(bbb);


                File.WriteAllText("C:\\Users\\Sahngwoon.Lee\\Downloads\\PageSource.html", bbb);
                //File.WriteAllText("C:\\Users\\Sahngwoon.Lee\\Downloads\\PageSource.html", driver.PageSource);

                // 5초 기다린다.
                Thread.Sleep(1000);
                                                             

                driver.Quit();

            }
           
            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

    }
}


