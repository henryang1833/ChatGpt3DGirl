using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestOfflineVoiceRec : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AsrData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 识别本地文件
    public void AsrData()
    {
        // 设置APPID/AK/SK
        var APP_ID = "S84966";
        var API_KEY = "ijOO7KGpogkZYPfh6L5DK8rB";
        var SECRET_KEY = "Dj1rVAcjmGt5EN6xqxGGA8l2fc5fDTuH";

        var client = new Baidu.Aip.Speech.Asr(APP_ID, API_KEY, SECRET_KEY);
        client.Timeout = 60000;  // 修改超时时间
        string path = System.IO.Path.Combine(Application.dataPath, "阿尔法配音3.mp3");
        var data = System.IO.File.ReadAllBytes("C:\\Users\\Lenovo\\newSpeech.wav");

        // 可选参数
        var options = new Dictionary<string, object>
     {
        {"dev_pid", 1537}
     };
        client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
        var result = client.Recognize(data, "wav", 16000, options);
        Debug.Log(result);
    }
}
