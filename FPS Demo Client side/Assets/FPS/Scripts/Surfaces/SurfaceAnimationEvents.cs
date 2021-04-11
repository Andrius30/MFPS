using System;
using UnityEngine;

public class SurfaceAnimationEvents : MonoBehaviour
{
    public static Action onFoostepPlay;

    public void OnFootstep() => onFoostepPlay?.Invoke();
}
