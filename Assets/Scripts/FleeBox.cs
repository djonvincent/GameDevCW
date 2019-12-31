using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBox : MonoBehaviour
{
    private GameManager GM;

    void Start() {
        GM = GameManager.instance;
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Flee collision");
        if (other.transform.IsChildOf(GM.player.transform)) {
            GM.Flee();
        }
    }
}
