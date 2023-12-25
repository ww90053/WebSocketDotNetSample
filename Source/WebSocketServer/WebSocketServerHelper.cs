using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer
{
    public class WebSocketServerHelper
    {
        public static string URL = "http://localhost:8080/";

        public static async Task RunServerAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(URL);
            listener.Start();

            Console.WriteLine($"WebSocket Server listening on {URL}");

            while (true)
            {
                //等待 取得WebSocket連線
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    //平行-處理WebSocket請求
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        /// <summary>
        /// 平行-處理WebSocket請求
        /// </summary>
        /// <param name="context"></param>
        private static async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);

            WebSocket webSocket = webSocketContext.WebSocket;
            string UserName = $"使用者{DateTime.Now.Ticks}";
            Console.WriteLine($"WebSocket 連線建立 (參數:{webSocketContext.RequestUri.Query},{UserName})");

            //建立一個平行續,將所有接收到的訊息都送給Client端

            // 處理接收到 WebSocket 訊息
            while (webSocket.State == WebSocketState.Open)
            {
                //等待 接收Client端發送的訊息,return 字串
                string message = await ReceiveMessageAsync(webSocket);
                Console.WriteLine($"接收到{UserName}發送的訊息: {message}");

                //有要求改名嗎?
                if (message.Contains("我是誰"))
                {
                    //這邊將接收到的訊息送回去給Client端
                    await SendMessageAsync(webSocket, $"你是 {UserName}");
                    continue;//下一回合
                }
                if (message.StartsWith("我叫"))
                {
                    UserName = message.Replace("我叫", "");

                    //這邊將接收到的訊息送回去給Client端
                    await SendMessageAsync(webSocket, $"{UserName} 你好");
                    continue;//下一回合
                }

                //這邊將接收到的訊息送回去給Client端
                string SendMessage = $"{UserName} 你剛才打的訊息是 {message} ,你總共打了{message.Length}個字";
                await SendMessageAsync(webSocket, SendMessage);
            }
        }

        /// <summary>
        /// 平行 接收Client端發送的訊息,return 字串
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        private static async Task<string> ReceiveMessageAsync(WebSocket webSocket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
        }

        /// <summary>
        /// 平行 發送訊息給Client端
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }


    }
}
