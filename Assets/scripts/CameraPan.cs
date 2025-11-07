using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panRange = 3f;
    public float panSpeed = 3f;
    [Range(0.05f, 0.3f)]
    public float edgeThreshold = 0.15f;
    public float maxTilt = 1.2f;

    [HideInInspector]
    public bool canPan = true;  // <-- add this

    private float screenWidth;

    void Start()
    {
        screenWidth = Screen.width;
    }

    void Update()
    {
        if (!canPan) return; // <-- stop movement when camera menu is open

        float mouseX = Input.mousePosition.x;
        float normalized = 0f;

        float leftEdge = screenWidth * edgeThreshold;
        float rightEdge = screenWidth * (1f - edgeThreshold);

        if (mouseX <= leftEdge)
            normalized = -1f + (mouseX / leftEdge);
        else if (mouseX >= rightEdge)
            normalized = (mouseX - rightEdge) / (screenWidth - rightEdge);

        float targetX = Mathf.Clamp(normalized * panRange, -panRange, panRange);
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * panSpeed);

        float rotationZ = normalized * maxTilt;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -rotationZ), Time.deltaTime * panSpeed);
    }
}
