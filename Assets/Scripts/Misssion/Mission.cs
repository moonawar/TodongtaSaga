using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum MissionTaskType { Dialogue, Cutscene, Travel, SwitchScene }

[Serializable]
public class MissionTask
{
    public MissionTaskType type;
    public string yarnTitle;  // Only used if taskType is Dialogue
    public Cutscene cutscene;  // Only used if taskType is Cutscene
    public Vector2 travelLocation;  // Only used if taskType is Travel
    public bool showWaypoint = false;  // Only used if taskType is Travel
    [Scene]
    public string sceneName;  // Only used if taskType is SwitchScene
    public List<MissionAction> onTaskComplete;
}

public enum MissionTriggerType { NPC, Object, Location, Auto }

[Serializable]
public class MissionTrigger
{
    public MissionTriggerType type;
    public string npcName;  // Only used if triggerType is NPC
    public string objectName;  // Only used if triggerType is Object
    public Vector2 location;  // Only used if triggerType is Location
    public float radius = 1f;
    public bool showWaypoint;  // Only used if triggerType is Location
}

[Serializable]
public class MissionRelationshipImpact {
    public string npcName;
    public int impactAmount;
}

[CreateAssetMenu(fileName = "NewMission", menuName = "RPG/Mission")]
public class Mission : ScriptableObject
{
    // ----------------------------------- //
    [Header("1. Mission Properties /ᐠ - ˕ -マ Ⳋ")]
    [HorizontalLine(color: EColor.Gray)]
    public string missionName;
    [ShowIf("announce")]
    public string description;
    public bool announce = true;
    public bool hidden;
    public bool isCompleted;

    // ----------------------------------- //
    [Header("2. Mission Trigger ₍^ >ヮ<^₎ .ᐟ.ᐟ")]
    [HorizontalLine(color: EColor.Gray)]
    public MissionTrigger trigger;

    // ----------------------------------- //
    [Header("3. Event and Tasks（• ˕ •マ.ᐟ")]
    [HorizontalLine(color: EColor.Gray)]
    public List<MissionAction> onMissionHandled;
    public List<MissionAction> onMissionStart;
    public List<MissionTask> tasks;
    public Action OnMissionCompleteAction;
    public List<MissionAction> onMissionComplete;

    // ----------------------------------- //
    [Header("4. Post Mission ᓚ₍ ^. ̫ .^₎")]
    [HorizontalLine(color: EColor.Gray)]
    public List<MissionRelationshipImpact> relationshipImpacts;
    public List<Mission> followUpMissions;

    [Header("5. More /ᐠ.ꞈ.ᐟ\")")]
    [HorizontalLine(color: EColor.Gray)]
    public bool isKeyMission;
    [ShowIf("isKeyMission")]
    public string keyMissionFor;
    [ShowIf("isKeyMission")]
    public string bookMissionDescription;
}