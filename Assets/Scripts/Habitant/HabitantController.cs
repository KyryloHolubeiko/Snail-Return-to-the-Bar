using UnityEngine;

public class HabitantController : MonoBehaviour {
    public float initialHealth = 100.0f;
    public float initialJoy = .0f;

    private float _health;
    private float _joy;

    void Start() {
        this._health = this.initialHealth;
        this._joy = this.initialJoy;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            // suggest to player to interact with this habitant / open a dialog
        }
    }
}