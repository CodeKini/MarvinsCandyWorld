// ============================================================
// CameraFollow.cs
// Place this script on the Main Camera.
// The camera orbits around the player with mouse input.
// ============================================================

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag the Player GameObject here.")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance    = 6f;    // how far back from the player
    public float height      = 2.5f;  // how high above the player
    public float mouseSensitivity = 120f;
    public float smoothSpeed = 10f;   // how quickly the camera catches up

    // Stores the current horizontal angle around the player
    private float yaw = 0f;

    void Start()
    {
        // Hide and lock the cursor so mouse orbit feels natural
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // LateUpdate runs after all Update calls, which gives the player
    // a chance to move before the camera repositions itself.
    void LateUpdate()
    {
        if (target == null) return;

        // Rotate around Y axis based on mouse X movement
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Calculate where the camera should be
        Quaternion rotation      = Quaternion.Euler(0f, yaw, 0f);
        Vector3   desiredOffset  = rotation * new Vector3(0f, height, -distance);
        Vector3   desiredPosition = target.position + desiredOffset;

        // Smoothly move there
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Always look at a point slightly above the player's feet
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}
