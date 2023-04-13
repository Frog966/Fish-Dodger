
using UnityEngine;

public class FishAI : MonoBehaviour {
    [SerializeField] private GameManager_LabTest2 gameManager;

    [Header("Move Stuff")]
    public float speed = 1;
    private bool isLeft;

    // Setter
    public void SetGM(GameManager_LabTest2 newGM) { gameManager = newGM;  }

    private void Update() {
        //transform.localPosition = new Vector3(speed * (isLeft ? -1f : 1), 0, 0) * Time.deltaTime;
        transform.Translate(new Vector3(isLeft ? 1f : -1f, 0, 0) * speed * Time.deltaTime);
    }

    public void StartMove(Vector2 startPos, bool _isLeft) {
        isLeft = _isLeft;
        this.transform.localPosition = startPos;

        this.transform.localScale = new Vector3(_isLeft ? -1 : 1, 1, 1);
    }

    // when the GameObjects collider arrange for this GameObject to travel to the left of the screen
    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            //Debug.Log(col.gameObject.name + " : " + gameObject.name);

            gameManager.ReduceHealth();
        }
    }
}