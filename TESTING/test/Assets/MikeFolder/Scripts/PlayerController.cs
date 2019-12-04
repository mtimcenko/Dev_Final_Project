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
                velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInputVertical,
                    walkAcceleration * Time.deltaTime);
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
            CrossHair.GetComponent<SpriteRenderer>().sprite = reloadSprite;
            CrossHair.GetComponent<Animator>().SetTrigger("reload");
            reloading = true;
            yield return new WaitForSeconds(reloadTime);
            ammoCount = 20;
            reloading = false;
            CrossHair.GetComponent<SpriteRenderer>().sprite = crossHairSprite;
        }
    }
