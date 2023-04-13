using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_LabTest2 : MonoBehaviour {
    [Header("Mine Stuff")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Transform mine;

    [Header("Fish Stuff")]
    [SerializeField] private Transform fish_Parent;
    [SerializeField] private Transform fish1_Pool, fish2_Pool, fish3_Pool;
    [SerializeField] private GameObject fish1, fish2, fish3;
    private List<GameObject> fish1_List, fish2_List, fish3_List;

    [Header("Difficulty Timer Stuff")]
    [SerializeField] private float fishSpawnDelay = 0.5f;
    [SerializeField] private float difficultyDelay = 10f;
    private float delayLeft;
    private float difficultyTimer;

    [Header("BG Stuff")]
    [SerializeField] private Transform bgParent;
    private Transform currBG;

    [Header("Health Stuff")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartParent;
    [SerializeField] private int healthMax = 3;
    private int health;

    [Header("Timer Stuff")]
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    private float timer;

    [Header("Start Screen Stuff")]
    [SerializeField] private GameObject startScreen;

    [Header("Game Over Screen Stuff")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    [Header("Audio Stuff")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audio_Thud;

    private bool isHoldingMouse = false;

    private void Awake() {
        fish1_Pool.gameObject.SetActive(false);
        fish2_Pool.gameObject.SetActive(false);
        fish3_Pool.gameObject.SetActive(false);

        // Set up bg looping stuff
        //===================================================================================================================================================
        for (int i = 0; i < bgParent.childCount; i++) {
            bgParent.GetChild(i).localPosition = new Vector3(0, -1080 * i, 0);
        }

        currBG = bgParent.GetChild(bgParent.childCount > 1 ? 1: 0);
        //===================================================================================================================================================

        timer = 0.0f;
        health = healthMax;

        startScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    private void Start() {
        // Set up fish
        //===================================================================================================================================================
        fish1_List = new List<GameObject>();
        fish2_List = new List<GameObject>();
        fish3_List = new List<GameObject>();

        // Set up fish pool
        for (int i = 0; i < 10; i ++) {
            GameObject newFish1 = Instantiate(fish1, fish1_Pool);
            GameObject newFish2 = Instantiate(fish2, fish2_Pool);
            GameObject newFish3 = Instantiate(fish3, fish3_Pool);

            fish1_List.Add(newFish1);
            fish2_List.Add(newFish2);
            fish3_List.Add(newFish3);

            newFish1.GetComponent<FishAI>().SetGM(this);
            newFish2.GetComponent<FishAI>().SetGM(this);
            newFish3.GetComponent<FishAI>().SetGM(this);
        }
        //===================================================================================================================================================

        // Health set up
        //===================================================================================================================================================
        Transform currHeart;

        for (int i = 0; i < health; i++) {
            currHeart = Instantiate(heartPrefab, heartParent).transform;
            currHeart.localPosition = new Vector2(105 * i, 0);
        }
        //===================================================================================================================================================
    }

    // Update is called once per frame
    private void Update() {
        // Do all these IF start screen isn't active and player isn't dead
        if (!startScreen.activeSelf && health > 0) {
            // Mine object following mouse stuff
            //===================================================================================================================================================
            if (Input.GetMouseButtonUp(0)) { isHoldingMouse = false; }
            if (Input.GetMouseButtonDown(0)) { isHoldingMouse = true; }

            if (isHoldingMouse) {
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                mine.position = Vector3.MoveTowards(mine.position, new Vector3(targetPos.x, mine.position.y, mine.position.z), 0.1f);
            }
            //===================================================================================================================================================

            // Timer stuff
            //===================================================================================================================================================
            timer += Time.deltaTime;
            timerText.text = timer.ToString("F2");
            //===================================================================================================================================================

            // Difficulty timer stuff
            //===================================================================================================================================================
            difficultyTimer += Time.deltaTime;

            if (difficultyTimer >= difficultyDelay) {
                fishSpawnDelay -= 0.1f;

                if (fishSpawnDelay < 0.1f) {
                    fishSpawnDelay = 0.1f;
                }

                difficultyTimer = 0f;
            }
            //===================================================================================================================================================

            // Fish spawn stuff
            //===================================================================================================================================================
            delayLeft -= Time.deltaTime;

            if (delayLeft < 0) {
                SpawnFish();
                delayLeft = fishSpawnDelay;
            }
            //===================================================================================================================================================
        }
    }

    public void StartGame() {
        startScreen.SetActive(false);
    }

    public void SetCurrBG(Transform newBG) { currBG = newBG; }

    public void ReturnFish(Transform fish) {
        if (fish.GetComponent<Rigidbody2D>()) {
            fish.GetComponent<Rigidbody2D>().velocity = Vector3.zero; // Stop fish movement
        }

        if (fish1_List.Contains(fish.gameObject)) { fish.SetParent(fish1_Pool); }
        else if (fish2_List.Contains(fish.gameObject)) { fish.SetParent(fish2_Pool); }
        else if (fish3_List.Contains(fish.gameObject)) { fish.SetParent(fish3_Pool); }
    }

    // Spawn fish outside of current BG. Fish will move in
    public void SpawnFish() {
        bool isLeft = UnityEngine.Random.Range(1, 3) % 2 > 0;
        Transform currFish = null;

        //Randomize a fish
        switch (UnityEngine.Random.Range(0, 4)) {
            case 0:
                if (fish1_Pool.childCount > 0) { currFish = fish1_Pool.GetChild(0); }
                break;
            case 1:
                if (fish2_Pool.childCount > 0) { currFish = fish2_Pool.GetChild(0); }
                break;
            case 2:
                if (fish3_Pool.childCount > 0) { currFish = fish3_Pool.GetChild(0); }
                break;
            default:
                // Spawn nothing
                break;
        }
 

        if (currFish != null) {
            // Randomize fish start pos with a type-casted lambda
            Vector2 fishPos = new Func<Vector2>(() => {
                return new Vector2(UnityEngine.Random.Range(0.0f, 1100.0f) * (isLeft ? -1.0f : 1.0f), -625.0f);
            })();

            currFish.GetComponent<FishAI>().StartMove(fishPos, isLeft);
            currFish.SetParent(currBG);
        }
    }

    public void ReduceHealth() {
        if (health > 0) {
            audioSource.PlayOneShot(audio_Thud);

            health -= 1;

            // Disable hearts based on health
            for (int i = healthMax - 1; i >= health; i--) {
                heartParent.GetChild(i).gameObject.SetActive(false);
            }

            //Debug.Log("Player Health: " + health);
        }

        if (health < 1) {
            //Debug.Log("Player is dead!");

            scoreText.text = "You've survived for " + timerText.text + " seconds.";
            gameOverScreen.SetActive(true);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() {
        Application.Quit();
    }
}