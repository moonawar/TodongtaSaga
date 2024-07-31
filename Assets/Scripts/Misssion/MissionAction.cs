using UnityEngine;

[System.Serializable]
public class MissionAction
{
    public enum Type
    {
        CreateInteractableObject,
        PositionPlayer,
        ExecuteNextTask,
        PositionNPC,
        SetDissapearNPC,
        ToExplore,
        GetAchievement, 
        UlosAction
    }

    public Type type;
    public string npcName = ""; // if setDissapearNPC, positionNPC
    public string achievementId = "";
    public UlosAction ulosAction; // if createInteractableObject
    public bool assignNextTask = false; // if positionNPC
    public Vector2 position; // if createInteractableObject, positionPlayer, or positionNPC
    public bool dissapearImmediately; // if setDissapearNPC
    public PosOrientationType orientation = PosOrientationType.Down; // if positionPlayer
}