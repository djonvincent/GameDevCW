using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public List<Animator> anims;
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
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical")*0.7f;
        Vector2 movement = new Vector2(moveH, moveV);
        if (moveH != 0) {
        movement.Normalize();
        }
        //rigidBody.MovePosition(rigidBody.position + movement*speed*Time.fixedDeltaTime);
        rigidBody.velocity = movement * speed * Time.fixedDeltaTime * 100;
        foreach(Animator anim in anims) {
        anim.SetFloat("Horizontal", moveH);
        anim.SetFloat("Vertical", moveV);
        anim.SetFloat("Speed", movement.magnitude);
        }
    }
}
