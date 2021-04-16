using FPSClient.Timers;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DirectionalDamageIndicator : MonoBehaviour
{
    public Action unregister;
    public float fadeValue = 0.3f;
    public float fadeOutAfterTime = 3f;

    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public Transform target;
    public Transform player;

    Vector3 pos;
    Quaternion rot;
    Timer fadeOutTimer;

    void Awake()
    {
        fadeOutTimer = new Timer(fadeOutAfterTime, false);
        player = transform.root;
    }

    public void Register(Transform target, Transform player, Action unregister)
    {
        this.target = target;
        this.player = player;
        this.unregister = unregister;

        StartCoroutine(RotateToTarget());
    }

    IEnumerator RotateToTarget()
    {
        while (enabled)
        {
            if (target != null)
            {
                pos = target.position;
                rot = target.rotation;
                Vector3 dir = player.position - target.position;

                rot = Quaternion.LookRotation(dir);
                rot.z = -rot.y;
                rot.x = 0;
                rot.y = 0;

                Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
                rectTransform.localRotation = rot * Quaternion.Euler(northDirection);
            }

            yield return null;
        }
    }

    void Update()
    {
        fadeOutTimer.StartTimer();
        if (fadeOutTimer.IsDone())
            FadeOut();
    }
    public void Restart() => fadeOutTimer.SetTimer(fadeOutAfterTime, false);
    void FadeOut()
    {
        if (canvasGroup.alpha <= 0)
        {
            canvasGroup.alpha = 0;
            Destroy(gameObject);
            unregister?.Invoke();
            return;
        }
        canvasGroup.alpha -= fadeValue * Time.deltaTime;
    }

}
