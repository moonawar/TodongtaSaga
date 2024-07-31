using UnityEngine;
using System.Collections.Generic;
using System;

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

    public NPC FindNPCWithName(string npcName)
    {
        foreach (var npc in npcs)
        {
            if (npc.Data.npcName == npcName)
            {
                return npc;
            }
        }

        Debug.LogError("NPC with name " + npcName + " not found.");
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
        npc.transform.position = mission.trigger.location;
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

    public void PositionNPC(string npcName, Vector2 location)
    {
        NPC npc = FindNPCWithName(npcName);
        npc.ShouldDissapear = false;
        npc.transform.position = location;
        npc.ClearInteractionListeners();
        npc.gameObject.SetActive(true);
    }

    public void SetDissapearNPC(string npcName, bool dissapearImmediately)
    {
        NPC npc = FindNPCWithName(npcName);
        npc.ClearInteractionListeners();
        if (dissapearImmediately)
        {
            npc.Dissapear();
        } else {
            npc.ShouldDissapear = true;
        }
    }

    public List<NPCData> GetNPCBookDatas()
    {
        List<NPCData> npcsData = new();
        foreach (var npc in npcs)
        {
            if (!npc.Data.ignoreFromBook) npcsData.Add(npc.Data);
        }

        return npcsData;
    }

    public NPCData GetNPCBookData(string npcName)
    {
        foreach (var npc in npcs)
        {
            if (npc.Data.npcName == npcName)
            {
                if (!npc.Data.ignoreFromBook) return npc.Data;
            }
        }

        return null;
    }

    public void LoadNPCBookDatas(List<NPCData> datas) {
        string dataStr = string.Join(", ", datas);
        Debug.Log($"Loading NPC Book Datas: [{dataStr}]");
        foreach (NPCData data in datas)
        {
            Debug.Log(data ? data : "Data is null");
            NPC npc = FindNPCWithName(data.npcName);
            npc.Data.relationshipLevel = data.relationshipLevel;
        }
    }
}