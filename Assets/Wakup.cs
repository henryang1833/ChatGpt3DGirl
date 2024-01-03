using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pv.Unity;
using System;

public class Wakup : MonoBehaviour
{
    private const string ACCESS_KEY = "RRJakSM2SJlhDvHDTOmfze4qtKvwZ+iy0tEzOgDMdPnTGZRIkZaZ3g=="; // AccessKey obtained from Picovoice Console (https://console.picovoice.ai/)
    private bool _isProcessing;
    PorcupineManager _porcupineManager;
    private bool isError = false;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //_porcupineManager = PorcupineManager.FromBuiltInKeywords(ACCESS_KEY, _keywords, OnWakeWordDetected, processErrorCallback: ErrorCallback);
            _porcupineManager = PorcupineManager.FromKeywordPaths(ACCESS_KEY, new List<string> { System.IO.Path.Combine(Application.dataPath, "嘿-小圆_zh_windows_v3_0_0.ppn") },
                OnWakeWordDetected, modelPath: Application.dataPath + "/porcupinezh.pv");
        }
        catch (PorcupineInvalidArgumentException ex)
        {
            SetError(ex.Message);
        }
        catch (PorcupineActivationException)
        {
            SetError("AccessKey activation error");
        }
        catch (PorcupineActivationLimitException)
        {
            SetError("AccessKey reached its device limit");
        }
        catch (PorcupineActivationRefusedException)
        {
            SetError("AccessKey refused");
        }
        catch (PorcupineActivationThrottledException)
        {
            SetError("AccessKey has been throttled");
        }
        catch (PorcupineException ex)
        {
            SetError("PorcupineManager was unable to initialize: " + ex.Message);
        }
        TalkPanelManager.Instance.onStartRecording += StopProcessing;
        TalkPanelManager.Instance.onEndRecording += StartProcessing;
        StartProcessing();

    }

    private void ToggleProcessing()
    {
        if (!_isProcessing)
        {
            StartProcessing();
        }
        else
        {
            StopProcessing();
        }
    }

    private void StartProcessing()
    {
        _porcupineManager.Start();
        _isProcessing = true;
    }

    private void StopProcessing()
    {
        if (_porcupineManager == null)
        {
            return;
        }

        _porcupineManager.Stop();
        _isProcessing = false;
    }

    private void OnWakeWordDetected(int keywordIndex)
    {
        Debug.Log("语音唤醒成功！");
        if (isError)
        {
            return;
        }

        if (keywordIndex >= 0)
        {
            //todo
            TalkPanelManager.Instance.OnRecvSentence("我在，有什么需要帮助的吗？");
            if (TalkPanelManager.Instance.myCoroutine == null)
            {
                TalkPanelManager.Instance.myCoroutine = StartCoroutine(TalkPanelManager.Instance.AudioClipQueueWatcher());
            }
        }
    }

    private void ErrorCallback(Exception e)
    {
        SetError(e.Message);
    }

    private void SetError(string message)
    {
        isError = true;
        //todo
        StopProcessing();
    }
}
