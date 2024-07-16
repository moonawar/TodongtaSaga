using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MissionTaskType { Dialogue, Cutscene, Travel, SwitchScene }

[System.Serializable]
public class MissionTask
{
    public MissionTaskType type;
    public string yarnTitle;  // Only used if taskType is Dialogue
    public Cutscene cutscene;  // Only used if taskType is Cutscene
    public Vector2 travelLocation;  // Only used if taskType is Travel
    public bool showWaypoint = false;  // Only used if taskType is Travel
    public string sceneName;  // Only used if taskType is SwitchScene

    public List<MissionAction> onTaskComplete;
}

public enum MissionTriggerType { NPC, Object, Location, Auto }

[System.Serializable]
public class MissionTrigger
{
    public MissionTriggerType type;
    public string npcName;  // Only used if triggerType is NPC
    public Vector2 npcLocation;  // Only used if triggerType is NPC
    public string objectName;  // Only used if triggerType is Object
    public Vector2 location;  // Only used if triggerType is Location
    public float radius = 1f;
    public bool showWaypoint;  // Only used if triggerType is Location
}

[System.Serializable]
public class MissionRelationshipImpact {
    public string npcName;
    public int impactAmount;
}

[CreateAssetMenu(fileName = "NewMission", menuName = "RPG/Mission")]
public class Mission : ScriptableObject
{
    public string missionName;
    public string description;
    public MissionTrigger trigger;
    public List<MissionTask> tasks;
    public bool announce = true;
    public bool hidden;
    public List<MissionRelationshipImpact> relationshipImpacts;
    public List<Mission> followUpMissions;
    public Action OnMissionCompleteAction;
}