using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaGirlAnswerUI : MonoBehaviour
{
    public Text alphaGirl_Text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void SetAlphaText(string _content = null)
    {
        if (_content == null||_content=="")
        {
            
            UIDoTweenType.Instance.GameObjectDoScaleHide(gameObject, 0.25f, () =>
            {
                //gameObject.SetActive(false);
                alphaGirl_Text.text = _content;
            });
        }
        else
        {
            alphaGirl_Text.text = _content;
            UIDoTweenType.Instance.GameObjectDoScaleShow(gameObject, 0.25f);
            //gameObject.SetActive(true);

        }
    }
}
