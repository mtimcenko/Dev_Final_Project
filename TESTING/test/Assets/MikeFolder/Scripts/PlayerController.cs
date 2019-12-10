using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
//    Rigidbody2D body;
    public bool steppingX = false;
    public bool steppingY = false;
    public Vector2 velocity;
    public Vector2 velocityY;
    private bool running = true;
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
    
    public string ScoreText = "";
    public TMP_Text ScoreTextComponent;
    private int _HighScoreAmount = 0;
    public string HighScoreText = "HighScore:";
    public TMP_Text HighScoreTextComponent;
    public TMP_Text ammoComponent;
    
    private int _ScoreAmount = 0;


    public GameObject GrenadePrefab;
    public float GrenadeSpeed = 3.0f;

    public float SwordThrowSpeed = 15.0f;
    public float SwordRotation = 500.0f;
    public GameObject Sword;
    public GameObject SwordHolder;
    public BoxCollider2D SwordHitCol;
    public float SwordHitTime = .15f;
    public float ResetTimeAttack = .20f;
    
    private Rigidbody2D SwordRB;
    private BoxCollider2D SwordCol;
    private Animator SwordAnimator;
    
    
    public bool IsSwordAttached = true;
    public Vector2 SwordPosition = new Vector2(-1.4f, 9.8f);

    public int ammoCount = 20;
    public bool reloading = false;
    public float reloadTime = 2.0f;
    public Sprite reloadSprite;
    public Renderer reloadSR;

    public GameObject CrossHair;
    private SpriteRenderer crossHairSR;
    private Transform crossHairTransform;
    public Sprite crossHairSprite;

    public GameObject reloadGO;
    private Transform reloadTransform;
    
    public float AttackTimer = 0f;

    public AudioManager AM;
    public int ScoreAmount 
    {
        get
        {
            return _ScoreAmount;
        }
        set
        {
            _ScoreAmount = value;
            ScoreTextComponent.text = ScoreText + _ScoreAmount.ToString();
            if (_ScoreAmount > _HighScoreAmount)
            {
                HighScoreAmount = _ScoreAmount;
            }
        }
    }
    
    public int HighScoreAmount
    {
        get
        {
            return _HighScoreAmount;
        }
        set
        {
            _HighScoreAmount = value;
            HighScoreTextComponent.text = HighScoreText + _HighScoreAmount.ToString();
            PlayerPrefs.SetInt ("High Score", _HighScoreAmount);
        }
    }
    
