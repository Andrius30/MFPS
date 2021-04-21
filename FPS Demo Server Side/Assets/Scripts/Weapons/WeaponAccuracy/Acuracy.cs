using MFPS.ServerCharacters;
using MFPS.ServerTimers;
using System.Collections.Generic;
using UnityEngine;

public class Acuracy : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;

    [SerializeField] float lerpTime = 2f;
    [SerializeField] float timeToSwichBetweenIndex = 0.01f;
    [SerializeField] List<Vector3> offsets = new List<Vector3>();

    [Space(20)]
    [Header("Repeat random")]
    [SerializeField] bool doesRepeatEnabled = false;
    [SerializeField] int fromIndex;
    [SerializeField] int toIndex;

    Timer timers;
    int index;
    float timer = 0;

    public void Awake() => timers = new Timer(0, true);

    public void UpdateWeaponInacuracy(Player player, bool isShoot)
    {
        timers.StartTimer();

        if (isShoot)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime) timer = lerpTime;
            if (timers.IsDone())
            {
                if (doesRepeatEnabled) // use random spray from
                {
                    if (index < fromIndex)
                    {
                        index++;
                        Debug.Log($"Less :red:20;".Interpolate());
                    }
                    else
                    {
                        index = RandomSpray();
                        Debug.Log($"Random :red:20;".Interpolate());
                    }
                }
                else // normal spray from index 0, to offsets.Count - 1
                {
                    index++;
                }
                if (index > offsets.Count - 1)
                {
                    index = offsets.Count - 1;
                }
                timers.SetTimer(timeToSwichBetweenIndex, false);
                Lerp(player);
            }
        }
        else
        {
            timer -= Time.deltaTime * 5f;
            if (timer < 0) timer = 0;
            if (timers.IsDone())
            {
                index--;
                if (index <= 0)
                {
                    index = 0;
                    player.shootOrigin.localRotation = Quaternion.identity;
                    PacketsToSend.RotateWeaponCameraBySpray(player);
                    return;
                }
                timers.SetTimer(0.01f, false);
                Lerp(player);
            }
        }
    }

    void Lerp(Player player)
    {
        float r = timer / lerpTime;
        float evaluate = curve.Evaluate(r);
        Quaternion rot = Quaternion.Euler(evaluate * offsets[index].x, evaluate * offsets[index].y, 0);
        player.shootOrigin.localRotation = Quaternion.Lerp(player.shootOrigin.localRotation, rot, r);
        PacketsToSend.RotateWeaponCameraBySpray(player);
    }
    // jeigu pasiektas from index pradek parinkineti random nuo to index'o iki (iki)index'o

    int RandomSpray() => Random.Range(fromIndex, toIndex);
}
