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
        ToExplore
    }

    public Type type;
    public string npcName = ""; // if setDissapearNPC, positionNPC
    public Vector2 position; // if createInteractableObject, positionPlayer, or positionNPC
    public bool dissapearImmediately; // if setDissapearNPC
    public PosOrientationType orientation = PosOrientationType.Down; // if positionPlayer
}