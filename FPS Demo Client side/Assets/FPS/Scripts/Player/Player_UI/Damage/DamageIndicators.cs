using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicators : MonoBehaviour
{
    public static Action<Transform> onIndicatorCreated;
    public static Func<Transform, bool> isVisible;

    [SerializeField] DirectionalDamageIndicator damageIndicator;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] new Camera camera;
    [SerializeField] Transform player;

    Dictionary<Transform, DirectionalDamageIndicator> indicatorsDict = new Dictionary<Transform, DirectionalDamageIndicator>();


    void Create(Transform target)
    {
        if (target == null) return;
        if (indicatorsDict.ContainsKey(target))
        {
            indicatorsDict[target].Restart();
            return;
        }

        DirectionalDamageIndicator indicator = Instantiate(damageIndicator, rectTransform);
        indicator.Register(target, player, () => { indicatorsDict.Remove(target); });
        indicatorsDict.Add(target, indicator);
    }
    bool IsTargetVisible(Transform target)
    {
        if (target != null)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(target.position);
            return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        }
        return false;
    }
    void OnEnable()
    {
        onIndicatorCreated += Create;
        isVisible += IsTargetVisible;
    }
    void OnDisable()
    {
        onIndicatorCreated -= Create;
        isVisible -= IsTargetVisible;
    }
}
