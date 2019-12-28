using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform firePoint;
    public Rigidbody2D projectile;
    public float damage;
    public float health;
    public float fireInterval;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Fire();
        }
    }

    void Fire()
    {
        Vector2 direction = player.transform.position + new Vector3(0,1) - firePoint.position;
        direction.Normalize();
        Rigidbody2D proj = Instantiate(projectile) as Rigidbody2D;
        proj.transform.position = firePoint.position;
        proj.transform.up = -direction;
        proj.velocity = direction * 5f;
    }
}
