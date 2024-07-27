using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "RPG/NPC")]
public class NPCData : ScriptableObject
{
    // ----------------------------------- //
    [Header("1. NPC Info /ᐠ - ˕ -マ Ⳋ")]
    [HorizontalLine(color: EColor.Gray)]
    public string npcName;
    public NPCTungku tungku;
    public string description;
    public bool ignoreFromBook = false;
    [Tooltip("From 0 to 10")]
    public int relationshipLevel;

    // ----------------------------------- //
    [Header("2. Graphics ₍^ >ヮ<^₎ .ᐟ.ᐟ")]
    [HorizontalLine(color: EColor.Gray)]
    public Sprite hiImage;
    public Sprite loImage;
}