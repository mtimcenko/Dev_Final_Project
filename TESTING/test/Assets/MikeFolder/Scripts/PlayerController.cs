using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PlayerController : MonoBehaviour
{
//    Rigidbody2D body;

    public Vector2 velocity;
    public Vector2 velocityY;
    public bool OutBoundaries = false;
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
    
    private float AttackTimer = 0f;
    private float HoldSpaceTimer = 0f;
    public float HoldSpaceDash = .3f;
    
    public TextMeshProUGUI AliveTextMesh;
    private float AliveTimer = 0f;
    private float LightingTimer = 0f;
    public float CurrentChangeLighting = 10f;
    public Vector2 ChangeLightingRange = new Vector2(20, 30);
    public bool IsLightsOn = true;
    public Color[] LightingColors;
    public Light2D GlobalLight;

    public BoxCollider2D PickUpCol;

    public bool SwordCircleSwing = false;
    
    public float Rotation = 15.0f;

    public int SwordDamage = 2;
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
        //Change Lighting
        AliveTimer += Time.deltaTime;
        LightingTimer += Time.deltaTime;
        AliveTextMesh.text = Math.Round(AliveTimer, 2).ToString();
        //EVERY 30 seconds, change lighting
        if (LightingTimer >= CurrentChangeLighting)
        {
            if (IsLightsOn)
            {
                IsLightsOn = false;
                StartCoroutine(LightsOff());
                //lights transition
            }
            else
            {
                IsLightsOn = true;
                StartCoroutine(LightsOn());
            }

            CurrentChangeLighting = UnityEngine.Random.Range(ChangeLightingRange.x, ChangeLightingRange.y);
            //float a = Random.Range(1f, 2f);
            LightingTimer = 0f;
        }
        
        if (reloading == false)
        {
            reloadSR.enabled = false;
            //  crossHairSR.sprite = crossHairSprite;
            ammoComponent.text = "Ammo: " + ammoCount;
        }
        else
        {
            //reloadSR.GetComponent<Animator>().SetTrigger("reload");
            reloadSR.enabled = false;
            // crossHairSR.sprite = reloadSprite;
            ammoComponent.text = "Reloading!";
        }

        // Debug.Log(ammoCount);

        TimerForBullets += Time.deltaTime;
        //Debug.Log(Gun.transform.rotation.z);
        //rotate player gun
        if (SwordCircleSwing)
        {
            transform.Rotate(0, 0, Rotation);
        }
        else
        {
            TurnPlayer();
        }

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
        if (Input.GetKeyUp(KeyCode.Space) && !PressedSpace && AttackTimer <= 0 && HoldSpaceTimer < HoldSpaceDash)
        {
            //ShootGrenade();
            PressedSpace = true;
            IsSwordAttached = false;
            ThrowSword();
        }
        else if (!IsSwordAttached)
        {
            if (SwordRB.velocity.magnitude < 1)
            {
                SwordHitCol.enabled = false;
            }

            if (Input.GetKeyUp(KeyCode.Space) && !IsSwordAttached)
            {
                CatchSword();
            }
        
        }
        //if sword attached not thrown
        else
        {
            Sword.transform.localPosition = SwordPosition; //For weird bugs when colliding with wall
            //attack sword
            if (AttackTimer <= 0)
            {
                if (Input.GetMouseButtonDown(1) && IsSwordAttached)
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

        if (!OutBoundaries)
        {
            transform.Translate(velocity * Time.deltaTime);
        }
        
        
        
        if (Input.GetKey(KeyCode.Space))
        {
            HoldSpaceTimer += Time.deltaTime;
        }
        else
        {
            HoldSpaceTimer = 0f;
        }
    
        //RB.velocity = (velocity * Time.deltaTime * 100f);
        //transform.Translate(velocity*Time.deltaTime

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


                BulletShell();
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
            
            if (other.gameObject.GetComponent<Collider2D>() == PickUpCol)
            {
                CatchSword();
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
            StartCoroutine(EnableCatchSword());
        }


        public void CatchSword()
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
            //temp true until circle sword swing is done
            SwordHitCol.enabled = true;
            Physics.IgnoreLayerCollision(10, 11, true);
            PickUpCol.enabled = false;
            //ABLE TO SWING
            AttackTimer = 0f;

            SwordCircleSwing = true;
            StartCoroutine(StopSwordCircleSwing());

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
            //ScoreTextComponent.text = "Reloading!";
            CrossHair.GetComponent<SpriteRenderer>().sprite = reloadSprite;
            CrossHair.GetComponent<Animator>().SetTrigger("reload");
            reloading = true;
            yield return new WaitForSeconds(reloadTime);
            ammoCount = 20;
            reloading = false;
            CrossHair.GetComponent<SpriteRenderer>().sprite = crossHairSprite;
        }

        IEnumerator LightsOff()
        {
            GlobalLight.enabled = false;
            yield return new WaitForSeconds(.6f);
            GlobalLight.enabled = true;
            yield return new WaitForSeconds(.4f);
            GlobalLight.enabled = false;
            yield return new WaitForSeconds(.3f);
            GlobalLight.enabled = true;
            yield return new WaitForSeconds(.2f);
            GlobalLight.enabled = false;
        }
        
        IEnumerator LightsOn()
        {
            GlobalLight.enabled = true;
            yield return new WaitForSeconds(.6f);
            GlobalLight.enabled = false;
            yield return new WaitForSeconds(.4f);
            GlobalLight.enabled = true;
            yield return new WaitForSeconds(.3f);
            GlobalLight.enabled = false;
            yield return new WaitForSeconds(.2f);
            GlobalLight.enabled = true;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Collider2D>() == PickUpCol)
            {
                CatchSword();
            }
            if (other.gameObject.CompareTag("Ammo"))
            {
                if (StoredAmmo <= 125)
                {
                    StoredAmmo += 5;
                }
                else
                {
                    StoredAmmo += 5;
                    if (StoredAmmo > 130)
                    {
                        StoredAmmo -= (StoredAmmo -= 130);
                        Debug.Log("AmmoFull");
                    }
                }
                Debug.Log(StoredAmmo);

            }
            
            
        }

        IEnumerator EnableCatchSword()
        {
            float delay = .3f;
            yield return new WaitForSeconds(delay);
            PickUpCol.enabled = true;
        }

        IEnumerator StopSwordCircleSwing()
        {
            float delay = .7f;
            yield return new WaitForSeconds(delay);
            SwordCircleSwing = false;
            SwordHitCol.enabled = false;
            transform.rotation = Quaternion.identity;
        }
        
        public int MaxAmmo = 20; //Maximum of bullets inside one clip
        public int StoredAmmo = 130; //Total bullets inside the gun
        private int AmmoDifference; 

        public void GunReload()
        {
            AmmoDifference = MaxAmmo - ammoCount; //Calculates the amount of bullets fired
            if (StoredAmmo >= 20) //If total bullets > 20
            {
                ammoCount = 20;    //Set ammo to 20 and reduce StoredAmmo by the ammo difference
                StoredAmmo -= AmmoDifference;
            }
            else //If ammo is less than 20
            {

                ammoCount += (Math.Min(StoredAmmo,AmmoDifference));  //Add
                StoredAmmo -= (Math.Min(StoredAmmo,AmmoDifference));
            }

            Debug.Log(StoredAmmo);
        }

        public float ForceMultiplier = 20f;
        public GameObject ShellPrefab;
        public void BulletShell()
        {
            GameObject shell = Instantiate(ShellPrefab, BulletSpawnPos.transform.position, Quaternion.identity);
    
            shell.GetComponent<Rigidbody2D>().AddForce(-SelectCircle.transform.up * ForceMultiplier/*Direction.transform.position-OriginPlayer.transform.position*ForceMultiper*/, ForceMode2D.Impulse);
            //
            //int hash = Animator.StringToHash("FaShell.transform.position = Vector3.Lerp(OriginPlayer.transform.position, Direction.transform.position, 0.5f);llDown");
            //anim.Play(hash, 0, 0);
        }

}
