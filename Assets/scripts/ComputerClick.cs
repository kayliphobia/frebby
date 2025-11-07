using UnityEngine;

public class ComputerClick : MonoBehaviour
{
    public GameObject cameraMenuUI;

    void OnMouseDown()
    {
        cameraMenuUI.SetActive(true);
    }
}
