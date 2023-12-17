from openai import OpenAI
import os

# 设置环境变量
os.environ["http_proxy"] = "http://127.0.0.1:10809"
os.environ["https_proxy"] = "http://127.0.0.1:10809"

client = OpenAI(api_key="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")

stream = client.chat.completions.create(
    model="gpt-3.5-turbo",
    messages=[
        {
            "role": "system",
            "content": "你是一个好助手。",
        },
        {"role": "user", "content": "如何看待元宇宙"},
    ],
    stream=True,
)
for chunk in stream:
    if chunk.choices[0].delta.content is not None:
        print(chunk.choices[0].delta.content, end="")
    else:
        print("None")
