using UnityEngine;

public class ComputerUIManager : MonoBehaviour
{
    public GameObject cameraPanel;
    public GameObject productivityPanel;
    public ProductivitySystem productivitySystem;

    public void OpenProductivity()
    {
        cameraPanel.SetActive(false);
        productivityPanel.SetActive(true);
        productivitySystem.SetWindowActive(true);
    }

    public void ReturnToCameras()
    {
        productivityPanel.SetActive(false);
        cameraPanel.SetActive(true);
        productivitySystem.SetWindowActive(false);
    }
}
