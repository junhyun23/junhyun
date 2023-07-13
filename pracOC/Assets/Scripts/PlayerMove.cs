using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    static public bool change = true;
    public float horizontal;
    public float maxspeed = 8f;
    public float jumppower;
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

    [SerializeField] private TrailRenderer tr;
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

        if (Input.GetButtonDown("Jump") && !ani.GetBool("IsJump"))
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

        if(Mathf.Abs(rigid.velocity.x) == 0)
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
