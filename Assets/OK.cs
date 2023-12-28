using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OK : MonoBehaviour
{
    private GameObject talkCanvas;
    private GameObject configCanvas;
    private void Awake()
    {
        talkCanvas = GameObject.Find("TalkCanvas");
        configCanvas = GameObject.Find("ConfigCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveConfig() 
    {
        string ip = ConfigManager.Instance.Ip;
        int port = ConfigManager.Instance.Port;
        string path = System.IO.Path.Combine(Application.persistentDataPath, "server.config");
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path,false))
        {
            // 写入IP和端口号作为字符串
            writer.WriteLine(ip);
            writer.WriteLine(port.ToString());
        }
        Cancel();
    }

    public void Cancel()
    {
        configCanvas.SetActive(false);
        talkCanvas.SetActive(true);
        Camera camera = GameObject.FindObjectOfType<Camera>();
        camera.cullingMask = -1;
    }
}
