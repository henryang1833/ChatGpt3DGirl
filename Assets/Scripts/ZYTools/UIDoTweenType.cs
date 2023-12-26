using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDoTweenType : SingletonAuto<UIDoTweenType>
{
    delegate Text GetTextComponent();
    GetTextComponent getTextComponent;
    public float FontSize{
        get {
            Text text = getTextComponent();
            return text.fontSize;
        }
        set {
            Text text = getTextComponent();
            text.fontSize = (int)value;
        }
    }
    public void GameObjectDoScaleShow(GameObject go, float speedTime = 0.25f,UnityAction action=null)
    {
        go.SetActive(true);
        Vector3 max = new Vector3(1, 1, 1);
        go.transform.DOScale(max, speedTime).OnComplete(() =>
        {
            if (action != null)
            {
                action.Invoke();
            }

        });
        Text text = go.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 1;
            DOTween.To(() => text.fontSize, x => text.fontSize = x, 50, 0.25f);
        }

    }
    public void GameObjectDoScaleHide(GameObject go, float speedTime = 0.25f, UnityAction action = null)
    {
        Vector3 max = new Vector3(0, 0, 0);
        go.transform.DOScale(max, speedTime).OnComplete(() =>
        {
            if (action != null)
            {
                action.Invoke();
            }
            go.SetActive(false);
        });
    }
}
