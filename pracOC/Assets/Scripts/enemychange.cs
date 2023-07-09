using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemychange : MonoBehaviour
{ 
    public Transform target;
    public float Speed;
    public GameObject camera_enemy;
    public GameObject Player;

    Rigidbody2D rb;
    SpriteRenderer sprite;

    void Awake()
    {
        gameObject.GetComponent<EnemyChMove>().enabled = false;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(PlayerMove.change == false && Player.activeSelf == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position,
                Speed * Time.deltaTime);
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player.SetActive(false);
            sprite.enabled = true;
            camera_enemy.SetActive(true);
            gameObject.GetComponent<EnemyChMove>().enabled = true;
        }
    }
}

