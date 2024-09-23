using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    
    public float jumpForce = 50f;
    public float floatDuration = 5f;
    private bool isGrounded = false;
    private int floatCount = 0;
    private const int maxFloatCount = 2; 
    private float floatTime;

    public float inhaleRange = 5f;
    public Transform mouthPosition;
    private GameObject inhaledEnemy;
    public GameObject projectilePrefab;

    public float pullSpeed = 3f;
    public float shrinkRate = 0.05f;
    public LayerMask enemyLayer;
    private bool isInhaling = false;
    private bool isFat = false;
    private bool Spit = false;
    public float spitForce = 3f;

    public LayerMask groundLayer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpandFloat();
        HandleInhale();
        HandleSpit();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        float currentSpeed = isFat ? moveSpeed * 0.75f : moveSpeed;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
        {
            if (isFat)
            {
                animator.SetBool("FatWalk", true);
                animator.SetBool("isWalking", false);
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("FatWalk", false);
            }

            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("FatWalk", false);
        }
    }

    private void HandleJumpandFloat()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        if (isGrounded)
        {
            floatCount = 0; //Reset when grounded
            animator.SetBool("isGrounded", true);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (isFat)
            {
                animator.SetTrigger("FatJump");
            }
            else
            {
                animator.SetTrigger("Jump");
            }
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        } 
        else if (!isGrounded && Input.GetButtonDown("Jump") && floatCount < maxFloatCount)
        {
            floatCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.9f);
            if(isFat)
            {
                animator.SetTrigger("FatJump");
            }
            else
            { 
                animator.SetTrigger("Jump");
            }
            animator.SetBool("isGrounded", false);
        }

        //Float logic
        if (!isGrounded && Input.GetButton("Jump") && floatCount > 0 && rb.velocity.y > 0f)
        {
            floatTime -= Time.deltaTime;
            if (floatTime < floatDuration)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 2f);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0f, 0.5f)); //Float behavior
                animator.SetTrigger("Jump");
                animator.SetBool("isGrounded", false);
            }
        }

        if (isGrounded || Input.GetButtonUp("Jump"))
        {
            floatTime = 0f;
            animator.SetBool("isGrounded", true);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }

    private void HandleInhale()
    {

        if (inhaledEnemy != null)
        {
            StartCoroutine(PullEnemyIn(inhaledEnemy));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isInhaling = true;
            animator.SetBool("isInhaling", true);
        }

        if (isInhaling)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(mouthPosition.position, inhaleRange, enemyLayer);
            foreach (Collider2D enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    inhaledEnemy = enemy.gameObject;
                    animator.SetBool("hasInhaledEnemy", true);
                    StartCoroutine(PullEnemyIn(inhaledEnemy));
                    break;
                }
            }
        }

        //Stop inhale, trigger fat animation
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isInhaling = false;
            animator.SetBool("isInhaling", false);
            
            if (inhaledEnemy != null)
            {
                animator.SetTrigger("Fat");
                isFat = true;                
            }
            else
            {
                animator.SetBool("hasInhaledEnemy", false);
            }
        }
    }

    private IEnumerator PullEnemyIn(GameObject enemy)
    {
        while (enemy != null && Vector2.Distance(enemy.transform.position, mouthPosition.position) > 0.1f)
        {

            if (enemy == null) yield break;
            //Move
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, mouthPosition.position, pullSpeed * Time.deltaTime);
            
            //Scale & Rotate
            Vector3 targetScale = Vector3.zero;
            enemy.transform.localScale = Vector3.Lerp(enemy.transform.localScale, targetScale, Time.deltaTime * pullSpeed);
            enemy.transform.Rotate(0f, 0f, 90f * Time.deltaTime);
            
            yield return null;
        }
    }

    private void HandleSpit()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && isFat)
        {
            Spit = true;
            animator.SetTrigger("Spit");

            float spitDirection = Mathf.Sign(transform.localScale.x);

            if (inhaledEnemy != null && Spit)
            {
                GameObject projectile = Instantiate(projectilePrefab, mouthPosition.position, Quaternion.identity);
                
                ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
                projectileController.Initialize(new Vector3(spitDirection, 0f, 0f));
                Animator projectileAnimator = projectile.GetComponent<Animator>();

                Destroy(inhaledEnemy);

            }

            isFat = false;
            Spit = false;
            inhaledEnemy = null;
            animator.SetBool("hasInhaledEnemy", false);

        }
    }

    //Visualize inhale range in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(mouthPosition.position, inhaleRange);
    }

}
