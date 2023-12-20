using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyMinnow.SALSA;
using DG.Tweening;
public class AlphaGrilMove : MonoBehaviour
{
    public Animator animator;
    public Salsa3D salsa3D;

    private void Start()
    {
        animator = transform.GetComponent<Animator>();
        salsa3D = transform.GetComponentInChildren<Salsa3D>();
        Debug.Log(salsa3D.name);
        //salsa3D.audioSrc=AudioManager.Instance.GetOtherAudioSource();
        Debug.Log("111");
    }

    public void PlayTalkAnima(bool isPlay)
    {
        animator.SetBool("IsTalk", isPlay);
    }

    public void PlayTalk(AudioClip clip)
    {
        salsa3D.audioSrc = AudioManager.Instance.OtherAudioSource;
        if (salsa3D.audioClip != null)
        {
            salsa3D.audioClip = null;
        }
        salsa3D.audioClip = clip;
        salsa3D.audioSrc.clip = salsa3D.audioClip;
        salsa3D.Play();
    }

    public void StopTalk()
    {
        salsa3D.Stop();
    }

    public void AlphaGirlNearScence()
    {
        Vector3 vector3 = new Vector3(-4.35f, 0.56f, -4.07f);
        GameObject camera = GameObject.Find("Camera");
        camera.transform.DOMove(vector3, 0.5f);
        
    }

    public void AlphaGirlNormalScence()
    {
        Vector3 vector3 = new Vector3(-2.68f, -0.3f, -5.48f);
        GameObject camera = GameObject.Find("Camera");
        camera.transform.DOMove(vector3, 0.5f);
    }


    public void AlphaGirlRotate(float dealtRotateY)
    {
        transform.localEulerAngles +=  new Vector3(0,dealtRotateY,0);
    }
}
