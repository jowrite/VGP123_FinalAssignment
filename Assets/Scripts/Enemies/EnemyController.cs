using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]

public class EnemyController : MonoBehaviour
{

    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    public bool isFacingRight = true;

    public Transform groundCheck;
    public LayerMask groundLayer;

    //Sir Kibble
    public bool isSirKibble = false;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 5f;
    public float attackCooldown = 3f;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();

        if (isSirKibble && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

    }

    private void HandleMovement()
    {
        
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down + (isFacingRight ? Vector2.right : Vector2.left) * 0.5f, 0.1f, groundLayer);
        
        if (groundInfo.collider != null)
        {
            rb.velocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, rb.velocity.y);
        }
        else
        {
            Flip();
        }

        //Flip enemy direction
        sr.flipX = !isFacingRight;
        animator.SetBool("isWalking", true);
    }


    private void Attack()
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            Animator projectileAnimator = projectile.GetComponent<Animator>();

            if (projectileAnimator != null)
            {
                projectileAnimator.Play("Projectile");
            }
            
            //Fire projectile in the direction the enemy is facing
            projectileRb.velocity = new Vector2(isFacingRight ? projectileSpeed : -projectileSpeed, 0);
            
            animator.SetTrigger("Throw");

        }
    }
    void Flip()
    {
       isFacingRight = !isFacingRight;
       transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

   
}
