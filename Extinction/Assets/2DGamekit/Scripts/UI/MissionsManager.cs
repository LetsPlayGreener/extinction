using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager instance;

    private List<Mission> missions;

    private bool applicationQuitting = false;

    MissionWithTimer tmpMissionTimer = null;
    MissionWithCounter tmpMissionCounter = null;

    public int MissionsCount
    {
        get
        {
            if (missions != null)
                return missions.Count;
            return -1;
        }
    }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            missions = new List<Mission>();
            if (!instance)
                instance = this;

            Application.wantsToQuit += ApplicationIsQuitting;
        }
    }

    private bool ApplicationIsQuitting()
    {
        applicationQuitting = true;
        return true;
    }

    public int GetMissionID(Mission mission)
    {
        if (missions.Contains(mission))
            return missions.IndexOf(mission);
        return -1;
    }

    public Mission GetMissionByID(int id)
    {
        if (id > -1 && id < MissionsCount)
            return missions[id];
        return null;
    }

    #region Register Functions
    public void Register(Mission mission)
    {
        if (mission != null && !missions.Contains(mission))
        {
            missions.Add(mission);
            mission.MissionEnabledEvent.Invoke(mission);

            //Display initial text
            if(mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                DisplayTimerText(tmpMissionTimer.GetComponent<TextMeshProUGUI>(), tmpMissionTimer.text, tmpMissionTimer.countDown ? tmpMissionTimer.duration : 0);
            }
            else if (mission is MissionWithCounter)
            {
                tmpMissionCounter = mission as MissionWithCounter;
                DisplayCounterText(tmpMissionCounter.GetComponent<TextMeshProUGUI>(), tmpMissionCounter.text, tmpMissionCounter.displayCounter, tmpMissionCounter.CountGoal);
            }
            
            if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                GBL_Interface.SendStatement("received", "quest", mission.gameObject.name, new Dictionary<string, List<string>>()
                    {
                        { "content", new List<string>() { mission.GetComponent<TextMeshProUGUI>().text } }
                    });

            //animation to display mission
            //set gameobject to the correct height depending on the number of mission registered
            mission.Rect.anchoredPosition = Vector2.down * mission.Rect.rect.height * (GetMissionID(mission) + 0.5f);
            Vector2 target = mission.Rect.anchoredPosition;
            mission.Rect.anchoredPosition += Vector2.left * mission.Rect.rect.width;
            StartCoroutine(MoveRectTowardPosition(mission.Rect, target));
        }
    }

    public void Unregister(Mission mission, bool completed = false)
    {
        if (missions.Contains(mission))
        {
            if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                GBL_Interface.SendStatement("completed", "quest", mission.gameObject.name, new Dictionary<string, List<string>>()
                    {
                        { "content", new List<string>() { mission.Text } }
                    });

            mission.MissionDisabledEvent.Invoke(mission);
            missions.Remove(mission);

            //check if application is quitting to avoid coroutine error
            if(!applicationQuitting)
                //animation to hide completed mission text
                StartCoroutine(MoveRectTowardPosition(mission.Rect, mission.Rect.anchoredPosition + Vector2.left * mission.Rect.rect.width, true, completed, true));
        }
    }

    public void RefreshMissionText(Mission mission)
    {
        if (mission is MissionWithTimer)
        {
            tmpMissionTimer = mission as MissionWithTimer;
            DisplayTimerText(tmpMissionTimer.GetComponent<TextMeshProUGUI>(), tmpMissionTimer.text, tmpMissionTimer.countDown ? tmpMissionTimer.duration - tmpMissionTimer.TimeSpent : tmpMissionTimer.TimeSpent);
        }
        else if (mission is MissionWithCounter)
        {
            tmpMissionCounter = mission as MissionWithCounter;
            DisplayCounterText(tmpMissionCounter.GetComponent<TextMeshProUGUI>(), tmpMissionCounter.text, tmpMissionCounter.displayCounter, tmpMissionCounter.CountGoal, tmpMissionCounter.Count);
        }
    }
    #endregion

    /// <summary>
    /// Coroutine to move the mission gameobject to the target position.
    /// If unregistering, disable the game object after reaching target
    /// </summary>
    /// <param name="rect">The RectTransform of the mission gameobject</param>
    /// <param name="target">The targeted position reached by the gameobject at the end of the coroutine</param>
    /// <param name="unregistering">True if the coroutine is called while unregistering</param>
    /// <param name="completed">True if the mission was completed (information used only if unregistering)</param>
    /// <param name="constant">The object moves with a constant speed if true, and slow down when getting close to the target if false</param>
    /// <returns></returns>
    private IEnumerator MoveRectTowardPosition(RectTransform rect, Vector2 target, bool unregistering = false, bool completed = false, bool constant = false)
    {
        Color initialColor = rect.GetComponent<TextMeshProUGUI>().color;
        if (unregistering)
        {
            //change text color depending on "completed"
            if (completed)
                rect.GetComponent<TextMeshProUGUI>().color = Color.green;
            else
                rect.GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        while(rect.anchoredPosition != target)
        {
            if(constant || unregistering)
                rect.anchoredPosition = Vector3.MoveTowards(rect.anchoredPosition, target, 20 * Time.deltaTime * 10);
            else
                rect.anchoredPosition = Vector3.MoveTowards(rect.anchoredPosition, target, (10 + (target - rect.anchoredPosition).magnitude / 10) * Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }

        if (unregistering)
        {
            //put back initial color and disable
            rect.GetComponent<TextMeshProUGUI>().color = initialColor;
            rect.gameObject.SetActive(false);

            //animation to move other mission text to their new position
            bool allMoved = false;
            Vector2 tmpTarget = Vector2.zero;
            while (!allMoved)
            {
                allMoved = true;
                for(int i = 0; i < missions.Count; i++)
                {
                    tmpTarget = new Vector2(missions[i].Rect.anchoredPosition.x, - missions[i].Rect.rect.height * (i + 0.5f));
                    missions[i].Rect.anchoredPosition = Vector2.MoveTowards(missions[i].Rect.anchoredPosition, tmpTarget, 15 * Time.deltaTime * 10);
                    if (missions[i].Rect.anchoredPosition != tmpTarget)
                        allMoved = false;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    #region Timer Range Functions
    public void ResetAllTimers()
    {
        foreach(Mission mission in missions)
        {
            if(mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                tmpMissionTimer.ResetTimer();
            }
        }
    }

    public void PauseAllTimers()
    {
        foreach (Mission mission in missions)
        {
            if (mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                tmpMissionTimer.PauseTimer();
            }
        }
    }

    public void ResumeAllTimers()
    {
        foreach (Mission mission in missions)
        {
            if (mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                tmpMissionTimer.ResumeTimer();
            }
        }
    }

    public void IncreaseAllTimeSpent(float value)
    {
        foreach (Mission mission in missions)
        {
            if (mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                tmpMissionTimer.IncreaseTimeSpent(value);
            }
        }
    }

    public void DecreaseAllTimeSpent(float value)
    {
        foreach (Mission mission in missions)
        {
            if (mission is MissionWithTimer)
            {
                tmpMissionTimer = mission as MissionWithTimer;
                tmpMissionTimer.DecreaseTimeSpent(value);
            }
        }
    }
    #endregion

    private void DisplayTimerText(TextMeshProUGUI tmpro, string missionText, float seconds)
    {
        if (missionText != " ")
            tmpro.text = string.Concat(missionText, " ", SecondsToTimer(seconds));
        else
            tmpro.text = string.Concat(tmpro.text, " ", SecondsToTimer(seconds));
    }

    private void DisplayCounterText(TextMeshProUGUI tmpro, string missionText, bool displayCounter, int goal = 0, int current = 0)
    {
        if (missionText != " ")
        {
            if (displayCounter)
                tmpro.text = string.Concat(missionText, " ", current, "/", goal);
            else
                tmpro.text = missionText;
        }
        else
            if (displayCounter)
                tmpro.text = string.Concat(tmpro.text, " ", current, "/", goal);
    }

    private string SecondsToTimer(float seconds)
    {
        return string.Concat((int)(seconds / 60), ":", Mathf.Abs((int)(seconds % 60)));
    }
}

public abstract class Mission: MonoBehaviour
{
    public abstract RectTransform Rect {
        get;
    }
    public abstract MissionEnabledEvent MissionEnabledEvent
    {
        get;
    }
    public abstract MissionDisabledEvent MissionDisabledEvent
    {
        get;
    }
    public abstract string Text
    {
        get;
    }

    public abstract void Register();

    public abstract void Unregister(bool completed);

    public abstract void ChangeMissionText(string newText);
}

[Serializable]
/// <summary>
/// Each mission has this event and calls it when it is completed
/// </summary>
public class MissionReachedTargetEvent: UnityEvent<Mission> { }

[Serializable]
/// <summary>
/// Each mission has this event and calls it when it is enabled
/// </summary>
public class MissionEnabledEvent: UnityEvent<Mission> { }

[Serializable]
/// <summary>
/// Each mission has this event and calls it when it is disabled
/// </summary>
public class MissionDisabledEvent: UnityEvent<Mission> { }
