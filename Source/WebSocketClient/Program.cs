using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient;

class Program
{
    static async Task Main(string[] args)
    {
        //等待五秒
        Console.WriteLine("5秒後開始 連線Web Socket伺服器");
        Thread.Sleep(5000);

        //呼叫並等待結果
        //執行Client端連線相關工作
        await WebSocketClientHelper.RunClientAsync();
    }
}
