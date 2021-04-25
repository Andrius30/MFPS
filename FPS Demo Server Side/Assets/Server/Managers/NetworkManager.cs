using MFPS.ServerCharacters;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public static Dictionary<int, Player> players = new Dictionary<int, Player>();

    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<Transform> playerSpawnPositions = new List<Transform>();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 50; // test on task manager performance

        Server.StartServer(50, 55555);
    }

    void OnApplicationQuit() => Server.Stop();

    public Player InstantiatePlayer() => Instantiate(playerPrefab, GetRandomSpawnPosition().position, Quaternion.identity).GetComponent<Player>();

    public Transform GetRandomSpawnPosition() => playerSpawnPositions[Random.Range(0, playerSpawnPositions.Count)];
}
