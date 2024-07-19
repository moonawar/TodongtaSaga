using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "RPG/NPC")]
public class NPCData : ScriptableObject
{
    public string npcName;

    [Tooltip("From 0 to 10")]
    public int relationshipLevel;
    public NPCTungku tungku;
    public Sprite hiImage;
    public Sprite loImage;
    public string description;
}