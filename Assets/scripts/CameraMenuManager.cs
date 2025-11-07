using UnityEngine;

public class CameraMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraMenuUI;
    public CameraPan cameraPan; // <-- reference to the pan script

    public void OpenCamera()
    {
        cameraMenuUI.SetActive(true);
        if (cameraPan != null)
            cameraPan.canPan = false; // lock panning
    }

    public void CloseCamera()
    {
        cameraMenuUI.SetActive(false);
        if (cameraPan != null)
            cameraPan.canPan = true; // unlock panning
    }
}
