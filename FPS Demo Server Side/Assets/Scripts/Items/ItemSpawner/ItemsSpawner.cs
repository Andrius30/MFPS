using System.Collections.Generic;
using UnityEngine;

public class ItemsSpawner : MonoBehaviour
{
    public static Dictionary<int, Item> itemsDict = new Dictionary<int, Item>();
    [SerializeField] List<Transform> itemPositions = new List<Transform>();
    [SerializeField] List<Item> serverItems = new List<Item>();
    [SerializeField] int itemsToSpawn = 5;

    void Start() => SpawnItems();

    void SpawnItems()
    {
        for (int i = 0; i < itemsToSpawn; i++)
        {
            Transform itemPosition = RandomPosition();
            ItemPosition pos = itemPosition.GetComponent<ItemPosition>();
            if (pos.DoesHaveItem())
            {
                i--;
                continue;
            }

            pos.SetItemPosition(itemPosition.position);
            pos.SethaveItem(true);
            GameObject rand = RandomItem();
            GameObject item = Instantiate(rand, pos.GetPosition(), rand.transform.rotation);
            Item it = item.GetComponent<Item>();
            it.itemID = i;
            itemsDict.Add(i, it);
        }
    }

    GameObject RandomItem() => serverItems[Random.Range(0, serverItems.Count)].gameObject;
    Transform RandomPosition() => itemPositions[Random.Range(0, itemPositions.Count)];
}
