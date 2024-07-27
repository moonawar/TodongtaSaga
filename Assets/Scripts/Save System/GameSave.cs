using System.Collections.Generic;
using UnityEngine;

public class GameSave 
{
    public Vector2 PlayerPosition;
    public string CurrentScene;
    public List<Mission> AllMissions;
    public List<Mission> AvailableMissions;
    public Dictionary<string, List<GameObject>> SceneObjects = new();
    public List<NPCData> npcsData = new();
}