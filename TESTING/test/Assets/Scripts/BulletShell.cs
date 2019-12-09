using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public Vector2 End;
    public Vector2 Origin;
    
    public float lerpMultiplier;
    public float DestroyTime = 10f;
    public Transform GunLoc;
    // Start is called before the first frame update
    void Start()
    {
        Origin = GameObject.Find("RealGun").transform.position;
        End = GameObject.Find("Direction").transform.position;
        StartCoroutine(DestroyDelay());
        //GetComponent<Rigidbody2D>().AddForce(Direction.transform.position-OriginPlayer.transform.position, ForceMode2D.Force);
    }

    // Update is called once per frame
    void Update()
    {
        //Origin = GameObject.Find("PlayerLight").transform.position;
        //End = GameObject.Find("Direction").transform.position;
        print(Time.deltaTime);
        //transform.position = Vector2.Lerp(Origin, End, lerpMultiplier*Time.deltaTime);
    }

    IEnumerator DestroyDelay()
    {
        
        yield return new WaitForSeconds(DestroyTime);
        Destroy(gameObject);
    }

}    
