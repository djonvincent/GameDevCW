using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausableAnimation : MonoBehaviour
{
    private bool paused;
    private float speed; 
    private GameManager GM;
    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Start() {
        GM = GameManager.instance;
    }

    void FixedUpdate() {
        if (!paused && GM.paused) {
            paused = true;
            speed = anim.speed;
            anim.speed = 0f;
        }
        if (paused && !GM.paused) {
            paused = false;
            anim.speed = speed;
        }
    }
}
