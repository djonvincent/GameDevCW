using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Animator anim;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveH, moveV);
        movement.Normalize();
        //rigidBody.MovePosition(rigidBody.position + movement*speed*Time.fixedDeltaTime);
        rigidBody.velocity = movement * speed * Time.fixedDeltaTime * 100;
        anim.SetFloat("Horizontal", moveH);
        anim.SetFloat("Vertical", moveV);
        anim.SetFloat("Speed", movement.magnitude);
    }
}
