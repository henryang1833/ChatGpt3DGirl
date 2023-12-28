using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    private GameObject configCanvas;
    private GameObject talkCanvas;
    private void Awake()
    {
        configCanvas = GameObject.Find("ConfigCanvas");
        talkCanvas = GameObject.Find("TalkCanvas");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick() 
    {
        configCanvas.SetActive(true);
        talkCanvas.SetActive(false);
        Camera camera = GameObject.FindObjectOfType<Camera>();
        camera.cullingMask = LayerMask.GetMask("ConfigWindow");
    }
}
