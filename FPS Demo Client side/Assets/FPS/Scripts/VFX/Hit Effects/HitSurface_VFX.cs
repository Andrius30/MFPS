using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Hit effects")]
public class HitSurface_VFX : ScriptableObject
{
    public GameObject hit_Default_VFX; // if hit surface not specified
    public GameObject hit_Concrete_VFX;
    public GameObject hit_Dirt_VFX;
    public GameObject hit_Metal_VFX;
    public GameObject hit_Wood_VFX;
    public GameObject hit_Grass_VFX;
    public GameObject hit_Blood_VFX;
   
    // Character => blood

    public void CreateHitEffect(Vector3 pos, Quaternion rot, int surfaceType)
    {
        switch (surfaceType)
        {
            case (int)SurfaceTypes.Concrete:
                CreateEffect(pos, rot, hit_Concrete_VFX);
                break;
            case (int)SurfaceTypes.Dirt:
                CreateEffect(pos, rot, hit_Dirt_VFX);
                break;
            case (int)SurfaceTypes.Metal:
                CreateEffect(pos, rot, hit_Metal_VFX);
                break;
            case (int)SurfaceTypes.Wood:
                CreateEffect(pos, rot, hit_Wood_VFX);
                break;
            case (int)SurfaceTypes.Grass:
                CreateEffect(pos, rot, hit_Grass_VFX);
                break;
            case (int)SurfaceTypes.Character:
                CreateEffect(pos, rot, hit_Blood_VFX);
                break;
            default:
                CreateEffect(pos, rot, hit_Default_VFX);
                break;
        }

    }

    void CreateEffect(Vector3 pos, Quaternion rot, GameObject effect)
    {
        GameObject gm = Instantiate(effect, pos, rot);
        Destroy(gm, 5f);
    }
}
