using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient
{
    public class WebSocketClientHelper
    {
        public static string URL = "ws://localhost:8080/";
        /// <summary>
        /// 執行Client端連線相關工作
        /// </summary>
        /// <returns></returns>
        public static async Task RunClientAsync()
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            Uri serverUri = new Uri(URL);

            Console.WriteLine($"連線到 {serverUri}...");
            //連線到伺服器
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("WebSocket 連線建立");
            Console.WriteLine(
                @"操作提示:
1.輸入 我叫XXX ,會自動幫你命名
2.輸入 我是誰 ,會回答你是誰
3.輸入其他文字 ,會幫你算字有多長
");

            string SendMsg = "";
            while (SendMsg.ToLower() != "exit")
            {
                Console.WriteLine("請輸入訊息");
                //請輸入訊息
                SendMsg = Console.ReadLine() ?? "";
                // 發送訊息給WebSocket伺服器
                await SendMessageAsync(webSocket, SendMsg);

                //等待伺服器回應訊息
                string ReciveMsg = await ReceiveMessageAsync(webSocket);
                Console.WriteLine($"接收到伺服器回應的訊息:\n {ReciveMsg}");
            }

            // 關閉連線
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "連線被client端關閉.", CancellationToken.None);
        }
        /// <summary>
        /// 發送訊息給伺服器
        /// </summary>
        /// <param name="webSocket">ClientWebSocket物件</param>
        /// <param name="message">訊息</param>
        /// <returns></returns>
        private static async Task SendMessageAsync(ClientWebSocket webSocket, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// 接收並回傳訊息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        private static async Task<string> ReceiveMessageAsync(ClientWebSocket webSocket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
        }

    }
}
