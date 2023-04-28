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
            options.AddArguments("--log-level=3");  // INFO = 0, WARNING = 1, LOG_ERROR = 2, LOG_FATAL = 3




            //Testing Dictionary...
            Dictionary<string, string> myHref1 = new Dictionary<string, string>();
            Dictionary<string, string> myHref2 = new Dictionary<string, string>();
            Dictionary<string, string> myHref3 = new Dictionary<string, string>();





            using ( IWebDriver driver = new ChromeDriver(options) )
            {
                // 블로그 URL로 접속 
                //string strResult = driver.Url = "https://www.geoje.go.kr/index.geoje?contentsSid=12979";   // main URL..... 0-th level...
                string strResult = driver.Url = "https://tour.geoje.go.kr";   // main URL..... 0-th level...
                Console.WriteLine(strResult);

                string parentWindow = driver.CurrentWindowHandle;
                

                if (driver.Title.IndexOf("missing.html") == -1)
                {

                    // 대기 설정. (find로 객체를 찾을 때까지 검색이 되지 않으면 대기하는 시간 초단위)
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                    


                    IList<IWebElement> links = driver.FindElements(By.TagName("a"));

                    Console.WriteLine("1-depth : " + links.Count());
                    //links.First(element => element.Text == "English").Click();




                    int k = 0;
                    int nTotalCount2 = 0;
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
                            if( k > 1)
                            {                                
                                js.ExecuteScript("window.history.go(-1)");
                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            }
                        }

                        

                        Console.WriteLine(">>>  1-depth: " + k + " of " + links.Count()  );

                       
                        //Console.WriteLine(link);
                        Console.WriteLine(link.Text);
                        if (link.Text == "Korea" || link.Text == "LANGUAGE" || link.Text == "English" || link.Text == "中文" || link.Text == "日文")
                        {
                            k++;
                            continue;
                        }


                        Console.WriteLine(link.GetAttribute("href"));
                        if( link.GetAttribute("href").IndexOf("geoje.go.kr") == -1)
                        {
                            k++;
                            continue;
                        }

                        if (link.GetAttribute("href").IndexOf("#") != -1){
                            k++;
                            continue;
                        }

                    

                        if (link.GetAttribute("href").IndexOf("tour.geoje.go.kr") != -1)   // 거제시청 특수 케이스 회피..( 남부면 관광지..)
                        {
                            k++;
                            continue;
                        }

                        if( link.Text=="남부면 관광지")
                        {
                            k++;
                            continue;
                        }


                        if (myHref1.ContainsKey(link.GetAttribute("href") ) == false)  //Dictionary
                            myHref1.Add(link.GetAttribute("href"), link.Text);


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

                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));


                            string href1, title1;
                            href1  = link.GetAttribute("href");
                            title1 = link.Text;
                            if (myHref1.ContainsKey(href1) == false)
                                myHref1.Add(href1, title1 );


                            if (bNewWindow == true )  // 새창...WindowHandle    new tab or new window
                            {
                                String theLastName = "";
                                int nWindow = 0;
                                foreach (string window in driver.WindowHandles)
                                {
                                    //Console.WriteLine("     [" + nWindow + "]" + window);
                                    theLastName = window;
                                    nWindow++;
                                }
                                //Console.WriteLine("p     " +  parentWindow );
                                driver.SwitchTo().Window(theLastName);                    // Forcus to the 'new tab'
                                //Console.WriteLine("c     " + driver.CurrentWindowHandle);
                            }
                            else
                            {
                                driver.SwitchTo().Window( parentWindow );
                            }



                            IList<IWebElement> links2 = driver.FindElements(By.TagName("a"));
                            nTotalCount2 += links2.Count();

                            Console.WriteLine("     2-depth : " + links2.Count()   + ">>>> Total2 = " + nTotalCount2   + "<<<<< myHref2.Count = " + myHref2.Count );


                            int l = 0;
                            string href2, title2;
                            foreach (var link2 in links2)                               // 2-level...
                            {
                                 //href2  = "";
                                 //title2 = "";


                                 href2 = link2.GetAttribute("href");
                                 title2 = link2.Text;
                                
                                /*
                                if (l == 0)
                                {
                                    Console.WriteLine("     T" + title2);
                                    Console.WriteLine("     R" + href2  + " <<< " + href1 );
                                    Console.WriteLine("           l = " + l);
                                }
                                */
                                

                                if( myHref2.ContainsKey( href2  ) == false )
                                    myHref2.Add( href2 , title2);


                                //l++;

                            }

                            Console.WriteLine("     2-depth : " + "DONE!!!");

                        }

                    
                        
                        if (bNewWindow == true) {
                            if (driver.WindowHandles.Count() >= 2)
                            {
                                foreach (string window in driver.WindowHandles)
                                {
                                    //Console.WriteLine("p     " + parentWindow);
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



            Console.WriteLine("Total key/value pairs in" +
                    " myHref2 are : " + myHref2.Count);

            //FileStream F = new FileStream("test.dat", FileMode.OpenOrCreate,   FileAccess.ReadWrite);

            var outputFile1 = new StreamWriter("D:\\aaa1.txt");
            int c = 0;

            foreach (KeyValuePair<string, string> kvp1 in myHref1)
            {
                if (c < 10)
                    Console.WriteLine("Key1 = {0}, Value1 = {1}", kvp1.Key, kvp1.Value);

                outputFile1.WriteLine("{0}\t{1}", kvp1.Key, kvp1.Value);

                c++;
            }
            outputFile1.Close();

            var outputFile2 = new StreamWriter("D:\\aaa2.txt");
            c = 0;
            foreach (KeyValuePair<string, string> kvp2 in myHref2)
            {
                if( c < 10 )
                    Console.WriteLine("Key2 = {0}, Value2 = {1}",       kvp2.Key, kvp2.Value);

                outputFile2.WriteLine("{0}\t{1}", kvp2.Key, kvp2.Value);

                c++;
            }
            outputFile2.Close();
            //F.Close();


            // 2 level -> 3 level.
            using (IWebDriver driver = new ChromeDriver(options))
            {
                int nTotalCount3 = 0;
                foreach (KeyValuePair<string, string> kvp2 in myHref2)
                {
                    Console.WriteLine("[2].........." + kvp2.Key);
                    string href2 = kvp2.Key;


                    if (href2.IndexOf("geoje.go.kr") == -1)
                    {                     
                        continue;
                    }

                    if ( href2.IndexOf("tour.geoje.go.kr") != -1)   // 거제시청 특수 케이스 회피..( 남부면 관광지..)
                    {                        
                        continue;
                    }

                    driver.Url = href2;
                    string parentWindow = driver.CurrentWindowHandle;
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                    IList<IWebElement> links3 = driver.FindElements(By.TagName("a"));
                    nTotalCount3 += links3.Count();
                    Console.WriteLine("            3-depth : " + links3.Count()  + "......." + nTotalCount3);

                    string href3, title3;
                    int p = 0;
                    foreach (var link3 in links3)                           
                    {
                        href3 = "";
                        title3 = "";

                        if( (link3 != null)  && (link3.Text != null) )
                        {
                            href3 = link3.GetAttribute("href");
                            title3 = link3.Text;
                        }

                        if( p == 0)
                        {
                            Console.WriteLine("         T" + title3);
                            Console.WriteLine("         R" + href3);
                            Console.WriteLine("           p = " + p);
                        }
                        p++;

                        if ( href3.IndexOf("https://") == -1  )
                            continue;

                        if (href3.IndexOf("https://") == -1)
                            continue;

                        if (myHref3.ContainsKey(href3) == false)
                            myHref3.Add(href3, title3);                        

                    }

                }
            }

            var outputFile3 = new StreamWriter("D:\\aaa3.txt");
            c = 0;
            foreach (KeyValuePair<string, string> kvp3 in myHref3)
            {
                if (c < 10)
                    Console.WriteLine("Key2 = {0}, Value2 = {1}", kvp3.Key, kvp3.Value);

                outputFile2.WriteLine("{0}\t{1}", kvp3.Key, kvp3.Value);

                c++;
            }
            outputFile3.Close();


            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

    }
}


