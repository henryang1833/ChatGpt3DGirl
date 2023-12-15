import socket
import time
from pydub import AudioSegment
from openai import OpenAI
from pathlib import Path
import requests
import os

# 创建一个带有代理的 requests 会话
session = requests.Session()
session.proxies = {
    "http": "http://127.0.0.1:10809",
    "https": "http://127.0.0.1:10809",
}

# 如果代理需要认证
# session.proxies = {
#     "http": "http://username:password@your_proxy_address:port",
#     "https": "http://username:password@your_proxy_address:port",
# }

# 设置环境变量
os.environ['http_proxy'] = 'http://127.0.0.1:10809'
os.environ['https_proxy'] = 'http://127.0.0.1:10809'

client = OpenAI(api_key="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",)

# 配置 OpenAI 使用这个会话
client.session = session

# 创建服务器套接字
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 绑定服务器地址和端口
server_address = ('192.168.0.25', 8888)  # 使用0.0.0.0表示监听所有网络接口
server_socket.bind(server_address)

# 开始监听客户端连接
server_socket.listen(5)  # 允许最多5个等待连接的客户端



def Whisper(filePath):
    audio_file = open(filePath, "rb")
    transcript = client.audio.transcriptions.create(model="whisper-1", file=audio_file)
    return transcript.text

def ChatCompletion(prompt):
    completion = client.chat.completions.create(model="gpt-3.5-turbo",messages=[{"role": "system", "content": "你是一个性格幽默活泼、喜欢逗趣的中国姑娘，以此你能说地道的汉语，并且喜欢玩梗。"},{"role": "user", "content": prompt}])
    return completion.choices[0].message.content


def convert_mpga_to_wav(mpga_path):
    # 加载MPGA文件
    audio = AudioSegment.from_file(mpga_path)

    wav_path = Path(__file__).parent / "speech.wav"
    # 导出为WAV格式
    audio.export(wav_path, format="wav")

    return wav_path

def CreateSpeech(inputText):
    speech_file_path = Path(__file__).parent / "speech.mpga"
    response = client.audio.speech.create(model="tts-1",voice="nova",input=inputText)
    response.stream_to_file(speech_file_path)
    wav_path = convert_mpga_to_wav(speech_file_path)
    return wav_path





print("Waiting for client connection...")

while True:
    # 等待客户端连接
    client_socket, client_address = server_socket.accept()

    print(f"Accepted connection from {client_address}")

    # 接收客户端发送的数据
    data = client_socket.recv(100*1024*1024)  # 接收最多100M的数据
    if data:
        data = data.decode("utf-8")

        print("Received data: ",len(data))
        
        print(data)

        # with open('./temp.wav', 'wb') as file:
        #     file.write(data)

        # currentTime = time.time()
        # #文字转语音
        # prompt = Whisper('./temp.wav')
        # print("语音识别结果：",prompt)
        # print("耗时(s)：",time.time()-currentTime)
        
        
        
        #与GPT进行交流
        currentTime = time.time()
        chatgptResponse = ChatCompletion(data)
        print("chatgpt回复：",chatgptResponse)
        print("耗时(s)：",time.time()-currentTime)

        # currentTime = time.time()
        # #合成语音
        # speech_file_path = CreateSpeech(chatgptResponse)
        # print("语音合成路径：",speech_file_path)
        # print("耗时(s)：",time.time()-currentTime)

        # with open(speech_file_path, 'rb') as file:
        #     # 使用read()方法读取整个文件内容
        #     file_data = file.read()
        #     totalLength = len(file_data)
        #     client_socket.send(totalLength.to_bytes(4,byteorder='big'))
        #     # 向客户端发送响应数据
        #     sendLen = client_socket.send(file_data)
        #     print("数据发送成功,共%d字节数据，发送了%d字节，还剩%d字节"%(totalLength,sendLen,totalLength-sendLen))
        
        totalLength = len(chatgptResponse)
        client_socket.send(totalLength.to_bytes(4,byteorder='big'))
        sendLen = client_socket.send(chatgptResponse.encode())
    # 关闭与客户端的连接
    #client_socket.close()


