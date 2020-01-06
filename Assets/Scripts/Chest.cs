using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator anim;
    public GameObject light;
    public int item;
    private GameManager GM;
    private bool open = false;

    void Start() {
        GM = GameManager.instance;
    }

    public void Open() {
        open = true;
        light.SetActive(false);
        anim.SetTrigger("Open");
        GM.playerClass.GiveItem(item);
        GM.ShowTreasure(item);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject == GM.player && !open) {
            Open();
        }
    }
}
