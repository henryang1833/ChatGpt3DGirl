import asyncio
import socket
import json
from openai import OpenAI
import os
import aiohttp

# 设置环境变量
os.environ["http_proxy"] = "http://127.0.0.1:10809"
os.environ["https_proxy"] = "http://127.0.0.1:10809"

client = OpenAI(api_key="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")

async def handle_client(reader, writer):
    client_address = writer.get_extra_info('peername')
    print(f"Accepted connection from {client_address}")

    data = await reader.read(100 * 1024 * 1024)  # 接收最多100M的数据
    if data:
        data = data.decode("utf-8")
        print("Received data: ", len(data))

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

            for chunk in stream:
                chunkStr = chunk.choices[0].delta.content
                if chunkStr is not None:
                    writer.write(chunkStr.encode())
                    await writer.drain()
                    print("发送了:{}".format(chunkStr))
                else:
                    writer.write("END_OF_STREAM".encode())
                    await writer.drain()
                    print("发送了:END_OF_STREAM")
        except Exception as e:
            print(e)
            if e.code == "rate_limit_exceeded":
                writer.write("您的访问频率太快了，请稍后再试！".encode())
                await writer.drain()
                print("发送了:您的访问频率太快了，请稍后再试！")
                writer.write("END_OF_STREAM".encode())
                await writer.drain()
                print("发送了:END_OF_STREAM")
    
    writer.close()

async def main():
    server = await asyncio.start_server(
        handle_client, '0.0.0.0', 8888)

    addr = server.sockets[0].getsockname()
    print(f'Serving on {addr}')

    async with server:
        await server.serve_forever()





async def send_openai_request_streaming(data, client_socket):
    api_key = "你的API密钥"
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json"
    }

    async with aiohttp.ClientSession() as session:
        async with session.post("https://api.openai.com/v1/chat/completions", headers=headers, data=json.dumps(data)) as response:
            if response.status == 200:
                async for chunk in response.content.iter_chunked(1024):
                    # 假设每个 chunk 是完整的 JSON 消息
                    chunk_str = chunk.decode('utf-8')
                    client_socket.send(chunk_str.encode())
                    print("发送了: {}".format(chunk_str))
            else:
                # 处理错误情况
                print(f"错误: {response.status} - {await response.text()}")
                if response.status == 429:
                    client_socket.send("您的访问频率太快了，请稍后再试！".encode())
                    print("发送了: 您的访问频率太快了，请稍后再试！")
                    client_socket.send("END_OF_STREAM".encode())
                    print("发送了: END_OF_STREAM")

# 准备请求数据
data = {
    "model": "gpt-3.5-turbo",
    "messages": [
        {
            "role": "system",
            "content": "You are a good assistant."
        },
        {
            "role": "user",
            "content": "YOUR_USER_MESSAGE"
        }
    ]
}

# 使用
# asyncio.run(send_openai_request_streaming(data, client_socket))


asyncio.run(main())
