using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator anim;
    public GameObject light;
    public int item;
    private GameManager GM;
    public bool open = false;
    public bool destroyOnOpen = false;

    void Start() {
        GM = GameManager.instance;
    }

    public void Open() {
        Destroy(GetComponent<Help>());
        open = true;
        light.SetActive(false);
        if (anim != null) {
            anim.SetTrigger("Open");
        }
        if (destroyOnOpen) {
            Destroy(gameObject);
        }
        GM.playerClass.GiveItem(item);
        GM.ShowTreasure(item);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.transform.IsChildOf(GM.player.transform) && !open) {
            Open();
        }
    }
}
