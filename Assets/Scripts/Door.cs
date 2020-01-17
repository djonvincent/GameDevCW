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
    public bool back = false;

    void Start() {
        GM = GameManager.instance;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject == GM.player && !hasEntered) {
            if (back) {
                GM.ShowMessage("You cannot go back", 2f);
                return;
            }
            foreach (Enemy enemy in GM.allEnemies) {
                if (enemy != null && enemy.alive) {
                    GM.ShowMessage("Defeat all enemies before proceeding", 2f);
                    return;
                }
            }
            foreach (Chest chest in GM.allChests) {
                if (!chest.open) {
                    GM.ShowMessage("Open all chests before proceeding", 2f);
                    return;
                }
            }
            hasEntered = true;
            GM.LoadLevel(gameObject.scene, sceneName, position);
        }
    }
}
