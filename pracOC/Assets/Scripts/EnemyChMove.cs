using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyChMove : MonoBehaviour
{
    public float horizontal;
    public float jumppower;
    public float maxspeed = 8f;
    public GameObject camera_enemy;
    public GameObject enemy_res;
    private bool isFacingRight = true;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator ani;

    public float jumpTime;
    private bool isJumping;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            rigid.AddForce(Vector2.up * jumppower, ForceMode2D.Impulse);
        }

        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (jumpTime > 0)
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

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (Mathf.Abs(rigid.velocity.x) == 0)
        {
            ani.SetBool("IsWalk", false);
        }
        else
        {
            ani.SetBool("IsWalk", true);
        }

        if (Input.GetMouseButtonDown(0) && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rigid.velocity = new Vector2(horizontal * maxspeed, rigid.velocity.y);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0f;
        rigid.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rigid.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            enemy_res.SetActive(false);
        }

        if (collision.gameObject.tag == "finish")
        {
            gameObject.GetComponent<EnemyChMove>().enabled = false;
            spriteRenderer.enabled = false;
            camera_enemy.SetActive(false);
            enemy_res.SetActive(true);
            SceneManager.LoadScene("SampleScene");
        }

        if (collision.gameObject.tag == "gold")
        {
            Destroy(collision.gameObject, 0.1f);
        }
    }
}
