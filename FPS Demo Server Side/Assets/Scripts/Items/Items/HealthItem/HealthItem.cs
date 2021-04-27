using MFPS.ServerCharacters;
using UnityEngine;

public class HealthItem : Item
{
    [SerializeField] float healthRestore = 50f;


    protected override void Execute(Player player)
    {
        player.SetHealth(healthRestore);
        ItemsSpawner.itemsDict.Remove(itemID);
        Destroy(gameObject);
    }
}
