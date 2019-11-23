using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
//    Rigidbody2D body;

    public Vector2 velocity;
    public Vector2 velocityY;

    public float speed = 5.0f;

    public float groundDeceleration = 0.5f;

    public float walkAcceleration = 0.5f;
//    
//    float horizontal;
//    float vertical;
//    
//    public float runSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
       // CameraFollow.Setup(new Vector3(0, -100));
    }

    // Update is called once per frame
  
    
    void Update()
    {
        
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
        }

        float moveInputVertical = Input.GetAxis("Vertical");

        if (moveInputVertical != 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInputVertical, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.y = Mathf.MoveTowards(velocity.y, 0, groundDeceleration * Time.deltaTime);
        }
        
        transform.Translate(velocity * Time.deltaTime);
       //transform.Translate(velocity*Time.deltaTime);
        
        

//        horizontal = Input.GetAxisRaw("Horizontal");
//        vertical = Input.GetAxisRaw("Vertical"); 
    }

    private void FixedUpdate()
    {
       // body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
