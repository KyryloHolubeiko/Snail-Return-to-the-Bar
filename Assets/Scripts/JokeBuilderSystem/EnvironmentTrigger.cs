using UnityEngine;

public class EnvironmentTrigger : MonoBehaviour {
    public GameManager gameManager;
    public int currentStateIndex;
    public int nextStateIndex;

    [TextArea(3, 10)] public string lineToAddOnSelect;
    ILineAction action;

    void OnTriggerEnter(Collider other) {
        if (this.gameManager == null) return;
        if (!other || !other.CompareTag("Player")) return;

        // maybe it worth to show confirm or something idk
        this.gameManager.triggerNextState(this);
        this.executeAction();
    }

    void OnTriggerExit(Collider other) {
        if (this.gameManager == null) return;
        if (!other || !other.CompareTag("Player")) return;

        this.gameManager.hideButtons();
    }

    public void executeAction() {
        if (this.action != null) this.action.Execute();
    }
}