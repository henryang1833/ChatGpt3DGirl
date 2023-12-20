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
        if (Input.GetMouseButton(0)&&mouseX!=0)
        {
            float dealtRatation = rotateSpeed * mouseX;
            mAlphaGrilMove.transform.Rotate(Vector3.up, -mouseX * rotateSpeed);
            Debug.Log($"dealtRatation:{dealtRatation}");
            Debug.Log($"Mouse X:{mouseX}");
        }
    }


}
