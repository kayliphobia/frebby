using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Office : Room

{
    public List<Room> retreatRooms;

    // We define left room as index 0, right room as index 1
    public new Room GetWeightedConnectedRoom()
    {
        return retreatRooms[(int) occupant.GetAIName()];
    }
}
