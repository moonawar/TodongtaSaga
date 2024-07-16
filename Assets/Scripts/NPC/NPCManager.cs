using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;
    public List<NPC> npcs;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public NPC FindNPCWithName(string name)
    {
        foreach (var npc in npcs)
        {
            if (npc.Data.name == name)
            {
                return npc;
            }
        }

        Debug.LogWarning("NPC with name " + name + " not found.");
        return null;
    }

    public void UpdateRelationship(string npcName, int amount)
    {
        NPC npc = FindNPCWithName(npcName);
        npc.Data.relationshipLevel += amount;
    }

    public void AssignMissionToNPC(Mission mission, string npcName)
    {
        NPC npc = FindNPCWithName(npcName);
        npc.ShouldDissapear = false;
        npc.transform.position = mission.trigger.npcLocation;
        npc.AddInteractionListener(() => {
            MissionManager.Instance.StartMission(mission);
        });

        npc.gameObject.SetActive(true);
    }

    public void AssignNextTaskToNPC(string npcName, Vector2 location)
    {
        NPC npc = FindNPCWithName(npcName);
        npc.ShouldDissapear = false;
        npc.transform.position = location;
        npc.AddInteractionListener(() => {
            MissionManager.Instance.ExecuteNextTask();
        });

        npc.gameObject.SetActive(true);
    }

    public void SetDissapearNPC(string npcName, bool dissapearImmediately)
    {
        NPC npc = FindNPCWithName(npcName);
        if (dissapearImmediately)
        {
            npc.Dissapear();
        } else {
            npc.ShouldDissapear = true;
        }

    }
}