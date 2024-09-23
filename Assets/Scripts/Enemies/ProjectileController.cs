using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int maxShots = 5;
    private int remainingShots;
    public float speed = 3f;
    public float lifetime = 6f;
    private Vector3 direction;

    public void Initialize(Vector3 facingDirection)
    {
        direction = facingDirection.normalized;
    }

    private void Start()
    {
        remainingShots = maxShots;
        Destroy(projectilePrefab, lifetime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && remainingShots > 0)
        {
            FireProjectile();
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void FireProjectile()
    {
        Instantiate(projectilePrefab, transform.position, transform.rotation);
        remainingShots--;

        if (remainingShots <= 0)
        {
            Debug.Log("Cutter ability expired");
        }

    }

    public void ResetShots()
    {
        remainingShots = maxShots;
    }
}
