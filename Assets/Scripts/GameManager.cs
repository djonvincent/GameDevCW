using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player{get; private set;}
    public Player playerClass{get; private set;}
    public static GameManager instance = null;
    public string startSceneName;
    public HealthbarPlayer healthbar;
    public GameObject gameOver;
    public GameObject helpScreen;
    public GameObject pauseTitle;
    public GameObject pauseMenu;
    public GameObject overlay;
    public GameObject overlayFadeIn;
    public GameObject apples;
    public GameObject books;
    public GameObject loading;
    public Image treasureOverlay;
    public TextMeshProUGUI treasureDesc;
    public TextMeshProUGUI treasureTitle;
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI appleCount;
    public TextMeshProUGUI bookCount;
    public TextMeshProUGUI message;
    public delegate Vector2 CameraTargetFunction();
    public Sprite[] treasureSprites = new Sprite[10];
    public string[] treasureTitles = new string[10];
    public string[] treasureDescriptions = new string[10];
    public bool paused = false;
    public Enemy[] allEnemies;
    public Chest[] allChests;
    public float checkpointHealth;
    public float checkpointMaxHealth;
    public int checkpointApples;
    public bool checkpointHasJacket;
    public bool checkpointHasSword;
    public int checkpointBooks;
    public bool checkpointHasFlashlight;
    public Vector2 checkpointPosition;
    public float checkpointHorizontal;
    public float checkpointVertical;
    public AudioClip victorySound;
    private AudioSource audio;

    private List<Enemy> currentEnemies = new List<Enemy>();
    //private Enemy furthestEnemy;
    private float cameraSpeed = 2f;
    private float targetCameraSize = 3.5f;
    private float baseCameraZoomSpeed = 2f;
    private float cameraZoomSpeed = 2f;
    private bool syncCameraZoom = true;
    private bool cameraAtTarget = false;
    private CameraTargetFunction cameraTargetFunc;
    private Vector2 lastPlayerPosition;
    private bool onStairs = false;
    private float timeOfLastHealthChange = -5f;
    private float lastPlayerHealth;
    private float hideMessageTime = 0f;
    private bool allowUnpause = true;
    private KeyCode pauseKey = KeyCode.Escape;
    private string currentSceneName;
    private bool showingPauseScreen = false;
    private bool showingHelpScreen = false;

    public CameraTargetFunction CameraTarget {
        get {
            return cameraTargetFunc;
        }
        set {
            cameraAtTarget = false;
            Vector2 target = value();
            float distance = ((Vector2)Camera.main.transform.position - target).magnitude;
            cameraSpeed = Math.Max(2, distance/1);
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
        lastPlayerHealth = playerClass.health;
        audio = GetComponent<AudioSource>();
    }

    void Start() {
        MoveCamera(player.transform.position);
        if (SceneManager.sceneCount == 1) {
            currentSceneName = startSceneName;
            if (startSceneName == "Start") {
                StartCoroutine("StartSequence");
            } else {
                SceneManager.LoadScene(startSceneName, LoadSceneMode.Additive);
            }
        } else {
            currentSceneName = SceneManager.GetSceneAt(1).name;
            SetCheckpoint();
            allEnemies = GameObject.FindObjectsOfType<Enemy>();
            allChests = GameObject.FindObjectsOfType<Chest>();
        }
    }

    private void RestartFromCheckpoint() {
        Camera.main.cullingMask = 0;
        lastPlayerHealth = checkpointHealth;
        playerClass.health = checkpointHealth;
        playerClass.maxHealth = checkpointMaxHealth;
        playerClass.hasJacket = checkpointHasJacket;
        playerClass.hasSword = checkpointHasSword;
        playerClass.books = checkpointBooks;
        playerClass.hasFlashlight = checkpointHasFlashlight;
        playerClass.apples = checkpointApples;
        playerClass.anim.SetFloat("Horizontal", checkpointHorizontal);
        playerClass.anim.SetFloat("Vertical", checkpointVertical);
        timeOfLastHealthChange = Time.time - 5f;
        lastPlayerHealth = checkpointHealth;
        LoadLevel(
            SceneManager.GetSceneByName(currentSceneName),
            currentSceneName,
            checkpointPosition
        );
    }

    private IEnumerator StartSequence() {
        player.transform.position = new Vector3(-2.5f, 0, 0);
        playerClass.books = 0;
        playerClass.hasJacket = false;
        playerClass.hasSword = false;
        playerClass.hasFlashlight = false;
        Camera.main.cullingMask = 0;
        AsyncOperation load =
            SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);
        while (!load.isDone) {
            yield return null;
        }
        allEnemies = GameObject.FindObjectsOfType<Enemy>();
        allChests = GameObject.FindObjectsOfType<Chest>();
        overlayFadeIn.SetActive(true);
        Camera.main.cullingMask = -1;
        ShowMessage("Where am I?", 8f);
    }

    public void ShowMessage(string msg, float duration) {
        message.text = msg;
        hideMessageTime = Time.time + duration;
        message.gameObject.SetActive(true);
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

    public void SetCheckpoint() {
        checkpointHealth = playerClass.health;
        checkpointMaxHealth = playerClass.maxHealth;
        checkpointApples = playerClass.apples;
        checkpointHasJacket = playerClass.hasJacket;
        checkpointHasSword = playerClass.hasSword;
        checkpointBooks = playerClass.books;
        checkpointHasFlashlight = playerClass.hasFlashlight;
        checkpointPosition = player.transform.position;
        checkpointHorizontal = playerClass.anim.GetFloat("Horizontal");
        checkpointVertical = playerClass.anim.GetFloat("Vertical");
    }

    private IEnumerator _LoadLevel(Scene scene, string sceneName, Vector2 position) {
        currentEnemies.Clear();
        Camera.main.cullingMask = 0;
        loading.SetActive(true);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
            sceneName, LoadSceneMode.Additive
        );
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone) {
            if (asyncUnload.isDone) {
                currentEnemies.Clear();
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        loading.SetActive(false);
        currentSceneName = sceneName;
        player.transform.position = position;
        MoveCamera(position);
        SetCheckpoint();
        Camera.main.cullingMask = -1;
        allEnemies = GameObject.FindObjectsOfType<Enemy>();
        allChests = GameObject.FindObjectsOfType<Chest>();
    }

    void ClearOverlay() {
        paused = false;
        overlay.SetActive(false);
        prompt.gameObject.SetActive(false);
        treasureTitle.gameObject.SetActive(false);
        treasureDesc.gameObject.SetActive(false);
        treasureOverlay.gameObject.SetActive(false);
        gameOver.SetActive(false);
    }

    void Update() {
        if (showingHelpScreen && Input.GetKeyDown(KeyCode.Escape)) {
            showingHelpScreen = false;
            showingPauseScreen = true;
            helpScreen.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else if (showingPauseScreen) {
            if (Input.GetKeyDown(KeyCode.H)) {
                pauseMenu.SetActive(false);
                helpScreen.SetActive(true);
                showingHelpScreen = true;
            } else if (Input.GetKeyDown(KeyCode.Q)) {
                Application.Quit();
            } else if (Input.GetKeyDown(KeyCode.Escape)) {
                ClearOverlay();
                pauseTitle.SetActive(false);
                pauseMenu.SetActive(false);
                showingPauseScreen = false;
                paused = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) && !playerClass.alive && allowUnpause) {
            ClearOverlay();
            RestartFromCheckpoint();
        }
        else if (Input.GetKeyDown(pauseKey) && playerClass.alive) {
            if (allowUnpause && paused) {
                ClearOverlay();
                paused = false;
                pauseKey = KeyCode.Escape;
            } else if (!paused) {
                paused = true;
                showingPauseScreen = true;
                pauseTitle.SetActive(true);
                pauseMenu.SetActive(true);
                overlay.SetActive(true);
                message.gameObject.SetActive(false);
            }
        }
        if (paused) {
            return;
        }
        if (playerClass.health != lastPlayerHealth) {
            lastPlayerHealth = playerClass.health;
            timeOfLastHealthChange = Time.time;
        }
        if (Time.time > hideMessageTime) {
            message.gameObject.SetActive(false);
        }
        appleCount.text = playerClass.apples.ToString();
        if (playerClass.apples > 0) {
            apples.SetActive(true);
        }
        bookCount.text = playerClass.books.ToString();
        if (playerClass.books > 0) {
            books.SetActive(true);
        }
        healthbar.health = playerClass.health/playerClass.maxHealth;
        healthbar.maxHealth = playerClass.maxHealth/100;
        healthbar.show = currentEnemies.Count > 0 ||
            (Time.time - timeOfLastHealthChange) < 4f;
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

        return;
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
        return player.transform.position + new Vector3(0, 0.5f, 0);
        if (playerClass.onStairs) {
            onStairs = true;
            cameraAtTarget = false;
            return new Vector2(
                player.transform.position.x,
                lastPlayerPosition.y + 0.5f
            );
        } else {
            if (onStairs) {
                onStairs = false;
                CameraTarget = PlayerPosition;
            }
            lastPlayerPosition = player.transform.position;
            return player.transform.position + new Vector3(0, 0.5f, 0);
        }
    }

    public Vector2 CombatPosition () {
        if (currentEnemies.Count == 0) {
            return PlayerPosition();
        }
        //float maxAggroRadius = currentEnemies[0].aggroRadius;
        //Enemy maxEnemy = currentEnemies[0];
        Vector2 avgPosition = player.transform.position;
        int count = 0;
        for (int i=0; i < currentEnemies.Count; i++) {
            Enemy enemy = currentEnemies[i];
            if (enemy != null && enemy.focusCamera) {
                avgPosition += (Vector2)enemy.transform.position;
                count ++;
            }
            //if (enemy.aggroRadius > maxAggroRadius) {
            //    maxAggroRadius = enemy.aggroRadius;
            //    maxEnemy = enemy;
            //}
        }
        //if (furthestEnemy != maxEnemy) {
        //    cameraAtTarget = false;
        //}
        //furthestEnemy = maxEnemy;
        avgPosition /= (count + 1);
        if (count != currentEnemies.Count) {
            cameraAtTarget = false;
        }
        return avgPosition + new Vector2(0, 0.8f);
        //return (maxEnemy.transform.position + player.transform.position)/2 +
        //    new Vector3(0, 0.5f, 0);
    }

    private void GetCameraSize() {
        if (currentEnemies.Count == 0) {
            targetCameraSize = 3.5f;
            return;
        }
        float maxAggroRadius = 0;
        foreach (Enemy enemy in currentEnemies) {
            if (enemy.aggroRadius > maxAggroRadius) {
                maxAggroRadius = enemy.aggroRadius;
            }
        }
        targetCameraSize = 1 + (maxAggroRadius / 2);
    }

    public void AddEnemy(Enemy enemy) {
        cameraAtTarget = false;
        if (!currentEnemies.Contains(enemy)) {
            currentEnemies.Add(enemy);
            CameraTarget = CombatPosition;
            GetCameraSize();
        }
    }

    public void RemoveEnemy(Enemy enemy) {
        cameraAtTarget = false;
        currentEnemies.Remove(enemy);
        GetCameraSize();
        if (currentEnemies.Count == 0) {
            CameraTarget = PlayerPosition;
        }
    }

    public void ShowTreasure(int item) {
        paused = true;
        pauseKey = KeyCode.Return;
        StartCoroutine(_ShowTreasure(item));
    }

    public IEnumerator _ShowTreasure(int item) {
        audio.clip = victorySound;
        audio.Play();
        allowUnpause = false;
        message.gameObject.SetActive(false);
        overlay.SetActive(true);
        treasureOverlay.sprite = treasureSprites[item];
        treasureOverlay.gameObject.SetActive(true);
        treasureDesc.text = treasureDescriptions[item];
        treasureTitle.text = treasureTitles[item];
        prompt.text = "Press enter to continue";
        yield return new WaitForSeconds(1.5f);
        treasureDesc.gameObject.SetActive(true);
        treasureTitle.gameObject.SetActive(true);
        prompt.gameObject.SetActive(true);
        allowUnpause = true;
    }

    public void OnDie() {
        StartCoroutine("_OnDie");
    }

    public IEnumerator _OnDie() {
        allowUnpause = false;
        overlay.SetActive(true);
        gameOver.SetActive(true);
        prompt.text = "Press enter to restart from checkpoint";
        yield return new WaitForSeconds(1f);
        prompt.gameObject.SetActive(true);
        allowUnpause = true;
    }
}
