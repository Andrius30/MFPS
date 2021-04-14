using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] int numberTospawn = 1;

    void Start() => SpawnEnemies();
    void SpawnEnemies()
    {
        for (int i = 0; i < numberTospawn; i++)
        {
            int randPos = Random.Range(0, spawnPositions.Count);
            var enemy = Instantiate(enemyPrefab, spawnPositions[randPos].position, Quaternion.identity).GetComponent<Enemy>();
            enemy.id++;
            enemies.Add(enemy.id, enemy);
            PacketsToSend.SpawnEnemy(enemy);
        }
    }
}
