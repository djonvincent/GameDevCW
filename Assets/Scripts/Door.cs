using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneName;
    public Vector3 position;
    private GameObject player;
    private bool hasEntered;

    void Start() {
        player = GameObject.FindWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("trigger " + other.tag);
        if(other.tag == "Player" && !hasEntered) {
            hasEntered = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.completed += (op) => {
            Camera.main.cullingMask = 0;
            //SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneName));
            player.transform.position = position;
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(gameObject.scene);
            asyncUnload.completed += (op2) => {
                Camera.main.cullingMask = -1;
            };
            };
            //SceneManager.LoadScene(sceneName);
        }
    }
}
