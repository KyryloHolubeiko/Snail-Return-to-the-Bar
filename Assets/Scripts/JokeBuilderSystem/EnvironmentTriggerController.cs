using UnityEngine;

public class EnvironmentTriggerController : MonoBehaviour, IStateAction {
    public string type { get { return "EnvironmentTrigger"; } }
    public State nextState { get; set; }
    public GameObject triggerObject;

    // public GameManager gameManager;

    // public EnvironmentTriggerController(GameObject triggerObject, GameManager gameManager, State nextState) {
    //     this.triggerObject = triggerObject;
    //     this.gameManager = gameManager;
    //     this.nextState = nextState;
    //     this.linkToObject();
    // }

    // public EnvironmentTriggerController(GameObject triggerObject, GameManager gameManager) {
    //     this.triggerObject = triggerObject;
    //     this.gameManager = gameManager;
    //     this.linkToObject();
    // }  

    // public EnvironmentTriggerController(GameManager gameManager, State nextState) {
    //     this.gameManager = gameManager;
    //     this.nextState = nextState;
    // }

    // public EnvironmentTriggerController(GameManager gameManager) {
    //     this.gameManager = gameManager;
    // }

    // public void linkToObject() {
    //     if (this.triggerObject == null) return;

    //     this.triggerObject.AddComponent<EnvironmentTrigger>();
    //     this.triggerObject.GetComponent<EnvironmentTrigger>().gameManager = this.gameManager;
    //     this.triggerObject.GetComponent<EnvironmentTrigger>().nextState = this.nextState;
    // }

    // public void linkToObject(GameObject triggerObject) {
    //     if (triggerObject == null) return;

    //     this.triggerObject = triggerObject;
    //     this.linkToObject();
    // }

    public void executeAction() {
        if (this.triggerObject == null) return;

        // this.triggerObject.GetComponent<EnvironmentTrigger>().executeAction();
    }
}