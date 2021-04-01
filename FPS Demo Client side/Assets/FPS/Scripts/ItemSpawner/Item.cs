using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public static Action<int, bool> onItemPicked;
    public static Action<int, bool> onItemRespawned;

    public int id { get; set; }
    public bool picked { get; set; }
    MeshRenderer renderer;

    void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        onItemPicked += ItemPicked;
        onItemRespawned += ItemRespawned;
    }

    public void Initialize(int id, bool picked)
    {
        this.id = id;
        this.picked = picked;
    }
    public void ItemPicked(int id, bool picked)
    {
        if (this.id == id)
        {
            this.picked = picked;
            renderer.enabled = false;
        }
    }
    public void ItemRespawned(int id, bool picked)
    {
        if (this.id == id)
        {
            this.picked = picked;
            renderer.enabled = true;
        }
    }
}
