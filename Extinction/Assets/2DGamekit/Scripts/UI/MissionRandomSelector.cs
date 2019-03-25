using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionRandomSelector : MonoBehaviour
{
    public List<Mission> missions;
    public bool removeFromListOnSelected = true;

    private void Start()
    {
        if (missions == null || missions.Count == 0)
            enabled = false;
    }

    public void EnableRandomMission()
    {
        if (missions != null && missions.Count != 0)
        {
            int id = (int)(Random.Range(0, missions.Count - 0.01f));
            missions[id].Register();
            if (removeFromListOnSelected)
                missions.RemoveAt(id);
        }
    }

    public void AddMissionToList(Mission mission)
    {
        if (missions == null)
            missions = new List<Mission>();
        if(mission)
            missions.Add(mission);
    }

    public void RemoveMissionFromList(Mission mission)
    {
        if (missions != null && missions.Contains(mission))
            missions.Remove(mission);
    }
}
