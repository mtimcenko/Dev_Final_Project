﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class FollowPlayer : MonoBehaviour
{

    public GameObject Player;
    public float AggroRange = 5f;
    public float MovementSpeed = 5f;
    public int Health = 3;
    public Vector3 PlayerPosition;
    private Rigidbody2D RB;
    public float Timer = 0f;

    public float FindPlayerPos = .5f;

    public GameObject AmmoPrefab;

    public bool IsRanged = false;

    public GameObject RangedPrefab;
    public float RangedBulletSpeed;
    private float RangedTimer = 0f;
    public float RangedRate = .5f;
    //private AudioManager AM;
    // Start is called before the first frame update
    void Start()
    {
        //Set private components
        // AM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Player = GameObject.FindWithTag("Player");
        RB = GetComponent<Rigidbody2D>();
    

    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardPlayer();
        //Chase player if in aggro range
        //if (Vector3.Distance(Player.transform.position, transform.position) <= AggroRange)
        if (!IsRanged)
        {
            TurnEnemy();
            MoveTowardPlayer();
        }
        //stand still otherwise
        else
        {
            RangedTimer += Time.deltaTime;
            if (RangedTimer >= RangedRate)
            {
                //Shoot bullet
                GameObject rangedAttack = Instantiate(RangedPrefab, transform.position,
                    transform.rotation);
                
                rangedAttack.GetComponent<Rigidbody2D>().velocity = transform.up * RangedBulletSpeed;
                RangedTimer = 0f;
            }
            
            if (Vector3.Distance(Player.transform.position, transform.position) >= AggroRange)
            {
                
            }
            
            TurnEnemy();
            RB.velocity = Vector2.zero;
        }

        Timer += Time.deltaTime;
        if (Timer >= FindPlayerPos)
        {
            Timer = 0f;
            PlayerPosition = Player.transform.position;
        }
    }

    void TurnEnemy()
    {
        Vector3 dir = (PlayerPosition - transform.position).normalized;
        transform.up = dir;
    }

    void MoveTowardPlayer()
    {
        RB.velocity = (PlayerPosition - transform.position).normalized * MovementSpeed;
    }

    public void LoseHealth(int amount)
    {
        Health -= amount;
        //AM.PlaySound("bulletEnemy");
        //Check if enemy is dead
        if (Health <= 0)
        {
            
            //play enemy dead sound
            //AM.PlaySound("enemyDead");
            DropAmmo();
            //destroy self
            Player.GetComponent<PlayerController>().ScoreAmount++;
            Destroy(gameObject);
            
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            LoseHealth(other.gameObject.GetComponent<BulletBehavior>().BulletDamage);
        }
        
        if (other.gameObject.CompareTag("Sword"))
        {
            //infinite damage
            LoseHealth(200);
            DropAmmo();
            Destroy(gameObject);
        }
            
    }

    public void DropAmmo()
    {
        Instantiate(AmmoPrefab, transform.position, Quaternion.identity);
    }

    
   
}
