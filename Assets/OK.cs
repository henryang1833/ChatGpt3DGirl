using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SaveConfig() 
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
    }

    void Cancel()
    {
        transform.parent.gameObject.SetActive(false);
        TalkPanelManager.Instance.TalkPanelUI.GetComponent<CanvasGroup>().interactable = true;
    }
}
