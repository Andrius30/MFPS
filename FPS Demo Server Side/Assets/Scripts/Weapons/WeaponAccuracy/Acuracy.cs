using MFPS.ServerTimers;
using System.Collections.Generic;
using UnityEngine;

public class Acuracy : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;

    [SerializeField] float lerpTime = 2f;
    [SerializeField] float timeToSwichBetweenIndex = 0.01f;
    [SerializeField] List<Vector3> offsets = new List<Vector3>();

    Timer timers;
    int index;
    float timer = 0;

    public void Initialize() => timers = new Timer(0, true);

    public void UpdateWeaponInacuracy(bool isShoot)
    {
        timers.StartTimer();

        if (isShoot)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
                timer = lerpTime;

            if (timers.IsDone())
            {
                index++;
                if (index > offsets.Count - 1)
                {
                    index = offsets.Count - 1;
                }
                timers.SetTimer(timeToSwichBetweenIndex, false);
            }

            Lerp();
        }
        else
        {
            timer -= Time.deltaTime * 5f;
            if (timer < 0)
                timer = 0;
            if (timers.IsDone())
            {
                index--;
                if (index <= 0)
                {
                    index = 0;
                    return;
                }
                timers.SetTimer(0.01f, false);
            }
            Lerp();
        }
    }

    void Lerp()
    {
        //Debug.Log($"List count {offsets.Count} index {index} :red;".Interpolate());
        float r = timer / lerpTime;
        Vector3 t = curve.Evaluate(r) * offsets[index];

        transform.localEulerAngles = Vector3.Lerp(transform.position, t, r);
    }
}
