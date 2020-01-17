using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour
{
    public string message;
    public float radius = 5f;
    private GameManager GM;

    void Start() {
        GM = GameManager.instance;
    }

    void FixedUpdate() {
        if (GM.paused) {
            return;
        }
        if ((GM.player.transform.position - transform.position).magnitude < radius) {
            GM.ShowMessage(message, 0.1f);
        }
    }
}
