using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp14
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] urls = new string[]
            //{
            //        "http://127.0.0.1:7789/HardwareConfig/ReturnDefaultWeighMode?Code=01",
            //        "http://127.0.0.1:7789/Search/GetMonthDailyNetWeight?Year=2025&Month=06",
            //        "http://127.0.0.1:7789/LocalSetting/GetBasicSystemSettings",
            //        "http://127.0.0.1:7789/Search/SearchCompleteWeight?CarNumber=&page=1&limit=10"
            //};
            string[] urls = new string[]
            {
                    "http://127.0.0.1:7789/ApiServer/Sum?a=10&b=20",
            };

            int concurrentRequests = 1; // 并发数
            int totalRequests = 2000; // 总请求数

            var result = RunStressTest(urls, concurrentRequests, totalRequests).GetAwaiter().GetResult();

            WriteLog($"完成: 成功 {result.SuccessCount}，失败 {result.FailCount}");
            WriteLog("按任意键退出...");
            Console.ReadKey();
        }
        static readonly object logLock = new object();

        static void WriteLog(string message)
        {
            lock (logLock)
            {
                File.AppendAllText("log.txt", message + Environment.NewLine, Encoding.UTF8);
                Console.WriteLine(message);
            }
        }
        public static async Task<(int SuccessCount, int FailCount)> RunStressTest(string[] urls, int concurrent, int total)
        {
            int success = 0, fail = 0;
            using (var client = new HttpClient())
            {
                SemaphoreSlim semaphore = new SemaphoreSlim(concurrent);
                Task[] tasks = new Task[total];
                Random rand = new Random();

                for (int i = 0; i < total; i++)
                {
                    await semaphore.WaitAsync();
                    int requestIndex = i + 1;
                    tasks[i] = Task.Run(async () =>
                    {
                        string url;
                        lock (rand)
                        {
                            url = urls[rand.Next(urls.Length)];
                        }
                        try
                        {
                            // 输出当前请求的URL
                            WriteLog($"请求{requestIndex}: URL={url}");

                            //// 先发送OPTIONS请求
                            var optionsRequest = new HttpRequestMessage(HttpMethod.Options, url);
                            var optionsResponse = await client.SendAsync(optionsRequest);
                            string optionsContent = await optionsResponse.Content.ReadAsStringAsync();
                            WriteLog($"请求{requestIndex} [OPTIONS]: 状态码={optionsResponse.StatusCode}, 内容={optionsContent.Substring(0, Math.Min(100, optionsContent.Length))}");

                            // 再发送GET请求
                            var response = await client.GetAsync(url);
                            string content = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                                Interlocked.Increment(ref success);
                            else
                                Interlocked.Increment(ref fail);

                            WriteLog($"请求{requestIndex} [GET]: 状态码={response.StatusCode}, 内容={content.Substring(0, Math.Min(100, content.Length))}");
                        }
                        //catch (Exception ex)
                        //{
                        //    Interlocked.Increment(ref fail);
                        //    WriteLog($"请求{requestIndex}: 失败, 异常={ex.Message}\n{ex.StackTrace}");
                        //}
                        finally
                        {
                            //await Task.Delay(1000);
                            semaphore.Release();
                        }
                    });
                }

                await Task.WhenAll(tasks);
            }
            return (success, fail);
        }
    }
}
