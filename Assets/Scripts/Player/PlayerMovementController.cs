using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    public float jumpForce = 5.0f;

    private Transform playerTransform;

    private Ray cameraRay;
	private RaycastHit cameraRayHit;
    private bool disabled = false;
    private Vector2 lastMousePosition = Vector2.zero;
    private Rigidbody rigidBody;
    private bool isOnGround = true;
    private double groundDistance = 0.1f;
    private bool doubleJumped = false;

    void Start() {
        playerTransform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody>();
        groundDistance = GetComponent<Collider>().bounds.extents.y;
    }

    void FixedUpdate() {
        if (disabled) return;

        float currentRotation = playerTransform.rotation.eulerAngles.y;

        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(-horizontalMovement, .0f, verticalMovement);
        movement = Quaternion.Euler(.0f, currentRotation, .0f) * movement;
        
        playerTransform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);

        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector2 mousePosition = Input.mousePosition;

        if (mousePosition - lastMousePosition == Vector2.zero) return;

        if (Physics.Raycast(cameraRay, out cameraRayHit) && cameraRayHit.transform.tag != "Player") {
            Vector3 targetPosition = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            lastMousePosition = mousePosition;
		} 
    }

    void Update() {
        if (disabled) return;

        this.isOnGround = Physics.Raycast(transform.position, -Vector3.up, ((float)groundDistance) + 0.1f);
        if (this.isOnGround) doubleJumped = false;

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (this.isOnGround) rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            else if (!doubleJumped) {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                doubleJumped = true;
            }
        }
    }

    public void Disable() {
        disabled = true;
    }

    public void Enable() {
        disabled = false;
    }
}
