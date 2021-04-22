using UnityEngine;

public enum SurfaceTypes
{
    NONE,
    Concrete,
    Dirt,
    Metal,
    Wood,
    Grass,
    Character
}

public class Surface : MonoBehaviour
{
    [SerializeField] SurfaceTypes surfaceType = SurfaceTypes.NONE;

    public SurfaceTypes GetSurfaceType() => surfaceType;
}
