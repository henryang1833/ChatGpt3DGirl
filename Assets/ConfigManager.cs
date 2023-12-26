using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static ConfigManager instance;
    private ConfigManager() { }

    public UnityEngine.UI.Text ipText;
    public UnityEngine.UI.Text portText;
    public UnityEngine.UI.Text ipPlaceHolderText;
    public UnityEngine.UI.Text portPlaceHolderText;

    private void OnEnable()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "server.config");
        string ip = "192.168.0.25";
        int port = 8888;
        if (System.IO.File.Exists(path))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
            {
                ip = reader.ReadLine();
                port = int.Parse(reader.ReadLine());
            }
        ipPlaceHolderText.text = ip;
        portPlaceHolderText.text = port+"";
    }

    public static ConfigManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ConfigManager>();
            }
            return instance;
        }
    }
    
    public string Ip
    {
        get
        {
            return ipText.text == "" ? ipPlaceHolderText.text : ipText.text;
        }
    }

    public int Port
    {
        get
        {
            return portText.text == "" ? int.Parse(portPlaceHolderText.text) : int.Parse(portText.text);
        }
    } 

    
}
