using UnityEngine;

public class CameraMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraMenuUI;
    public CameraPan cameraPan; // <-- reference to the pan script
    [SerializeField] private Collider2D computerCollider;

    private void Start()
    {

    }

    public void OpenCamera()
    {
        cameraMenuUI.SetActive(true);
        if (cameraPan != null)
            cameraPan.canPan = false; // lock panning

        if (computerCollider != null)
            computerCollider.enabled = false;
    }

    public void CloseCamera()
    {
        cameraMenuUI.SetActive(false);
        if (cameraPan != null)
            cameraPan.canPan = true; // unlock panning

        if (computerCollider != null)
            computerCollider.enabled = true;
    }
}
