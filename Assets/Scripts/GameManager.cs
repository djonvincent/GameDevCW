using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string startSceneName;
    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        if (SceneManager.sceneCount == 1) {
            SceneManager.LoadScene(startSceneName, LoadSceneMode.Additive);
        }
        for (int n=0; n < SceneManager.sceneCount; n++) {
            Scene scene = SceneManager.GetSceneAt(n);
            if (scene.name == "Manager") {
                continue;
            }
            //SceneManager.MoveGameObjectToScene(player, scene);
        }
    }
}
