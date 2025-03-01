using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text label;
    private float maxHealth;
    public float MaxHealth { set => maxHealth = value; }

    public void Initialize(float curHealth, float maxHealth)
    {
        fill.fillAmount = curHealth / maxHealth;
        label.text = curHealth + "/" + maxHealth;
    }

    public void ChangeFill(float curHealth)
    {
        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        fill.fillAmount = curHealth / maxHealth;
        label.text = curHealth + "/" + maxHealth;
    }
}
