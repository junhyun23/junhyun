using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    static public bool change = true;
    public float maxspeed;
    public float jumppower;
    public GameObject enemy_res;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator ani;

    public float jumpTime;
    private bool isJumping;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && !ani.GetBool("IsJump"))
        {
            isJumping = true;
            rigid.AddForce(Vector2.up * jumppower, ForceMode2D.Impulse);
            ani.SetBool("IsJump", true);
        }

        if(Input.GetButton("Jump") && isJumping == true)
        {
            if(jumpTime > 0)
            {
                rigid.velocity = Vector2.up * jumppower;
                jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                jumpTime = 0.4f;
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }


        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        if(Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if(Mathf.Abs(rigid.velocity.x) == 0)
        {
            ani.SetBool("IsWalk", false);
        }
        else
        {
            ani.SetBool("IsWalk", true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxspeed)
        {
            rigid.velocity = new Vector2(maxspeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxspeed*(-1))
        {
            rigid.velocity = new Vector2(maxspeed*(-1), rigid.velocity.y);
        }

        //Lanaing Platform
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1,
                LayerMask.GetMask("Platform"));
            if (rayhit.collider != null)
            {
                if (rayhit.distance < 0.5f)
                {
                    ani.SetBool("IsJump", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            ani.SetBool("IsJump", false);
            enemy_res.SetActive(false);
            change = false;
        }

        if(collision.gameObject.tag == "finish")
        {
            SceneManager.LoadScene("SampleScene");
        }

        if(collision.gameObject.tag == "gold")
        {
            Destroy(collision.gameObject, 0.1f);
        }
    }
}