//    float horizontal;
//    float vertical;
//    
//    public float runSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(zombieSounds());

        
        InvokeRepeating("zombieSounds", 0.001f, 10.0f);
        InvokeRepeating("zombieThroat", 0.001f, 5.0f);

        
        crossHairTransform = CrossHair.GetComponent<Transform>();
        reloadSR = reloadGO.GetComponent<SpriteRenderer>();
        reloadTransform = reloadGO.GetComponent<Transform>();
        crossHairSR = CrossHair.GetComponent<SpriteRenderer>();
        SwordCol = Sword.GetComponent<BoxCollider2D>();
        SwordRB = Sword.GetComponent<Rigidbody2D>();
        SwordAnimator = SwordHolder.GetComponent<Animator>();
        //Physics.IgnoreLayerCollision(10, 11, true);
        
        ScoreAmount = _ScoreAmount;
        HighScoreAmount = PlayerPrefs.GetInt("High Score");
        RB = GetComponent<Rigidbody2D>();
        Offset = PlayerCam.transform.position;
        
        // CameraFollow.Setup(new Vector3(0, -100));
    }

    // Update is called once per frame

    [FormerlySerializedAs("ah")] public bool PressedSpace = false;

    private static readonly int Swing = Animator.StringToHash("Swing");
    //private static readonly int IsSwinging = Animator.StringToHash("IsSwinging");
    //private static readonly int Swing = Animator.StringToHash("Swing");

    void Update()
    {
       
       

        if (reloading == false)
        {
            reloadSR.enabled = false;
          //  crossHairSR.sprite = crossHairSprite;
            ammoComponent.text = "Ammo: " + ammoCount;
        }
        else
        {
            reloadSR.GetComponent<Animator>().SetTrigger("reload");
            reloadSR.enabled = false;
           // crossHairSR.sprite = reloadSprite;
            ammoComponent.text = "Reloading!";
        }

        // Debug.Log(ammoCount);
        
        TimerForBullets += Time.deltaTime;
        //Debug.Log(Gun.transform.rotation.z);
        //rotate player gun
        TurnPlayer();
        //check if player shoots with left click of mouse
        if (Input.GetMouseButton(0) && reloading == false && ammoCount > 0)
        {
            
            PlayerShoots();
        }

        if (Input.GetMouseButton(0)&& ammoCount <= 0)
        {
            AudioManager.Instance.PlaySound("empty");
        }
        //RELOAD
        if (Input.GetKeyDown(KeyCode.R)|| Input.GetKey(KeyCode.RightShift) && reloading == false)
        {
           // Debug.Log("here?");
            playerReload();
        }
        
        //THROW SWORD
        if (Input.GetKeyUp(KeyCode.Space) && !PressedSpace && AttackTimer <= 0)
        {
            //ShootGrenade();
            PressedSpace = true;
            IsSwordAttached = false;
            ThrowSword();
        }
        else if (!IsSwordAttached)
        {
            CatchSword();
        }
        //if sword attached not thrown
        else
        {
            Sword.transform.localPosition = SwordPosition; //For weird bugs when colliding with wall
            //attack sword
            if (AttackTimer <= 0)
            {
                if (Input.GetMouseButton(1) && IsSwordAttached)
                {
                    SwordHolder.SetActive(true);
                    SwordAnimator.SetTrigger(Swing);
                    SwordHitCol.enabled = true;
                    StartCoroutine(SwordSwing(SwordHitTime));
                    AttackTimer = ResetTimeAttack;
                }
            }
            else
            {
                AttackTimer -= Time.deltaTime;
            }
            
            
        }

        if (!IsSwordAttached)
        {
          //  Debug.Log("chillin here");
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
       // Debug.Log(moveInput);

        if (moveInput == -1)
        {
            steppingX = true;
            

        }
        
        if (moveInput == 1)
        {
            steppingX = true;
          
        }
        

            if (moveInput != 0)
            {
              //  AudioManager.Instance.PlaySound("step");
                velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
            }
            else
            {
                steppingX = false;
              // AudioManager.Instance.PlaySound("step");
                velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
            }

            
            
         

            float moveInputVertical = Input.GetAxis("Vertical");
            
            if (moveInputVertical == -1)
            {
                steppingY = true;
            

            }
        
            if (moveInputVertical == 1)
            {
                steppingY = true;
          
            }


            if (moveInputVertical != 0)
            {
               // AudioManager.Instance.PlaySound("step");
                
                velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInputVertical,
                    walkAcceleration * Time.deltaTime);
            }
            else
            {
                steppingY = false;
               // AudioManager.Instance.PlaySound("step");
                velocity.y = Mathf.MoveTowards(velocity.y, 0, groundDeceleration * Time.deltaTime);
            }
            //AudioManager.Instance.PlaySound("step");
            transform.Translate(velocity * Time.deltaTime);
            //RB.velocity = (velocity * Time.deltaTime * 100f);
            //transform.Translate(velocity*Time.deltaTime);

            

            if (!steppingX && !steppingY)
            {
                AudioManager.Instance.PlaySound("step");
            }

//        horizontal = Input.GetAxisRaw("Horizontal");
//        vertical = Input.GetAxisRaw("Vertical"); 
        }

    void TurnPlayer()
        {
            Vector3 mousePos = Input.mousePosition - Offset; //direction of mouse
            Vector3 difference = PlayerCam.ScreenToWorldPoint(mousePos) - transform.position;
            Vector3 direction = difference.normalized * 10f;

            if (Gun.transform.rotation.z < 0)
            {

                //GunSprite.flipY = false;
            }
            else
            {
                //GunSprite.flipY = true;
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
            if ( /*PlayerInven.AmmoCount > 0 && */ TimerForBullets >= 1.0 / BulletsPerSecond)
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
                ammoCount--;
                GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPos.transform.position,
                    SelectCircle.transform.rotation);

                //Set velocity of bullet
                bullet.GetComponent<Rigidbody2D>().velocity = SelectCircle.transform.up * BulletSpeed;
                AudioManager.Instance.PlaySound("Shooting");
                //reset timer for bullets
                TimerForBullets = 0f;

                //play sound
                //AM.PlaySound("shoot");
            }
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                //Destroy(gameObject);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void ShootGrenade()
        {
            GameObject grenade = Instantiate(BulletPrefab, BulletSpawnPos.transform.position,
                SelectCircle.transform.rotation);
            grenade.GetComponent<Rigidbody2D>().velocity = SelectCircle.transform.up * GrenadeSpeed;

        }

        public void ThrowSword()
        {
            AudioManager.Instance.PlaySound("swordYeet");

            SwordRB.drag = 4f;
            SwordRB.angularDrag = 1f;
            SwordRB.velocity = SelectCircle.transform.up * SwordThrowSpeed;
            Sword.transform.parent = null;
            SwordRB.AddTorque(SwordRotation);
            Physics.IgnoreLayerCollision(10, 11, false);
            SwordCol.isTrigger = false;
            SwordHitCol.enabled = true; //make sure to disable once stops moving
        }


        public void CatchSword()
        {
            if (Input.GetKeyUp(KeyCode.Space) && !IsSwordAttached)
            {
                //print(Sword.transform.position);
                transform.position = Sword.transform.position;
                Sword.transform.parent = SwordHolder.transform;
                Sword.transform.localPosition = SwordPosition;
                Sword.transform.rotation = SelectCircle.transform.rotation;
                SwordRB.velocity = Vector2.zero;
                SwordRB.drag = 1000000f;
                SwordRB.angularDrag = 1000000f;
                SwordRB.SetRotation(0);
                IsSwordAttached = true;
                PressedSpace = false;
                SwordCol.isTrigger = true;
                SwordHitCol.enabled = false;
                Physics.IgnoreLayerCollision(10, 11, true);
                
                //ABLE TO SWING
                AttackTimer = 0f;
            }
        }

        IEnumerator SwordSwing(float swingTime)
        {
            AudioManager.Instance.PlaySound("swordSwing");
            yield return new WaitForSeconds(swingTime);
            SwordHitCol.enabled = false;
            AttackTimer = ResetTimeAttack; //MAGIC NUMBER FOR TIME IN BETWEEN ATTACKS
        }

        public void playerReload()
        {
            StartCoroutine(reloadPlayer());
            
            
            
        }
        IEnumerator reloadPlayer()
        {
            AudioManager.Instance.PlaySound("gunReload");
            CrossHair.GetComponent<SpriteRenderer>().sprite = reloadSprite;
            CrossHair.GetComponent<Animator>().SetTrigger("reload");
            reloading = true;
            yield return new WaitForSeconds(reloadTime);
            ammoCount = 20;
            reloading = false;
            CrossHair.GetComponent<SpriteRenderer>().sprite = crossHairSprite;
        }
        
        void zombieSounds()
        {
            
            AudioManager.Instance.PlaySound("zombieRoar");
            
        }
        void zombieThroat()
        {
            
            AudioManager.Instance.PlaySound("zombieThroat");
            
        }
    }
