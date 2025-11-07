using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panRange = 3f;          // how far the camera can move left/right
    public float panSpeed = 3f;          // how smoothly it moves
    [Range(0.05f, 0.3f)]
    public float edgeThreshold = 0.15f;  // portion of screen near edges
    public float maxTilt = 1.2f;         // subtle tilt amount (degrees)

    private float screenWidth;

    void Start()
    {
        screenWidth = Screen.width;
    }

    void Update()
    {
        float mouseX = Input.mousePosition.x;
        float normalized = 0f; // default = centered

        float leftEdge = screenWidth * edgeThreshold;
        float rightEdge = screenWidth * (1f - edgeThreshold);

        // Only pan when mouse is near edges
        if (mouseX <= leftEdge)
        {
            // Map range [0, leftEdge] to [-1, 0]
            normalized = -1f + (mouseX / leftEdge);
        }
        else if (mouseX >= rightEdge)
        {
            // Map range [rightEdge, screenWidth] to [0, +1]
            normalized = (mouseX - rightEdge) / (screenWidth - rightEdge);
        }
        else
        {
            // No pan — smoothly return to center
            normalized = 0f;
        }

        // Compute target position
        float targetX = Mathf.Clamp(normalized * panRange, -panRange, panRange);
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * panSpeed);

        // Add a subtle tilt effect
        float rotationZ = normalized * maxTilt;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -rotationZ), Time.deltaTime * panSpeed);
    }
}
