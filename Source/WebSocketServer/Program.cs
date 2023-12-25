using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketServer;

class Program
{
    static async Task Main(string[] args)
    {
        //呼叫並等待結果,
        await WebSocketServerHelper.RunServerAsync();
    }
}
