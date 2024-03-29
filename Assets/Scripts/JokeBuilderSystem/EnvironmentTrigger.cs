using UnityEngine;
using System.Collections;

public class EnvironmentTrigger : MonoBehaviour {
    public GameManager gameManager;
    public string currentState;
    public string nextState;

    private bool disabled = false;

    [TextArea(3, 10)] public string lineToAddOnSelect;
    ILineAction action;

    void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered");

        if (this.gameManager == null) return;
        if (!other || !other.CompareTag("Player")) return;
        if (this.disabled) return;

        // maybe it worth to show confirm or something idk
        this.gameManager.triggerNextState(this);
        this.executeAction();

        this.disabled = true;
    }

    void OnTriggerExit(Collider other) {
        if (this.gameManager == null) return;
        if (!other || !other.CompareTag("Player")) return;

        this.gameManager.hideButtons();
        StartCoroutine(disableForSeconds(5f));
    }

    public void executeAction() {
        if (this.action != null) this.action.Execute();
    }

    private IEnumerator disableForSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        this.disabled = false;
    }
}