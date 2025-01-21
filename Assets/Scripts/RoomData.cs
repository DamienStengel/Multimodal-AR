using System;
using System.Collections.Generic;

[Serializable]
public class RoomInfo
{
    public string name;
    public int capacity;
    public string[] equipment;
    public string type;
    public int floor;
    public string building;
}

[Serializable]
public class RoomsData
{
    public Dictionary<string, RoomInfo> rooms;
} 