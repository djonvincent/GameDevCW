using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneName;
    public Vector2 position;
    private bool hasEntered;
    private GameManager GM;

    void Start() {
        GM = GameManager.instance;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Player" && !hasEntered) {
            hasEntered = true;
            GM.LoadLevel(gameObject.scene, sceneName, position);
        }
    }
}
