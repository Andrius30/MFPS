using System;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Action<int, Vector3, bool> onItemSpawned;

    public GameObject item;

    void Awake()
    {
        onItemSpawned += SpawnItem;    
    }

    public void SpawnItem(int id, Vector3 position, bool picked)
    {
        GameObject gm = Instantiate(item, position, Quaternion.identity);
        Item it = gm.GetComponent<Item>();
        it.Initialize(id, picked);
    }
}
