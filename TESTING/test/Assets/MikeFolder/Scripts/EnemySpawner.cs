using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public Vector2 OutsideBox = new Vector2(18f, 10f);

    public float SpawnTimer = 0f;

    public float SpawnEvery = .3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTimer += Time.deltaTime;
        if (SpawnTimer >= SpawnEvery)
        {
            SpawnTimer = 0f;
            SpawnEnemy();
        }
    }
    
    void SpawnEnemy()
    {
        Vector2 spawnLoc;
        //50 50 chance
        if (Random.Range(0, 2) == 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                spawnLoc = new Vector2(OutsideBox.x, Random.Range(-OutsideBox.y, OutsideBox.y));
                
            }
            else
            {
                spawnLoc = new Vector2(OutsideBox.x, Random.Range(-OutsideBox.y, OutsideBox.y));
            }
        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                spawnLoc = new Vector2(Random.Range(-OutsideBox.x, OutsideBox.x), OutsideBox.y);
            }
            else
            {
                spawnLoc = new Vector2(Random.Range(-OutsideBox.x, OutsideBox.x), -OutsideBox.y);
            }
        }
        Instantiate(EnemyPrefab, spawnLoc, Quaternion.identity);
    }
}
