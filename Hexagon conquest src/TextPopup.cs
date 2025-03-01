using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TextPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text popupText;
    public string Text { set => popupText.text = value; }

    public void SetColor(bool isRed)
    {
        if (isRed)
        {
            popupText.color = Color.red;
        }
        else
        {
            popupText.color = Color.green;
        }
    }

    public void FadeOut()
    {
        InvokeRepeating("Fade", 0, 0.2f);
        Invoke("Delete", 3);
    }

    public void Fade()
    {
        popupText.alpha *= 0.8f;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

}
