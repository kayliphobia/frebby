using UnityEngine;

public class ComputerClick : MonoBehaviour
{
    public CameraMenuManager cameraMenuManager;

    void OnMouseDown()
    {
        if (cameraMenuManager != null)
            cameraMenuManager.OpenCamera();
    }
}
