using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
//    Rigidbody2D body;

    public Vector2 velocity;
    public Vector2 velocityY;

    public float speed = 5.0f;

    public float groundDeceleration = 0.5f;

    public float walkAcceleration = 0.5f;

    public Rigidbody2D RB;

    public Camera PlayerCam;

    public Vector3 Offset;

    public Transform Gun;

    public SpriteRenderer GunSprite;
    
    public GameObject SelectCircle;
    [FormerlySerializedAs("GunPosition")] public GameObject BulletSpawnPos;
    public GameObject BulletPrefab;
    public float BulletSpeed = 5.0f;//spawn bullets
    public int BulletsPerSecond = 3;     //how many bullets per seconds
    public float TimerForBullets = 0f;
    public bool CanShoot = true;  
//    float horizontal;
//    float vertical;
//    
//    public float runSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Offset = PlayerCam.transform.position;
        // CameraFollow.Setup(new Vector3(0, -100));
    }

    // Update is called once per frame
  
    
    void Update()
    {
        TimerForBullets += Time.deltaTime;

        Debug.Log(Gun.transform.rotation.z);
        //rotate player gun
        TurnPlayer();
        //check if player shoots with left click of mouse
        if (Input.GetMouseButton(0))
        {
            PlayerShoots();
        }
        
        
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
        //RB.velocity = (velocity * Time.deltaTime * 100f);
        //transform.Translate(velocity*Time.deltaTime);



//        horizontal = Input.GetAxisRaw("Horizontal");
//        vertical = Input.GetAxisRaw("Vertical"); 
    }

    void TurnPlayer()
    {
        Vector3 mousePos = Input.mousePosition - Offset; //direction of mouse
        Vector3 difference = PlayerCam.ScreenToWorldPoint(mousePos) - transform.position;
        Vector3 direction = difference.normalized * 10f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position += direction;
        }
        if (Gun.transform.rotation.z < 0)
        {
            
            GunSprite.flipY = false;
        }
        else
        {
            GunSprite.flipY = true;
        }
        SelectCircle.transform.up = (difference).normalized; //rotate player

    
    }
    private void FixedUpdate()
    {
       // body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
    void PlayerShoots()
    {
        //If available ammo and timer cooldown
        if (/*PlayerInven.AmmoCount > 0 && */ TimerForBullets >= 1.0 / BulletsPerSecond)
        {
            CanShoot = true;
        }
        else
        {
            CanShoot = false;
        }

        //Check if player inputs shoot
        int leftMouseButton = 0;
        if (Input.GetMouseButton(leftMouseButton) && CanShoot)
        {
            //Use one ammo from inventory
            //PlayerInven.AmmoCount--;

            //Spawn bullet
            GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPos.transform.position, SelectCircle.transform.rotation);
            
            //Set velocity of bullet
            bullet.GetComponent<Rigidbody2D>().velocity = SelectCircle.transform.up * BulletSpeed;

            //reset timer for bullets
            TimerForBullets = 0f;

            //play sound
            //AM.PlaySound("shoot");
        }
    }
    
    
}
