using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraMovementSmooth = 0.05f;

    private Vector3 offset;

    void Start() {
        this.offset = this.transform.position - this.player.transform.position;
    }

    void FixedUpdate() {
        this.moveAndRotate();
    }

    private void moveAndRotate() {
        Vector3 distanceToPlayer = this.transform.position - this.player.transform.position;
        distanceToPlayer.y = 0.0f;

       this.transform.position = Vector3.Lerp(
            this.transform.position,
            this.player.transform.position + this.offset,
            this.cameraMovementSmooth
        );
    }
    
}
