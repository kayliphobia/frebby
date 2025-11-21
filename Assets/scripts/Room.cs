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
        if (connectedRoom)
        {
            return OverlaySprites(GetUnproccessedImage(), connectedRoom.GetUnproccessedImage());
        }
        return GetUnproccessedImage();
    }

    public Sprite GetUnproccessedImage()
    {
        if (IsOccupied())
        {
            return occupiedRoomImages[(int)occupant.GetAIName()];
        }
        return emptyRoomImage;
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
        if (available.Count == 0)
            return null;

        float maxWeight = 0f;
        Room maxWeightRoom = available[0];
        foreach (var room in available)
        {
            maxWeight = System.Math.Max(room.officeBias, maxWeight);
            maxWeightRoom = room;    
        }


        float rand = Random.Range(0f, maxWeight + 1);
        foreach (var room in available)
        {
            if (rand < room.officeBias)
                return room;
        }

        
        return maxWeightRoom;
    }

    // Returns a readable Texture2D from a sprite
    private static Texture2D SpriteToTexture(Sprite sprite)
    {
        Texture2D tex = sprite.texture;
        Rect rect = sprite.textureRect;
        Texture2D readableTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        try
        {
            Color[] pixels = tex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            readableTex.SetPixels(pixels);
            readableTex.Apply();
        }
        catch
        {
            // Non-readable texture: copy via RenderTexture
            RenderTexture rt = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 0);
            Graphics.Blit(tex, rt);
            RenderTexture.active = rt;
            readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            readableTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
        }

        return readableTex;
    }

public static Texture2D OverlayTextures(Texture2D bottom, Texture2D top)
{
    int width = bottom.width;
    int height = bottom.height;

    Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

    // Copy bottom into result
    result.SetPixels(bottom.GetPixels());

    // Overlay top texture
    int topWidth = top.width;
    int topHeight = top.height;

    for (int y = 0; y < topHeight; y++)
    {
        if (y >= height) break; // don't go out of bounds

        for (int x = 0; x < topWidth; x++)
        {
            if (x >= width) break; // don't go out of bounds

            Color b = result.GetPixel(x, y);
            Color t = top.GetPixel(x, y);

            // Alpha blend
            Color outPixel = t.a >= 1f ? t : (t + b * (1f - t.a));
            result.SetPixel(x, y, outPixel);
        }
    }

    result.Apply();
    return result;
}

    public static Sprite OverlaySprites(Sprite bottom, Sprite top)
    {
        Texture2D bottomTex = SpriteToTexture(bottom);
        Texture2D topTex = SpriteToTexture(top);

        Texture2D result = OverlayTextures(bottomTex, topTex);

        return Sprite.Create(
            result,
            new Rect(0, 0, result.width, result.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}
