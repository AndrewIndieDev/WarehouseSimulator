using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public enum ETimeEventType
{
    None,
    TruckArrived,
    TruckDeparted,
    Order,
    RandomOrder
}

[System.Serializable]
public class TimeEventData
{
    public float time;
    public ETimeEventType eventType;
}

[System.Serializable]
public class RandomTimeEventData
{
    public ETimeEventType eventType;
    [Range(0.0f, 24.0f)]
    public float minTime = 6.0f;
    [Range(0.0f, 24.0f)]
    public float maxTime = 18.0f;
    [Range(0.0f, 100.0f)]
    public float chance = 50.0f;
    public int amountMin = 1;
    public int amountMax = 1;
}

public class TimeManager : NetworkBehaviour
{
    public static TimeManager Instance;
    
    [SerializeField] private float dayStartTime = 6f;
    [SerializeField] private float dayEndTime = 18f;
    [SerializeField] private float secondsPerHour = 60f;
    [SerializeField] private bool use24HourClock = true;
    [SerializeField] private float timeTickInterval = 0.1f;
    [SerializeField] private List<TimeEventData> defaultTimeEvents = new List<TimeEventData>();
    [SerializeField] private List<RandomTimeEventData> randomTimeEvents = new List<RandomTimeEventData>();
    
    int nextEventIndex = 0;

    private NetworkVariable<float> time = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [HideInInspector] public string currentTimeString;
    
    List<TimeEventData> timeEvents = new List<TimeEventData>();
    
    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsServer)
            StartDay();
    }

    public void StartDay()
    {
        nextEventIndex = 0;
        time.Value = dayStartTime;
        timeEvents = new List<TimeEventData>(defaultTimeEvents);
        foreach (var randomTimeEvent in randomTimeEvents)
        {
            if (UnityEngine.Random.Range(0f, 100f) < randomTimeEvent.chance)
            {
                int amount = UnityEngine.Random.Range(randomTimeEvent.amountMin, randomTimeEvent.amountMax + 1);
                for (int i = 0; i < amount; i++)
                {
                    TimeEventData randomTimeEventData = new TimeEventData();
                    randomTimeEventData.time = UnityEngine.Random.Range(randomTimeEvent.minTime, randomTimeEvent.maxTime);
                    randomTimeEventData.eventType = randomTimeEvent.eventType;
                    timeEvents.Add(randomTimeEventData);
                }
            }
        }
        SortTimeEventsByTime();
        StartCoroutine(TimeLoop());
    }

    public void EndDay()
    {
        
    }
    
    private void SortTimeEventsByTime()
    {
        timeEvents.Sort((a, b) => a.time.CompareTo(b.time));
    }
    
    private void Update()
    {
        currentTimeString = GetTimeString();
    }

    private IEnumerator TimeLoop()
    {
        yield return null;
        while (true)
        {
            if (time.Value < dayEndTime)
            {
                time.Value = Mathf.Clamp(time.Value + timeTickInterval / secondsPerHour, dayStartTime, dayEndTime);
                if (timeEvents.IsValidIndex(nextEventIndex) && time.Value >= timeEvents[nextEventIndex].time)
                {
                    ExecuteTimeEvent(timeEvents[nextEventIndex]);
                    nextEventIndex++;
                }
            }

            yield return new WaitForSeconds(timeTickInterval);
        }
    }

    public void ExecuteTimeEvent(TimeEventData timeEventData)
    {
        Debug.Log("Executing time event: " + timeEventData.eventType + " at time: " + timeEventData.time);
        switch (timeEventData.eventType)
        {
            case ETimeEventType.TruckArrived:
                StartCoroutine(TruckArrived());
                break;
            case ETimeEventType.TruckDeparted:
                StartCoroutine(TruckDeparted());
                break;
            case ETimeEventType.Order:
                ProductManager.Instance.GenerateNewRandomOrder();
                break;
            case ETimeEventType.RandomOrder:
                ProductManager.Instance.GenerateNewRandomOrder();
                break;
        }
    }
    
    public string GetTimeString()
    {
        int hour = (int)time.Value;
        if (!use24HourClock && hour > 12)
            hour -= 12;
        int minute = (int)((time.Value - (int)time.Value) * 60f);
        if (use24HourClock)
            return hour.ToString("D2") + ":" + minute.ToString("D2");
        return hour.ToString("D2") + ":" + minute.ToString("D2") + " " + ((int)time.Value > 12 ? "pm" : "am");
    }

    private IEnumerator TruckArrived()
    {
        yield return null;
    }
    
    private IEnumerator TruckDeparted()
    {
        yield return null;
    }
}
