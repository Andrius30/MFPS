using UnityEngine;

public class HealthItem : Item
{
    [SerializeField] Vector3 effectOffset; // Y: -0.809 Z: 0.366

    public override void Execute(PlayerManager player)
    {
        PlaySFX();
        PlayVFX(player);
        models.ForEach(x => x.enabled = false);
        Destroy(gameObject, item_VFX_DestroyTime);
    }


    protected override void PlayVFX(PlayerManager player)
    {
        GameObject gm = Instantiate(item_VFX, player.transform);
        gm.transform.localPosition = effectOffset;

        GameManager.items.Remove(itemID);
        Destroy(gm, item_VFX_DestroyTime);
    }
}
