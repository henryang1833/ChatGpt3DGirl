import socket
import time
from openai import OpenAI
from pathlib import Path
import os

# 设置环境变量
os.environ["http_proxy"] = "http://127.0.0.1:10809"
os.environ["https_proxy"] = "http://127.0.0.1:10809"

client = OpenAI(api_key="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")

# 创建服务器套接字
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 绑定服务器地址和端口
server_address = ("0.0.0.0", 8888)  # 使用0.0.0.0表示监听所有网络接口
server_socket.bind(server_address)

# 开始监听客户端连接
server_socket.listen(5)  # 允许最多5个等待连接的客户端

print("Waiting for client connection...")

while True:
    # 等待客户端连接
    client_socket, client_address = server_socket.accept()

    print(f"Accepted connection from {client_address}")

    # 接收客户端发送的数据
    data = client_socket.recv(100 * 1024 * 1024)  # 接收最多100M的数据
    if data:
        data = data.decode("utf-8")
        print("Received data: ", len(data))
        print(data)

        # 与GPT进行交流
        currentTime = time.time()
        try:
            stream = client.chat.completions.create(
                model="gpt-3.5-turbo",
                messages=[
                    {
                        "role": "system",
                        "content": "You are a good assistant.",
                    },
                    {"role": "user", "content": data},
                ],
                stream=True,
            )

            # cutWords = ["。", "?", "？", "……", ".", "!", "！"]
            for chunk in stream:
                chunkStr = chunk.choices[0].delta.content
                if chunkStr is not None:
                    client_socket.send(chunkStr.encode())
                    print("发送了:{}".format(chunkStr))
                else:
                    client_socket.send("END_OF_STREAM".encode())
                    print("发送了:END_OF_STREAM")
        except Exception as e:
            print(e)
            if e.code == "rate_limit_exceeded":
                client_socket.send("您的访问频率太快了，请稍后再试！".encode())
                print("发送了:您的访问频率太快了，请稍后再试！")
                client_socket.send("END_OF_STREAM".encode())
                print("发送了:END_OF_STREAM")
            pass
