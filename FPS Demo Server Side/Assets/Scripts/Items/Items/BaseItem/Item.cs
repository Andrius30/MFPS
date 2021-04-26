using UnityEngine;

public enum ItemType
{
    NONE,
    Health,
    Mine
}

public abstract class Item : MonoBehaviour
{
    [SerializeField] ItemType itemType = ItemType.NONE;
    public int itemID;

    public ItemType ItemType => itemType;

    protected abstract void Execute(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other)
            Execute(other);
    }
}
