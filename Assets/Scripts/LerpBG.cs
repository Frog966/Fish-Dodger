using UnityEngine;
using System.Collections;

public class LerpBG : MonoBehaviour {
    [SerializeField] private GameManager_LabTest2 gameManager;
    [SerializeField] private Rigidbody2D rb;

    private float height;

    private void Start() {
        height = this.GetComponent<RectTransform>().rect.height;

        rb.velocity = new Vector2(0, 5);
    }

    private void Update() {
        if (this.transform.localPosition.y > height) {
            // Return all fish of this BG
            foreach (Transform fish in this.transform) { gameManager.ReturnFish(fish); }

            this.transform.localPosition = new Vector3(0, -1080 * (this.transform.parent.childCount - 1), 0);

            gameManager.SetCurrBG(this.transform);
        }
    }
}