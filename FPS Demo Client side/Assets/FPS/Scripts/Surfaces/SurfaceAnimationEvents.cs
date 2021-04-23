using System;
using UnityEngine;

public class SurfaceAnimationEvents : MonoBehaviour
{
    public static Action onFoostepPlay;
    public static Action onJumpPlay;
    public static Action onLandPlay;

    public void OnFootstep() => onFoostepPlay?.Invoke();
    public void OnJump() => onJumpPlay?.Invoke();
    public void OnLand() => onLandPlay?.Invoke();
}
