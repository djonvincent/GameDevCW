using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public string startSceneName;
    public GameObject player;
    public float cameraSpeed = 2f;
    public float cameraZoomSpeed = 2f;
    private bool inCombat = false;
    private Actor currentEnemy;
    private float targetCameraSize = 5;
    public Vector2 targetCameraPosition;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        player = GameObject.FindWithTag("Player");
        moveCamera(player.transform.position);
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

    private void moveCamera(Vector2 position) {
        Vector3 pos = position;
        pos.z = -10;
        Camera.main.transform.position = pos;
    }

    /*
    IEnumerator ResizeCamera(float size, float duration = 1f) {
        StopCoroutine("ResizeCamera");
        float camSize = Camera.main.orthographicSize;
        float t = 0f;
        do {
            t += Time.deltaTime;
            float frac = Math.Min(t,1/duration);
            Camera.main.orthographicSize = camSize - frac*(camSize-size);
            yield return null;
        } while (t < duration);
    }

    IEnumerator MoveCamera(Vector3 position, float duration = 1f) {
        Vector3 start = Camera.main.transform.position;
        float t = 0f;
        do {
            t += Time.deltaTime;
            float frac = Math.Min(t,1/duration);
            Vector3 p = Vector3.Lerp(start, position, frac);
            p.z = -10;
            Camera.main.transform.position = p;
            yield return null;
        } while (t < 1);
    }*/

    public void LoadLevel(Scene scene, string sceneName, Vector2 position) {
        StartCoroutine(_LoadLevel(scene, sceneName, position));
    }

    private IEnumerator _LoadLevel(Scene scene, string sceneName, Vector2 position) {
        Camera.main.cullingMask = 0;
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        yield return asyncUnload;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
            sceneName, LoadSceneMode.Additive
        );
        yield return asyncLoad;
        player.transform.position = position;
        moveCamera(position);
        Camera.main.cullingMask = -1;
    }

    void Update() {
        if (!inCombat) {
            Vector3 p = player.transform.position;
            targetCameraPosition = p;
        }
        float camSize = Camera.main.orthographicSize;
        Vector2 camPosition = Camera.main.transform.position;
        if (camPosition != targetCameraPosition) {
            float delta = Time.deltaTime * cameraSpeed;
            Vector2 newPos = Vector2.MoveTowards(
                camPosition, targetCameraPosition, delta
            );
            float eta = (targetCameraPosition-camPosition).magnitude / cameraSpeed;
            cameraZoomSpeed = Math.Abs(targetCameraSize - camSize) / eta;
            moveCamera(newPos);
        }
        if (camSize != targetCameraSize) {
            float delta = cameraZoomSpeed * Time.deltaTime;
            if (Math.Abs(targetCameraSize - camSize) < delta) {
                Camera.main.orthographicSize = targetCameraSize;
            } else {
                Camera.main.orthographicSize +=
                    (camSize < targetCameraSize ? 1 : -1) * delta;
            }

        }
    }

    public void StartCombat(Actor enemy) {
        Debug.Log("Combat");
        inCombat = true;
        currentEnemy = enemy;
        targetCameraPosition =
            (enemy.transform.position + player.transform.position) / 2;
        targetCameraSize = 3;
        //StartCoroutine(MoveCamera(
        //    (enemy.transform.position + player.transform.position)/2
        //));
        //StartCoroutine("ResizeCamera", 3);
    }

    public void Flee() {
        //StartCoroutine("ResizeCamera", 5);
        targetCameraSize = 5;
        inCombat = false;
        if (currentEnemy != null) {
            currentEnemy.inCombat = false;
        }
        currentEnemy = null;
    }
}
