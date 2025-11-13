using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraMenuUI;
    public CameraPan cameraPan; // <-- reference to the pan script
    [SerializeField] private Collider2D computerCollider;

    [SerializeField] private Room currentCamera;

    private List<Room> rooms;

    private Image cameraFeed;

    private void Start()
    {
        cameraFeed = cameraMenuUI.transform.Find("CameraFeed")?.GetComponent<Image>();
        GameObject roomsParent = GameObject.Find("Rooms");
        if (roomsParent == null)
        {
            Debug.Log("Rooms object not found in the scene!");
            return;
        }
        else
        {
            Debug.Log("Rooms object was found");
        }

        // Get all Room components in descendants (including inactive)
        rooms = new List<Room>();
        rooms.AddRange(roomsParent.GetComponentsInChildren<Room>(true));
    }

    void Update()
    {
        if (cameraMenuUI.activeInHierarchy)
        {
            cameraFeed.sprite = currentCamera.GetCurrentImage();
        }
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

    public void CameraButtonClick()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Room room = rooms.Find(r => r.name == clickedButton.name);
        if (room == null)
        {
            Debug.Log("Room not found");
            return;
        } else
        {
            Debug.Log($"{clickedButton} was clicked");
        }
        currentCamera = room;
    }
}
