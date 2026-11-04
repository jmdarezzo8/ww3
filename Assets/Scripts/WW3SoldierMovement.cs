using UnityEngine;

// WW3: third-person character locomotion test. No animation clips required.
// WASD move, Shift sprint, Space jump, right mouse drag rotate view, Q/E pose-test hands later.
[RequireComponent(typeof(CharacterController))]
public sealed class WW3SoldierMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.8f;
    public float sprintSpeed = 5.2f;
    public float turnSpeed = 600f;
    public float jumpHeight = 1.0f;
    public float gravity = -18f;
    public Camera followCamera;

    private CharacterController controller;
    private float verticalVelocity;
    private Vector3 cameraOffset = new Vector3(0, 1.8f, -3.6f);

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (followCamera == null) followCamera = Camera.main;
        if (followCamera != null) cameraOffset = transform.InverseTransformPoint(followCamera.transform.position);
    }

    private void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;

        Vector3 cameraForward = followCamera != null ? followCamera.transform.forward : Vector3.forward;
        Vector3 cameraRight = followCamera != null ? followCamera.transform.right : Vector3.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 move = Vector3.ClampMagnitude(cameraForward * vertical + cameraRight * horizontal, 1f);
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            verticalVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
        verticalVelocity += gravity * Time.deltaTime;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        controller.Move((move * speed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (followCamera == null) return;
        Vector3 offset = transform.rotation * cameraOffset;
        followCamera.transform.position = Vector3.Lerp(
            followCamera.transform.position, transform.position + offset, 10f * Time.deltaTime);
        followCamera.transform.LookAt(transform.position + Vector3.up * 1.3f);
    }
}
