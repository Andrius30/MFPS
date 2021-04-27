using MFPS.ServerCharacters;
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

    protected abstract void Execute(Player player);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                Execute(player);
                PacketsToSend.ExecuteItem(this, player);
            }
        }
    }
}
