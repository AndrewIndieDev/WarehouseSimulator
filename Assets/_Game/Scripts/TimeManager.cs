using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        if (time.Value != dayEndTime)
        {
            Debug.Log($"You can't leave. It's only {currentTimeString}, you need to wait till {GetTimeString(dayEndTime)}");
            return;
        }
        Debug.Log($"Day has ended. New day has started.");
        time.Value = dayStartTime;
    }
    
    private void SortTimeEventsByTime()
    {
        timeEvents.Sort((a, b) => a.time.CompareTo(b.time));
    }
    
    private void Update()
    {
        currentTimeString = GetTimeString(time.Value);
    }

    private IEnumerator TimeLoop()
    {
        yield return null;
        while (true)
        {
            if (time.Value < dayEndTime)
            {
                if (time == null)
                    break;
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
    
    public string GetTimeString(float time)
    {
        int hour = (int)time;
        if (!use24HourClock && hour > 12)
            hour -= 12;
        int minute = (int)((time - (int)time) * 60f);
        if (use24HourClock)
            return hour.ToString("D2") + ":" + minute.ToString("D2");
        return hour.ToString("D2") + ":" + minute.ToString("D2") + " " + ((int)time > 12 ? "pm" : "am");
    }

    private IEnumerator TruckArrived()
    {
        yield return null;
        if (ProductManager.Instance.HasOrderedStock)
        {
            PalletManager.Instance.PlaceOrderInTruck();
            if (!DoorController.receiving.isOpen.Value)
                DoorController.receiving.OpenServerRPC();
        }
    }
    
    private IEnumerator TruckDeparted()
    {
        yield return null;
        if (DoorController.receiving.isOpen.Value)
            DoorController.receiving.CloseServerRPC();
    }
}
