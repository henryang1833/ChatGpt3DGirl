using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
namespace ChatGpt
{
    public delegate void OnRecvASentenceHandler(string message);
    public class ChatAgent : MonoBehaviour
    {
        private IPEndPoint remoteEP;
        public event OnRecvASentenceHandler onRecvASentenceEvent;

        private String wordBuffer = "";
        private Queue<String> sentenceQueue = new Queue<String>();
        private bool isRecving = false;
        private bool isChating = false;
        private string ip;
        private int port;

        
        public void Initialize(string ip,int port)
        {
            this.ip = ip;
            this.port = port;
            // 服务器的IP地址和端口
            IPAddress ipAddress = IPAddress.Parse(ip); // IP
            remoteEP = new IPEndPoint(ipAddress, port);
        }
        public void BeginChat()
        {
            isChating = true;
        }
        public void EngChat()
        {
            isChating = false;
            isRecving = false;
            wordBuffer= string.Empty;
            sentenceQueue.Clear();
            onRecvASentenceEvent = null;
        }
        

        public IEnumerator Chat(string wavPath)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool connected = false;
            try
            {
                sock.Connect(remoteEP);
                // 连接成功
                Debug.Log("连接成功！");
                connected = true;
            }
            catch (System.Exception e)
            {
                // 处理发送异常
                Debug.LogError("发送失败：" + e.Message);
                connected = false;
            }

            if (connected)
            {
                byte[] byteSendArray = File.ReadAllBytes(wavPath);
                int bytesSent = sock.Send(byteSendArray);
                // 数据发送成功

                if (bytesSent > 0)
                {
                    Debug.Log("数据发送成功" + bytesSent);
                    byte[] byteBufferArray = new byte[50 * 1024 * 1024];//50M空间接受数据
                    sock.Blocking = false;
                    int byteRecv = 0;
                    while (byteRecv <= 0)
                    {
                        try
                        {
                            byteRecv = sock.Receive(byteBufferArray);
                        }
                        catch
                        {

                        }
                        yield return null;
                    }
                    Debug.Log("数据接受成功" + byteRecv);
                    byte[] byteRecvArray = new byte[byteRecv];
                    Array.Copy(byteBufferArray, byteRecvArray, byteRecv);
                    // 释放Socket资源
                    BaiDuAI.Instance.OnRecvData?.Invoke(byteRecvArray);
                }
                else
                {
                    Debug.Log("数据发送失败");
                }
            }
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }


        public IEnumerator Chat(byte[] byteSendArray)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool connected = false;
            try
            {
                sock.Connect(remoteEP);
                // 连接成功
                Debug.Log("连接成功！");
                connected = true;
            }
            catch (System.Exception e)
            {
                // 处理发送异常
                Debug.LogError("发送失败：" + e.Message);
                connected = false;
            }

