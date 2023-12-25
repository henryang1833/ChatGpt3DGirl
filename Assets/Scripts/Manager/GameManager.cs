using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private bool nearScence = false;
    public float doubleClickTime;
    private float lastClickTime = -1f;
    public float rotateSpeed;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
    public AlphaGrilMove mAlphaGrilMove;
    public SessionListManager mSessionListManager;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                if (nearScence)
                    mAlphaGrilMove.AlphaGirlNormalScence();
                else
                    mAlphaGrilMove.AlphaGirlNearScence();

                nearScence = !nearScence;
                lastClickTime = -1f;
            }
            else
            {
                lastClickTime = Time.time;
            }
        }

        float mouseX = Input.GetAxis("Mouse X");

#if UNITY_EDITOR
        mouseX = Input.GetAxis("Mouse X");
#elif UNITY_ANDROID
        // 在安卓上使用不同的方式来获取水平移动值
        // 比如使用触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            mouseX = touch.deltaPosition.x;
            rotateSpeed = 0.5f;
        }
        else
        {
            mouseX = 0;
        }
#endif

        if (Input.GetMouseButton(0)&&mouseX!=0)
        {
            float dealtRatation = rotateSpeed * mouseX;
            mAlphaGrilMove.transform.Rotate(Vector3.up, -mouseX * rotateSpeed);
            Debug.Log($"dealtRatation:{dealtRatation}");
            Debug.Log($"Mouse X:{mouseX}");
        }
    }


}
