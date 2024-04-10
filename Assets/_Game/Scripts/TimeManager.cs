using System;
using System.Collections.Generic;
using UnityEngine;

public enum ETimeEventType
{
    None,
    TruckArrived,
    TruckDeparted
}

[System.Serializable]
public class TimeEventData
{
    public int time;
    public ETimeEventType eventType;
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    [SerializeField] private float dayStartTime = 6f;
    [SerializeField] private float dayEndTime = 18f;
    [SerializeField] private float secondsPerHour = 60f;
    [SerializeField] private bool use24HourClock = true;
    [SerializeField] private List<TimeEventData> timeEvents = new List<TimeEventData>();

    
    private float time = 0f;
    [HideInInspector] public string currentTimeString;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timeEvents.Sort((a, b) => a.time.CompareTo(b.time));
    }

    public void StartDay()
    {
        time = dayStartTime;
    }

    public void EndDay()
    {
        
    }
    
    private void Update()
    {
        if (time < dayEndTime)
            time = Mathf.Clamp(time + Time.deltaTime / secondsPerHour, dayStartTime, dayEndTime);
        currentTimeString = GetTimeString();
    }
    
    public string GetTimeString()
    {
        int hour = (int)time;
        if (!use24HourClock && hour > 12)
            hour -= 12;
        int minute = (int)((time - (int)time) * 60f);
        if (use24HourClock)
            return hour.ToString("D2") + ":" + minute.ToString("D2");
        return hour.ToString("D2") + ":" + minute.ToString("D2") + " " + ((int)time > 12 ? "pm" : "am");
    }
}
