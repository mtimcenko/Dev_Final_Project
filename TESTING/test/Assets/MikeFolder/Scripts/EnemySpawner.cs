using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Vector2 SpawnBoundaries = new Vector2(41f, 24f);
    public Vector2 OutBoundaries = new Vector2(61f, 61f);
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
            SpawnEnemyBeyondPlayer();
        }
        
        
        
    }
    
    void SpawnEnemyOnEdge()
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

    Vector2 RandPosOnBoundaryEdge(Vector2 centralPos, Vector2 boundary)
    {
        Vector2 spawnLoc;
        //50 50 chance
        if (Random.Range(0, 2) == 0)
        {
            //right side
            if (Random.Range(0, 2) == 0)
            {
                spawnLoc = new Vector2(centralPos.x + boundary.x, Random.Range(centralPos.y - boundary.y, centralPos.y + boundary.y));
                
            }
            else
            {
                spawnLoc = new Vector2(centralPos.x - boundary.x, Random.Range(centralPos.y - boundary.y, centralPos.y + boundary.y));
            }
        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                spawnLoc = new Vector2(Random.Range(centralPos.x - boundary.x, centralPos.x + boundary.x), centralPos.x + boundary.y);
            }
            else
            {
                spawnLoc = new Vector2(Random.Range(centralPos.x - boundary.x, centralPos.x + boundary.x), centralPos.x - boundary.y);
            }
        }
        return spawnLoc;
    }

    bool Vector2WithinBounds(Vector2 position, Vector2 outBounds)
    {
        return position.x < outBounds.x && position.x > -outBounds.x &&
               position.y < outBounds.y
               && position.y > -outBounds.y;
    }
    
    void SpawnEnemyBeyondPlayer()
    {
        Vector3 spawnLoc;
        do
        {
            spawnLoc = RandPosOnBoundaryEdge(transform.position, SpawnBoundaries);
        } while (!Vector2WithinBounds(spawnLoc, OutBoundaries));
        Instantiate(EnemyPrefab, spawnLoc, Quaternion.identity);
    }


    
}
