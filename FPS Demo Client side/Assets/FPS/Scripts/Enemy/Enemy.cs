using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxhealth;

    public void Die()
    {
        if (maxhealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
