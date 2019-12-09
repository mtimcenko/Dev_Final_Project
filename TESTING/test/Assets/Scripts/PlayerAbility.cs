using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public PlayerController pMovement;
    public Transform PlayerTransform;
    public Vector2 dashPos;

   
    public float speedInc = 10f;
    public float accelInc = 5f;
    public float groundInc = 0f;

    public Vector2 Boundaries;
    // Start is called before the first frame update
    void Start()
    {
        pMovement = GetComponent<PlayerController>();
        PlayerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        mDash();
    }

    void mDash()
    {
        //Press space to move toward mouse position
/*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dashPos = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if ((Vector2) transform.position != dashPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashPos,
                pMovement.walkAcceleration * Time.deltaTime + 10);
            
        }
*/
    /*    if (Input.GetKey(KeyCode.Space))
        {
            var dashPos = Input.mousePosition;
            dashPos.z = transform.position.z - Camera.main.transform.position.z;
            dashPos = Camera.main.ScreenToWorldPoint(dashPos);
            transform.position = Vector2.MoveTowards(transform.position, dashPos,
                10f * Time.deltaTime);
        }
*/

    //Increasing speed on spacebar
        if (transform.position.x < Boundaries.x && transform.position.x > -Boundaries.x &&
            transform.position.y < Boundaries.y
            && transform.position.y > -Boundaries.y)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                pMovement.speed += speedInc;
                pMovement.walkAcceleration += accelInc;
                pMovement.groundDeceleration += groundInc;
    
            
            }
    
            //Reset speed 
            
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            pMovement.speed -= speedInc;
            pMovement.walkAcceleration -= accelInc;
            pMovement.groundDeceleration -= groundInc;
        }
    

    }
}



    