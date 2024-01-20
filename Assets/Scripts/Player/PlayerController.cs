using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float initialHealth = 3.0f;
    public float initialMana = 100.0f;

    private float _health;
    private float _mana;

    void Start() {
        this._health = this.initialHealth;
        this._mana = this.initialMana;
    }

    float getHealth() {
        return this._health;
    }

    float getMana() {
        return this._mana;
    }

    void setHealth(float health) {
        this._health = health;
    }

    void setMana(float mana) {
        this._mana = mana;
    }

    bool isDead() {
        return this._health <= 0.0f;
    }
}