            if (connected)
            {
                int bytesSent = sock.Send(byteSendArray);
                // 数据发送成功

                if (bytesSent > 0)
                {
                    Debug.Log($"数据发送成功,共发送了{bytesSent}个字节" + bytesSent);


                    byte[] byteBufferArray = new byte[50 * 1024 * 1024];//50M空间接受数据
                    int bufferSize = byteBufferArray.Length;
                    sock.Blocking = false;
                    int totalBytesReceived = 0;
                    while (totalBytesReceived <= 0)
                    {
                        int byteRecv = 0;
                        try
                        {
                            byteRecv = sock.Receive(byteBufferArray);
                            totalBytesReceived += byteRecv;
                        }
                        catch
                        {

                        }
                        // 确保字节数组至少有四个字节
                        if (byteRecv >= 4)
                        {
                            byte[] byteTotalLenght = new byte[4];
                            Array.Copy(byteBufferArray, byteTotalLenght, 4);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(byteTotalLenght);
                            }
                            int totalLength = BitConverter.ToInt32(byteTotalLenght, 0) + 4; // 从数组的第0个位置开始转换
                            Debug.Log($"即将接受的音频数据总长度{totalLength - 4}");
                            while (totalBytesReceived < totalLength)
                            {
                                try
                                {
                                    byteRecv = sock.Receive(byteBufferArray, totalBytesReceived, bufferSize - totalBytesReceived, SocketFlags.None);
                                    totalBytesReceived += byteRecv;
                                }
                                catch
                                {

                                }
                                yield return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("字节数组长度不足以进行转换");
                        }

                        yield return null;
                    }
                    Debug.Log("数据接受成功" + (totalBytesReceived - 4));
                    byte[] byteRecvArray = new byte[totalBytesReceived - 4];
                    Array.Copy(byteBufferArray, 4, byteRecvArray, 0, totalBytesReceived - 4);
                    byteBufferArray = null;
                    // 释放Socket资源
                    BaiDuAI.Instance.OnRecvData?.Invoke(byteRecvArray);
                }
                else
                {
                    Debug.Log("数据发送失败");
                }
            }
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }

        public IEnumerator TextChat(string text, OnRecvASentenceHandler onRecvASentenceHandler)
        {
            if (isChating)
            {
                onRecvASentenceEvent += onRecvASentenceHandler;
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool connected = false;
                try
                {
                    sock.Connect(remoteEP);
                    // 连接成功
                    Debug.Log("连接成功！");
                    connected = true;
                }
                catch (System.Exception e)
                {
                    // 处理发送异常
                    Debug.LogError("发送失败：" + e.Message);
                    connected = false;
                }

                if (connected)
                {
                    int bytesSent = sock.Send(System.Text.Encoding.UTF8.GetBytes(text));
                    

                    if (bytesSent > 0)
                    {
                        // 数据发送成功

                        Debug.Log($"数据发送成功,共发送了{bytesSent}个字节" + bytesSent);

                        sock.Blocking = false;
                        isRecving = true;
                        StartCoroutine(WordBufferWatcher());
                        StartCoroutine(SentenceQueueWatcher());
                        while (isRecving && isChating)
                        {
                            try
                            {
                                byte[] byteBufferArray = new byte[1024 * 1024];//1M空间接受数据
                                int byteRecv = sock.Receive(byteBufferArray);
                                string strChunk = Encoding.UTF8.GetString(byteBufferArray, 0, byteRecv);                            
                                wordBuffer += strChunk;
                            }
                            catch
                            {

                            }

                            yield return null;

                        }
                    }
                    else
                    {
                        Debug.Log("数据发送失败");
                    }
                }
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
        }

        IEnumerator WordBufferWatcher()
        {
            char[] cutWords = { '。', '?', '？', '!', '！', '：',':' };
            while (isChating && (wordBuffer.Length > 0 || isRecving))
            {
                int start = 0;
                for (int i = 0; i < wordBuffer.Length; i++)
                {
                    for (int j = 0; j < cutWords.Length; j++)
                    {
                        if (wordBuffer[i] == cutWords[j])
                        {
                            sentenceQueue.Enqueue(wordBuffer.Substring(start, i - start + 1));
                            start = i + 1;
                            break;
                        }
                    }
                }

                if (start < wordBuffer.Length)
                {
                    wordBuffer = wordBuffer.Substring(start);
                }
                else
                {
                    wordBuffer = "";
                }

                if (wordBuffer.Length > 0 && wordBuffer.Contains("END_OF_STREAM"))
                {
                    isRecving = false;
                    wordBuffer = "";
                }
                yield return null;
            }
        }

        IEnumerator SentenceQueueWatcher()
        {
            while (isChating&&(sentenceQueue.Count > 0 || isRecving))
            {               
                if (sentenceQueue.Count > 0)
                {
                    string str = sentenceQueue.Dequeue();
                    onRecvASentenceEvent?.Invoke(str);
                }
                yield return null;
            }
            onRecvASentenceEvent = null;
        }
        
    }
}
