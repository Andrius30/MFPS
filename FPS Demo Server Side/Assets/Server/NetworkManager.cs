using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

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

    public Player InstantiatePlayer() => Instantiate(playerPrefab, new Vector3(0, 1.5f, 0), Quaternion.identity).GetComponent<Player>();


}
