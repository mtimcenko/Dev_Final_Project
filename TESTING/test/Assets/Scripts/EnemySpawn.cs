using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public int height;
    public int width;
    public GameObject enemyPrefab;

    public void spawn
    {
        val position = new Vector2(Random.Range(width, width + 5), Random.Range(height, height + 5))
        Instantiate(enemyPrefab, position, Quaternion.identity)
    }
}
