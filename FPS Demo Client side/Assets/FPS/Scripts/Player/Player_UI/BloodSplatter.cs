using UnityEngine;
using UnityEngine.UI;

public class BloodSplatter
{
    Image bloodimage;
    float fadeOutTime;
    Color color;

    public BloodSplatter(Image bloodimage, float fadeOutTime)
    {
        this.bloodimage = bloodimage;
        this.fadeOutTime = fadeOutTime;
        color = bloodimage.color;
        color.a = 0;
        SetAlpha(color);
    }

    public void Splash(float dmg)
    {
        if (color.a >= 1f)
        {
            color.a = 1;
            return;
        }
        color.a += dmg * Time.deltaTime;
        SetAlpha(color);
    }

    void SetAlpha(Color color) => bloodimage.color = color;

    public void ReleaseAlpha()
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
}
