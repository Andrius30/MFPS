using System.Collections.Generic;
using UnityEngine;

public class ItemsSpawner : MonoBehaviour
{
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    [SerializeField] List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] GameObject item;

    void Start()
    {
        SpawnItem();
    }

    public void SpawnItem()
    {
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject GmItem = Instantiate(item, spawnPositions[i].position, Quaternion.identity);
            Item it = GmItem.GetComponent<Item>();
            it.InitializeItem(i, spawnPositions[i].position, false);
            items.Add(i, it);
        }
    }
}
