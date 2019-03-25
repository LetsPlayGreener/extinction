using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWithTimer : Mission
{
    //Time.time - startingTime = time spent since activation
    private float startingTime;
    //when the timer is paused we set this float to Time.time - startingTime to store the time spent
    //when the timer is resumed we set startingTime to Time.time - paused and set paused to -1
    private float paused = -1;
    /// <summary>
    /// Mission Text displayed on screen
    /// </summary>
    public string text;
    /// <summary>
    /// The mission is over when the time spent reaches duration
    /// </summary>
    public float duration;
    /// <summary>
    /// True if the displayed timer is a count down from duration and false if it goes up from 0
    /// </summary>
    public bool countDown = true;
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
    private bool eventInvoked = false;

    #region Getter/Setter
    public float TimeSpent
    {
        get
        {
            if (paused < 0)
                return Time.time - startingTime;
            else
                return paused;
        }
    }

    public override RectTransform Rect
    {
        get
        {
            return rect;
        }
    }

    public bool DurationReached
    {
        get
        {
            return TimeSpent >= duration;
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
        duration = duration < 0 ? 0 : duration;
        Register();
        ResetTimer();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!eventInvoked && DurationReached)
        {
            eventInvoked = true;
            PauseTimer();
            OnMissionReachedTarget.Invoke(this);
            Unregister();
        }
        else if (!DurationReached)
            eventInvoked = false;

        //update mission text if the timer is not paused
        if(paused < 0)
            MissionsManager.instance.RefreshMissionText(this);
    }

    private void OnEnable()
    {
        rect = this.GetComponent<RectTransform>();
        Register();
    }

    private void OnDisable()
    {
        Unregister();
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

    public override void Unregister(bool completed = false)
    {
        if (MissionsManager.instance)
        {
            MissionsManager.instance.Unregister(this, completed);
        }
    }
    #endregion

    #region Timer Functions
    /// <summary>
    /// Set TimeSpent to 0
    /// </summary>
    public void ResetTimer()
    {
        if (paused >= 0)
            paused = 0;
        startingTime = Time.time;
    }

    public void PauseTimer()
    {
        if(paused < 0)
            paused = Time.time - startingTime;
    }

    public void ResumeTimer()
    {
        if (paused >= 0)
            startingTime = Time.time - paused;
    }

    public void IncreaseTimeSpent(float value)
    {
        if (paused < 0)
            startingTime -= value;
        if (startingTime > Time.time)
            startingTime = Time.time;
        else
        {
            paused += value;
            if (paused < 0)
                paused = 0;
        }
    }

    public void DecreaseTimeSpent(float value)
    {
        if (paused < 0)
            startingTime += value;
        if (startingTime > Time.time)
            startingTime = Time.time;
        else
        {
            paused -= value;
            if (paused < 0)
                paused = 0;
        }
    }
    #endregion
}
