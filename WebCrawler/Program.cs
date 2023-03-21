using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace WebCrawler
{
    static class Program
    {
        // driver를 Script 실행 인터페이스로 변환
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
            // 스택 영역이 종료되면 자동으로 Chrome 브라우저는 닫힌다.
            using (IWebDriver driver = new ChromeDriver())
            {
                // 블로그 URL로 접속
                driver.Url = "https://nowonbun.tistory.com";

                /*
                // 대기 설정. (find로 객체를 찾을 때까지 검색이 되지 않으면 대기하는 시간 초단위)
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                // xpath로 element를 찾는다. 이 xpath는 명월 일지 블로그의 왼쪽 메뉴의 Dev note의 Javascript, Jquery, Css 카테고리다.
                var element = driver.FindElement(By.XPath("//*[@id='leftside']/div[3]/ul/li/ul/li[1]/ul/li[6]/a"));

                // 클릭한다. 사실 element.Click()로도 클릭이 가능한데 가끔 호환성 에러가 발생하는 경우가 있다.
                driver.ClickScript(element);

                // css selector로 "[Javascript] 팝업 라이브러리(bootbox)"의 포스트를 찾는다. 찾을 때까지 무한 루프
                while (true)
                {
                    try
                    {
                        // css selector로 element를 찾는다.
                        element = driver.FindElement(By.CssSelector("[href^='/626']"));
                        // 클릭
                        driver.ClickScript(element);
                        // 루프 정지
                        break;
                    }
                    catch (Exception)
                    {
                        // 해당 element가 없으면 아래의 다음 페이지 element를 찾는다.
                        element = driver.FindElement(By.CssSelector(".paging li.active+li > a"));
                        // 클릭
                        driver.ClickScript(element);
                    }
                }

                // id가 promptEx인 데이터를 찾는다.
                element = driver.FindElement(By.XPath("//*[@id='promptEx']"));
                element.Click();

                // xpath로 팝업의 dom를 찾는다.
                element = driver.FindElement(By.XPath("/html/body/div[6]/div/div/div[2]/div/form/input"));
                // input text에 test의 값을 넣는다.
                element.SendKeys("test");
                // 5초 기다린다.
                Thread.Sleep(5000);

                // xpath로 팝업의 dom를 찾는다.
                element = driver.FindElement(By.XPath("/html/body/div[6]/div/div/div[2]/div/form/input"));

                // 속성 value를 출력한다.
                Console.WriteLine(element.GetAttribute("value"));

                // .article의 글에 p 태그의 속성을 전부 가져온다.
                var elements = driver.FindElements(By.CssSelector(".article p"));
                foreach (var ele in elements)
                {
                    // 속성의 NodeText를 전부 출력한다.
                    Console.WriteLine(ele.Text);
                }
                */
            }
            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
