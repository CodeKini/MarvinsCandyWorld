// ============================================================
// PlayerController.cs
// Place this script on the Player GameObject.
// The Player needs: Capsule mesh, CharacterController component,
// and the "Player" tag assigned in the Inspector.
// ============================================================

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("References")]
    [Tooltip("Drag the Main Camera here, or leave empty to auto-find.")]
    public Transform cameraTransform;

    // -- private state --
    private CharacterController controller;
    private Vector3 velocity;          // tracks vertical (gravity) velocity

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Auto-find the camera if nothing was assigned in the Inspector
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // ---- Grounded check ----
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;   // small negative keeps us stuck to the ground

        // ---- Horizontal movement relative to the camera ----
        float h = Input.GetAxis("Horizontal");   // A/D or left/right arrow
        float v = Input.GetAxis("Vertical");     // W/S or up/down arrow

        Vector3 move = Vector3.zero;
        if (cameraTransform != null)
        {
            // Flatten the camera's forward/right vectors onto the XZ plane
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight   = Vector3.Scale(cameraTransform.right,   new Vector3(1, 0, 1)).normalized;
            move = camForward * v + camRight * h;
        }
        else
        {
            // Fallback: world-space movement if no camera
            move = new Vector3(h, 0f, v);
        }

        // Rotate the player to face the movement direction
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
        }

        controller.Move(move * moveSpeed * Time.deltaTime);

        // ---- Jump ----
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // ---- Apply gravity ----
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
