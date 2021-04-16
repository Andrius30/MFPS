using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();

    [SerializeField] GameObject localPlayerPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject sceneCamera;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation)
    {
        PlayerManager playerManager;

        if (id == Client.instance.id) // local player
            CreatePlayer(id, userName, position, rotation, out playerManager, true);
        else // remote player
            CreatePlayer(id, userName, position, rotation, out playerManager, false);

        if (!players.ContainsKey(id))
            players.Add(id, playerManager);

        sceneCamera.SetActive(false);
    }

    void CreatePlayer(int id, string userName, Vector3 position, Quaternion rotation, out PlayerManager playerManager, bool isLocal)
    {
        GameObject player;
        if (isLocal)
            player = Instantiate(localPlayerPrefab, position, rotation);
        else
            player = Instantiate(playerPrefab, position, rotation);
        playerManager = player.GetComponent<PlayerManager>();
        playerManager.Initialize(id, userName, isLocal);
    }

    public void SpawnProjectile(int id, Vector3 position, Quaternion rot)
    {
        GameObject gm = MonoBehaviour.Instantiate(projectilePrefab, position, rot);
        Projectile projectile = gm.GetComponent<Projectile>();
        projectiles.Add(id, projectile);
    }
    public void SpawnEnemy(int id, Vector3 position, Quaternion rotation)
    {
        if (!enemies.ContainsKey(id))
        {
            GameObject gm = Instantiate(enemyPrefab, position, rotation);
            Enemy enemy = gm.GetComponent<Enemy>();
            enemies.Add(id, enemy);
        }
    }
}
