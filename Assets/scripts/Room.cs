using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class Room : MonoBehaviour
{
    public Sprite emptyRoomImage;

    public Room connectedRoom;

    public List<Sprite> occupiedRoomImages;

    public SpriteRenderer backgroundRenderer;
    [HideInInspector]
    public string roomName;

    [SerializeField]
    protected List<Room> connectedRooms;

    [Tooltip("Higher means more likely an AI will move here")]
    public float officeBias = 1f; // bias toward office

    [SerializeField]
    protected AI occupant;

    public bool IsOccupied() => occupant != null;

    public void Start()
    {
        roomName = gameObject.name;

        connectedRooms = new List<Room>();
        // Add parent room if it exists
        Room parentRoom = transform.parent ? transform.parent.GetComponent<Room>() : null;
        if (parentRoom != null)
            connectedRooms.Add(parentRoom);

        // Add all direct child rooms
        foreach (Transform child in transform)
        {
            Room childRoom = child.GetComponent<Room>();
            if (childRoom != null)
                connectedRooms.Add(childRoom);
        }
    }

    public Sprite GetCurrentImage()
    {
        if (IsOccupied())
        {
            return occupiedRoomImages[(int)occupant.GetAIName()];
        }
        return emptyRoomImage;
    }

    public Sprite GetOverlayImage()
    {
        if (connectedRoom)
        {
            return connectedRoom.GetCurrentImage();
        }
        return null;
    }

    public void Enter(AI ai)
    {
        Debug.Log($"{ai.GetAIName()} moved to {roomName}");

        if (occupant == null)
            occupant = ai;
        else
            Debug.LogWarning($"{roomName} is already occupied by {occupant.name}!");
        if (backgroundRenderer)
            backgroundRenderer.sprite = GetCurrentImage();
    }

    public void Leave(AI ai)
    {
        if (occupant == ai)
            occupant = null;
        if (backgroundRenderer)
            backgroundRenderer.sprite = GetCurrentImage();        
    }

    public Room GetWeightedConnectedRoom()
    {
        List<Room> available = connectedRooms.FindAll(r => !r.IsOccupied());
        if (available.Count == 0) return null;

        // Shuffle the available rooms to randomize order
        for (int i = 0; i < available.Count; i++)
        {
            int j = Random.Range(i, available.Count);
            Room temp = available[i];
            available[i] = available[j];
            available[j] = temp;
        }

        // Weighted random selection
        float totalWeight = 0f;
        foreach (var room in available)
            totalWeight += room.officeBias;

        float rand = Random.Range(0f, totalWeight);

        foreach (var room in available)
        {
            if (rand < room.officeBias)
                return room;
            rand -= room.officeBias;
        }

        return available[available.Count - 1]; // fallback
    }

    public Room GetParentRoom()
    {
        return connectedRooms[0];
    }

}
