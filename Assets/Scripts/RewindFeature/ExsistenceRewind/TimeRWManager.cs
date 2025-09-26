using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeRWManager : MonoBehaviour
{
    private static TimeRWManager Instance;
    bool bRewinding = false;
    public void SetRewind(bool bInRewind) => bRewinding = bInRewind;
    private float CurrentTime = 0;
    List<RewindableEvent> RewindableEvents = new List<RewindableEvent>();

    public static TimeRWManager GetInst()
    {
        if (Instance == null)
        {
            Instance = FindFirstObjectByType<TimeRWManager>();
            if (Instance == null)
            {
                var timeRWMObject = new GameObject("TimeRWManager");
                Instance = timeRWMObject.AddComponent<TimeRWManager>();
            }
        }
        return Instance;
    }

    private void Awake()
    {
        //Only need one if level has many TimeRWManagers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //auto setup if only one in each level
        Instance = this;
    }
  
    
    const float TOLERANCE = 0.1f;
    void FixedUpdate()
    {
        if (bRewinding)
        {
            CurrentTime -= Time.fixedDeltaTime; //Current Time < 0 is not problem, because this is fake time for handling rewind
            for (int i = RewindableEvents.Count - 1; i >= 0; i--)
            {
                var rwEvent = RewindableEvents[i];
                if (Mathf.Abs((float)(CurrentTime - rwEvent.OccurredTime)) < TOLERANCE)
                {
                    Debug.Log( rwEvent+ "at " + rwEvent.OccurredTime + "Rewind");
                    rwEvent.Rewind();
                    RewindableEvents.RemoveAt(i);
                }
            }

        }
        else
        {
            CurrentTime += Time.fixedDeltaTime;
            RewindableEvents.RemoveAll(rwEvent => rwEvent.IsExpired(CurrentTime));
        }
    }

    public void RecordEvent(RewindableEvent inRewindableEvent)//Ten nay hoi to ra nguy hiem, nhung no tuong tu viec Register
    {
        inRewindableEvent.OccurredTime = CurrentTime;
        RewindableEvents.Add(inRewindableEvent);
        Debug.Log( inRewindableEvent+ " at " + inRewindableEvent.OccurredTime + " Record");
    }
}
