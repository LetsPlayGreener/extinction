using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWithCounter : Mission
{
    private int count = 0;
    /// <summary>
    /// Mission Text displayed on screen
    /// </summary>
    public string text;
    /// <summary>
    /// The mission is completed when the count reaches CountGoal
    /// </summary>
    public int CountGoal;
    /// <summary>
    /// If true the count progression will be displayed in the mission text
    /// </summary>
    public bool displayCounter = true;
    public bool countWhileDisabled = false;
    /// <summary>
    /// Each mission has this event and calls it when it is completed
    /// </summary>
    public MissionReachedTargetEvent OnMissionReachedTarget;
    /// <summary>
    /// Each mission has this event and calls it when it is enabled
    /// </summary>
    public MissionEnabledEvent OnMissionEnabled;
    /// <summary>
    /// Each mission has this event and calls it when it is disabled
    /// </summary>
    public MissionDisabledEvent OnMissionDisabled;

    private RectTransform rect;

    #region Getter/Setter
    public int Count
    {
        get
        {
            return count;
        }
    }

    public override RectTransform Rect
    {
        get
        {
            return rect;
        }
    }

    public bool GoalReached
    {
        get
        {
            return count >= CountGoal;
        }
    }

    public override MissionEnabledEvent MissionEnabledEvent
    {
        get
        {
            return OnMissionEnabled;
        }
    }

    public override MissionDisabledEvent MissionDisabledEvent
    {
        get
        {
            return OnMissionDisabled;
        }
    }

    public override string Text
    {
        get
        {
            return text;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        Register();
        if (GoalReached)
        {
            OnMissionReachedTarget.Invoke(this);
            Unregister(true);
        }
    }

    private void OnEnable()
    {
        rect = this.GetComponent<RectTransform>();
        Register();
    }

    private void OnDisable()
    {
        Unregister(GoalReached);
    }

    public override void ChangeMissionText(string newText)
    {
        text = newText;
        MissionsManager.instance.RefreshMissionText(this);
    }

    #region Registration to MissionManager
    /// <summary>
    /// Register this mission to the list in MissionManager.
    /// Does nothing if this mission is already registered
    /// </summary>
    public override void Register()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        StartCoroutine(TryRegister());
    }

    private IEnumerator TryRegister()
    {
        while (!MissionsManager.instance)
            yield return new WaitForSeconds(0.1f);
        MissionsManager.instance.Register(this);
    }

    public override void Unregister(bool completed)
    {
        if (MissionsManager.instance)
        {
            MissionsManager.instance.Unregister(this, completed);
        }
    }
    #endregion

    #region Counter Functions
    public void IncreaseCount(int value = 1)
    {
        if (countWhileDisabled || gameObject.activeInHierarchy)
        {
            count += value;
            if (count < 0)
                count = 0;
            MissionsManager.instance.RefreshMissionText(this);
            if (GoalReached)
            {
                OnMissionReachedTarget.Invoke(this);
                Unregister(true);
            }
        }
    }

    public void DecreaseCount(int value = 1)
    {
        if (countWhileDisabled || gameObject.activeInHierarchy)
        {
            count -= value;
            if (count < 0)
                count = 0;
            MissionsManager.instance.RefreshMissionText(this);
            if (GoalReached)
            {
                OnMissionReachedTarget.Invoke(this);
                Unregister(true);
            }
        }
    }

    public void SetCountToValue(int value)
    {
        if (countWhileDisabled || gameObject.activeInHierarchy)
        {
            count = value;
            if (count < 0)
                count = 0;
            MissionsManager.instance.RefreshMissionText(this);
            if (GoalReached)
            {
                OnMissionReachedTarget.Invoke(this);
                Unregister(true);
            }
        }
    }

    public void IncreaseCountGoal(int value = 1)
    {
        CountGoal += value;
        if (CountGoal < 0)
            CountGoal = 0;
        MissionsManager.instance.RefreshMissionText(this);
        if (GoalReached)
        {
            OnMissionReachedTarget.Invoke(this);
            Unregister(true);
        }
    }

    public void DecreaseCountGoal(int value = 1)
    {
        CountGoal -= value;
        if (CountGoal < 0)
            CountGoal = 0;
        MissionsManager.instance.RefreshMissionText(this);
        if (GoalReached)
        {
            OnMissionReachedTarget.Invoke(this);
            Unregister(true);
        }
    }

    public void SetCountGoalToValue(int value)
    {
        CountGoal = value;
        if (CountGoal < 0)
            CountGoal = 0;
        MissionsManager.instance.RefreshMissionText(this);
        if (GoalReached)
        {
            OnMissionReachedTarget.Invoke(this);
            Unregister(true);
        }
    }
    #endregion
}
