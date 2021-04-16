using System;
using UnityEngine;
using UnityEngine.UI;

public class BloodSplatter : MonoBehaviour
{
    public static Action<float> onDamage;
    [SerializeField] Image bloodimage;
    [SerializeField] float fadeOutTime;

    [SerializeField] Color color; // hide after testing

    public void Start()
    {
        color = bloodimage.color;
        color.a = 0;
        SetAlpha(color);
    }

    void Update() => SetToZeroAlpha();

    void Splash(float dmg)
    {
        if (color.a >= 1f)
        {
            color.a = 1;
            return;
        }
        color.a += dmg * Time.deltaTime;
        SetAlpha(color);
    }

    void SetToZeroAlpha()
    {
        if (color.a <= 0)
        {
            color.a = 0;
            return;
        }
        color.a -= Time.deltaTime * fadeOutTime;
        //Debug.Log($"Fade out {color.a}");
        SetAlpha(color);
    }
    void SetAlpha(Color color) => bloodimage.color = color;

    void OnEnable() => onDamage += Splash;
    void OnDisable() => onDamage -= Splash;
}
