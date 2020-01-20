using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLoadLevel : MonoBehaviour
{
    public Animator anim;
    public GameObject light;
    private GameManager GM;
    public bool open = false;
    public string sceneName;
    public Vector2 position;

    void Start() {
        GM = GameManager.instance;
    }

    public void Open() {
        open = true;
        light.SetActive(false);
        if (anim != null) {
            anim.SetTrigger("Open");
        }
        GM.LoadLevel(gameObject.scene, sceneName, position, null, true);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.transform.IsChildOf(GM.player.transform) && !open) {
            Open();
        }
    }
}
