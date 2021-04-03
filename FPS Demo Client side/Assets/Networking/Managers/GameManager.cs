using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    [SerializeField] GameObject localPlayerPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject sceneCamera;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == Client.instance.id)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        playerManager.Initialize(id, userName);

        if (!players.ContainsKey(id))
            players.Add(id, playerManager);

        sceneCamera.SetActive(false);
    }

    public void SpawnProjectile(int id, Vector3 position, Quaternion rot)
    {
        GameObject gm = MonoBehaviour.Instantiate(projectilePrefab, position, rot);
        Projectile projectile = gm.GetComponent<Projectile>();
        projectiles.Add(id, projectile);
    }

}
