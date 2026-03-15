// ============================================================
// PoolPortal.cs
// Place this on an EMPTY GAMEOBJECT sitting inside the pool.
// That object needs a Box Collider with "Is Trigger" checked.
// Size it to fill the water surface area.
// ============================================================

using UnityEngine;

public class PoolPortal : MonoBehaviour
{
    [Tooltip("Must match the exact Scene name in Build Settings.")]
    public string candyWorldSceneName = "CandyWorld";

    // Prevents double-triggering if the player bounces in the collider
    private bool alreadyTriggered = false;

    // This fires when any collider enters the trigger zone.
    // Unity calls this even for CharacterController-based objects.
    void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;

        if (other.CompareTag("Player"))
        {
            alreadyTriggered = true;

            // Ask the GameManager to fade out and load the new scene
            if (GameManager.Instance != null)
                GameManager.Instance.LoadScene(candyWorldSceneName);
            else
                // Fallback in case GameManager isn't in the scene yet
                UnityEngine.SceneManagement.SceneManager.LoadScene(candyWorldSceneName);
        }
    }

    // Draw a visible blue wireframe in the editor so we can see the trigger zone
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
            Gizmos.DrawCube(transform.position + bc.center, bc.size);
    }
}
