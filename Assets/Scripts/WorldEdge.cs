using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEdge : MonoBehaviour
{
    private GameManager GM;

    void Start() {
        GM = GameManager.instance;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject == GM.player) {
            GM.ShowMessage("You cannot go that way", 1f);
            return;
        }
    }
}
