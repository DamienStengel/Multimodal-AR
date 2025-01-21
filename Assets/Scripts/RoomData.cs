using System;
using System.Collections.Generic;

[Serializable]
public class RoomInfo
{
    public string name;
    public int capacity;
    public string[] equipment;
    public string type;
    public string building;
}

[Serializable]
public class RoomEntry
{
    public string id;
    public RoomInfo info;
}

[Serializable]
public class RoomsData
{
    public RoomEntry[] rooms;
    private Dictionary<string, RoomInfo> roomsDictionary;

    public Dictionary<string, RoomInfo> GetRoomsDictionary()
    {
        if (roomsDictionary == null)
        {
            roomsDictionary = new Dictionary<string, RoomInfo>();
            if (rooms != null)
            {
                foreach (var entry in rooms)
                {
                    roomsDictionary[entry.id] = entry.info;
                }
            }
        }
        return roomsDictionary;
    }
} 