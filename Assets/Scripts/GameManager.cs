using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Player playerClass;
    public static GameManager instance = null;
    public string startSceneName;
    public float cameraSpeed = 2f;
    public Graphic healthbar;
    public delegate Vector2 CameraTargetFunction();

    private List<Enemy> currentEnemies = new List<Enemy>();
    private float targetCameraSize = 5;
    private float baseCameraZoomSpeed = 2f;
    private float cameraZoomSpeed = 2f;
    private bool syncCameraZoom = true;
    private bool cameraAtTarget = false;
    private CameraTargetFunction cameraTargetFunc;

    public CameraTargetFunction CameraTarget {
        get {
            return cameraTargetFunc;
        }
        set {
            cameraAtTarget = false;
            Vector2 target = value();
            float distance = ((Vector2)Camera.main.transform.position - target).magnitude;
            cameraSpeed = distance/1;
            cameraTargetFunc = value;
        }
    }   

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        player = GameObject.FindWithTag("Player");
        playerClass = player.GetComponent<Player>();
        MoveCamera(player.transform.position);

        if (SceneManager.sceneCount == 1) {
            SceneManager.LoadScene(startSceneName, LoadSceneMode.Additive);
        }
        for (int n=0; n < SceneManager.sceneCount; n++) {
            Scene scene = SceneManager.GetSceneAt(n);
            if (scene.name == "Manager") {
                continue;
            }
        }
    }

    private void MoveCamera(Vector2 position) {
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
        MoveCamera(position);
        Camera.main.cullingMask = -1;
    }

    void Update() {
        healthbar.rectTransform.localScale = new Vector3(
            Mathf.Clamp(playerClass.health/100, 0, 1), 1, 1
        );
        if (CameraTarget == null) {
            CameraTarget = PlayerPosition;
        }
        Vector2 targetCamPosition = CameraTarget();
        float camSize = Camera.main.orthographicSize;
        Vector2 camPosition = Camera.main.transform.position;

        if (cameraAtTarget) {
            MoveCamera(CameraTarget());
        } else if (camPosition == targetCamPosition) {
            cameraAtTarget = true;
        } else {
            float delta = Time.deltaTime * cameraSpeed;
            Vector2 newPos = Vector2.MoveTowards(
                camPosition, targetCamPosition, delta
            );
            MoveCamera(newPos);
            if (syncCameraZoom) {
                float eta = (targetCamPosition-camPosition).magnitude / cameraSpeed;
                cameraZoomSpeed = Math.Abs(targetCameraSize - camSize) / eta;
            }
        }

        if (!syncCameraZoom) {
            cameraZoomSpeed = baseCameraZoomSpeed;
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

    public Vector2 PlayerPosition () {
        return player.transform.position;
    }

    public Vector2 CombatPosition () {
        if (currentEnemies.Count == 0) {
            return PlayerPosition();
        }
        float maxAggroRadius = currentEnemies[0].aggroRadius;
        Enemy maxEnemy = currentEnemies[0];
        for (int i=1; i < currentEnemies.Count; i++) {
            Enemy enemy = currentEnemies[i];
            if (enemy.aggroRadius > maxAggroRadius) {
                maxAggroRadius = enemy.aggroRadius;
                maxEnemy = enemy;
            }
        }
        return (maxEnemy.transform.position + player.transform.position)/2;
    }

    private void GetCameraSize() {
        if (currentEnemies.Count == 0) {
            targetCameraSize = 5;
            return;
        }
        float maxAggroRadius = 0;
        foreach (Enemy enemy in currentEnemies) {
            if (enemy.aggroRadius > maxAggroRadius) {
                maxAggroRadius = enemy.aggroRadius;
            }
        }
        targetCameraSize = maxAggroRadius / 2;
    }

    public void AddEnemy(Enemy enemy) {
        if (!currentEnemies.Contains(enemy)) {
            currentEnemies.Add(enemy);
            CameraTarget = CombatPosition;
            GetCameraSize();
        }
    }

    public void RemoveEnemy(Enemy enemy) {
        currentEnemies.Remove(enemy);
        GetCameraSize();
        if (currentEnemies.Count == 0) {
            CameraTarget = PlayerPosition;
        }
    }
}
