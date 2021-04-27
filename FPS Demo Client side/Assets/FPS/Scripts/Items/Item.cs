using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    NONE,
    Health,
    Mine
}

public abstract class Item : MonoBehaviour
{
    public ItemType itemType = ItemType.NONE;
    public int itemID;

    public List<MeshRenderer> models = new List<MeshRenderer>(); // models to visualy destroy untill sounds and other effects in progress

    [Space(10)]
    [Header("Item effects")]
    public GameObject item_VFX;
    public float item_VFX_DestroyTime = 5f;
    public AudioClip[] item_SFX_clips;

    public AudioSource itemSource;

    public abstract void Execute(PlayerManager player);
    protected virtual void PlayVFX(PlayerManager player = null)
    {
        GameObject gm = Instantiate(item_VFX, transform.position, transform.rotation);
        Destroy(gm, item_VFX_DestroyTime);
    }
    protected virtual void PlaySFX()
    {
        itemSource.PlayOneShot(RandomClip(item_SFX_clips));
    }
    protected virtual AudioClip RandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];

    //protected virtual void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Characters"))
    //    {
    //        Execute(other);
    //    }
    //}
}
