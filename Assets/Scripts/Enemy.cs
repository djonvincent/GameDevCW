using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public Transform firePoint;
    public GameObject projectile;
    public float damage;
    public float fireInterval;
    public Animator anim;
    private GameObject player;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
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
        Vector3 start = firePoint.position;
        Vector3 target = player.transform.position + new Vector3(0,1,0);
        Vector3 direction = target - start;
        direction.Normalize();
        GameObject proj = (GameObject)Instantiate(projectile);
        Rigidbody2D projRB = proj.GetComponent<Rigidbody2D>();
        proj.transform.position = start - new Vector3(0,1,0) + direction;
        proj.transform.GetChild(0).transform.up = -direction;
        projRB.velocity = direction * 5f;
        Projectile projClass = proj.GetComponent<Projectile>();
        projClass.owner = transform;
        projClass.damage = damage;
        projClass.lifetime = 5f;
        projClass.knockBack = 30;
    }
}
