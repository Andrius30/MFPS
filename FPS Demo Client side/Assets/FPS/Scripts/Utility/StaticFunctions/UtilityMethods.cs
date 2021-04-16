
using System;
using UnityEngine;

public class UtilityMethods : MonoBehaviour
{
    public static Func<Transform, string, Transform> onChildRequested;

    void OnEnable() => onChildRequested += FindChild;
    void OnDisable() => onChildRequested -= FindChild;

    public Transform FindChild(Transform parent, string childName)
    {
        if (parent != null)
        {
            foreach (Transform ch in parent.GetComponentsInChildren<Transform>(true))
            {
                if (parent.name != ch.name)
                {
                    if (ch.name == childName)
                        return ch;
                }
            }
        }
        return null;
    }
}
