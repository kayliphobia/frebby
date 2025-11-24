using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraMenuUI;
    public GameObject productivityUI;
    public CameraPan cameraPan; // <-- reference to the pan script
    [SerializeField] private Collider2D computerCollider;

    [SerializeField] private Room currentCamera;

    private List<Room> rooms;

    private Image feed;
    private Image overlay;
    public Sprite transparentImage;

    private Sprite oldFeed, oldOverlay;



    private void Start()
    {
        Transform cameraFeed = cameraMenuUI.transform.Find("CameraFeed");
        if (cameraFeed)
        {
            feed = cameraFeed.Find("feed").GetComponent<Image>();
            overlay = cameraFeed.Find("overlay").GetComponent<Image>();
        }
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
        GameManager.ReturnToDesk += CloseCamera;
    }

    void Update()
    {
        if (cameraMenuUI.activeInHierarchy)
        {
            Sprite camImage = currentCamera.GetCurrentImage();
            if (camImage)
            {
                feed.sprite = camImage;
            } else
            {
                feed.sprite = transparentImage;
            }
            camImage = currentCamera.GetOverlayImage();
            if (camImage)
            {
                overlay.sprite = camImage;
            } else
            {
                overlay.sprite = transparentImage;
            }
            if (oldFeed && oldOverlay && (oldFeed != feed.sprite || oldOverlay != overlay.sprite))
            {
                GlitchEffect.GlitchEvent?.Invoke();
                Debug.Log("asdf");
            }
            // feed.sprite = currentCamera.GetCurrentImage() ?? transparentImage;
            // overlay.sprite = currentCamera.GetOverlayImage() ?? transparentImage;
            oldFeed = feed.sprite;
            oldOverlay = overlay.sprite;
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
        productivityUI.SetActive(false);
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
