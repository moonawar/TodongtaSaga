using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "RPG/NPC")]
public class NPCData : ScriptableObject
{
    public string npcName;

    [Tooltip("From 0 to 100")]
    public int relationshipLevel;
}