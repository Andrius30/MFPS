using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID;
    public Vector3 position;
    public bool itemPickedUp;
    
    BoxCollider boxCollider;
    MeshRenderer renderer;

    public void InitializeItem(int id, Vector3 pos, bool picked)
    {
        itemID = id;
        position = pos;
        itemPickedUp = picked;

        renderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemPicked(other.GetComponent<Player>());
        }
    }

    IEnumerator RespawnItemRoutine()
    {
        yield return new WaitForSeconds(3f);
        ItemRespawned();
    }

    void ItemPicked(Player player)
    {
        if (player.CanPickMoreItems())
        {
            itemPickedUp = true;
            boxCollider.enabled = false;
            renderer.enabled = false;

            player.SetItems();
            StartCoroutine(RespawnItemRoutine());

            PacketsToSend.ItemPickedUp(player, this);
        }
    }
    void ItemRespawned()
    {
        itemPickedUp = false;
        boxCollider.enabled = true;
        renderer.enabled = true;

        PacketsToSend.ItemRespawned(this);
    }
}
