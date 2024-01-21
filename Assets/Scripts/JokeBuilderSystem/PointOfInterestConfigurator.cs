using UnityEngine;

[System.Serializable]
public class PointOfInterestConfigurator : MonoBehaviour {
    [SerializeField] public GameObject triggerObject;
    [SerializeField] public int nextStateIndex;
    [SerializeField] public int currentStateIndex;
}