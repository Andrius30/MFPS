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

    [Space(10)]
    [Header("Item effects")]
    public GameObject item_VFX;
    public float item_VFX_DestroyTime = 5f;
    public AudioClip[] item_SFX_clips;

    public AudioSource itemSource;

    protected abstract void Execute(Collider other);
    protected virtual void PlayVFX()
    {
        GameObject gm = Instantiate(item_VFX, transform.position, transform.rotation);
        Destroy(gm, item_VFX_DestroyTime);
    }
    protected virtual void PlaySFX()
    {
        itemSource.PlayOneShot(RandomClip(item_SFX_clips));
    }
    protected virtual AudioClip RandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Characters"))
        {
            Execute(other);
            PlayVFX();
            PlaySFX();
        }
    }
}
