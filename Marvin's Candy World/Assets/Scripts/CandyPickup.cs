// ============================================================
// CandyPickup.cs
// Place this on each candy collectible in CandyWorld.
// Each candy needs:
//   - A mesh (Sphere or Capsule works great as a placeholder)
//   - A Sphere Collider with "Is Trigger" checked
//   - This script
// ============================================================

using UnityEngine;

public class CandyPickup : MonoBehaviour
{
    [Header("Animation")]
    public float spinSpeed  = 90f;   // degrees per second
    public float bobSpeed   = 2f;    // bobs per second
    public float bobHeight  = 0.25f; // metres up and down

    // Remember the starting height so we bob relative to it
    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        // Spin around the Y axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // Bob up and down using a sine wave
        float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    // Called when the player's CharacterController enters this trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the GameManager one candy was collected
            if (GameManager.Instance != null)
                GameManager.Instance.CollectCandy();

            // Remove this candy from the world
            Destroy(gameObject);
        }
    }
}
