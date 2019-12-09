using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public float despawnTimer = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ammoDespawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ammoDespawn()
    {
        yield return new WaitForSeconds(despawnTimer);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            Debug.Log("Picked up");
            Destroy(gameObject);
        }
    }
}
