using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public Transform firePoint;
    public Rigidbody2D projectile;
    public float damage;
    public float fireInterval;
    public Animator anim;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Health", health);
        if (Input.GetKeyDown(KeyCode.Space)) {
            Invoke("Fire", 0.52f);
            anim.SetTrigger("Attack");
        }
    }

    public void Fire()
    {
        Vector3 direction = player.transform.position + new Vector3(0,1) - firePoint.position;
        direction.Normalize();
        Rigidbody2D proj = Instantiate(projectile) as Rigidbody2D;
        proj.transform.position = firePoint.position + direction;
        proj.transform.up = -direction;
        proj.velocity = direction * 5f;
        Projectile projClass = proj.GetComponent<Projectile>();
        projClass.owner = transform;
        projClass.damage = damage;
        projClass.lifetime = 5f;
    }
}
