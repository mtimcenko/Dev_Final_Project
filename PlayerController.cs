using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Vector2 velocity;

    public float speed = 5.0f;

    public float groundDeceleration = 0.5f;

    public float walkAcceleration = 0.5f;

    void Update()
    {
        
        float moveInput = Input.GetAxisRaw("Horizontal");
        //if the player is moving right or left
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        //if they're not inputting anything
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);

        }

        float moveInputVertical = Input.GetAxis("Vertical");
    //if they're moving vertically
        if (moveInputVertical != 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInputVertical, walkAcceleration * Time.deltaTime);
        }
        //if they're not inputting anything
        else
        {

            velocity.y = Mathf.MoveTowards(velocity.y, 0, groundDeceleration * Time.deltaTime);
        }
        
        transform.Translate(velocity * Time.deltaTime);

    }

}
