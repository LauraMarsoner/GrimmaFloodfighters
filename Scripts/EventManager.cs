using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private readonly Dictionary<int, GameEvent> _undiscoveredEvents = new Dictionary<int, GameEvent>();
    private readonly Dictionary<int, GameEvent> _discoveredEvents = new Dictionary<int, GameEvent>();

    private readonly HashSet<EventSpotter> _eventSpotters = new HashSet<EventSpotter>();

    private int eventIndex = 0;

    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        eventIndex = EventGenerator.Instance.GetStartOffset(GameEvent.EventType.Custom);
    }

    public void Update()
    {
        if (!GameManager.instance.gameIsPlaying) return;
        DiscoverEvents();
        UpdateEvents();
    }

    public IEnumerable<GameEvent> GetNearbyEvents(Vector3 position)
    {
        return _discoveredEvents.Select(e => e.Value).Where(e => (e.transform.position - position).sqrMagnitude < e.range * e.range);
    }

    private void UpdateEvents()
    {
        HashSet<int> toRemove = new HashSet<int>();
        foreach (var (k, e) in _discoveredEvents)
        {
            e.time -= Time.deltaTime * e.WorkingCount;
            if (e.time <= 0.0)
            {
                toRemove.Add(k);
                e.active = false;
                GameManager.instance.EventFinished();
                EventGenerator.Instance.FreeLocation(k);
                e.SuccessCallback?.Invoke();
                EventLog.Instance.LogMessage(e.GetFinishedMessage(),new []{e.location.position});
                Destroy(e.gameObject);
            }
        }
        foreach (var i in toRemove)
        {
            _discoveredEvents.Remove(i);
        }
    }

    public void AddEvent(GameEvent gameEvent, int index = -1)
    {
        if (index == -1)
        {
            index = eventIndex++;
        }
        _undiscoveredEvents[index] = gameEvent;
    }

    public bool LocationDestroyed(int index)
    {
        EventGenerator.Instance.LockLocation(index);
        if (_discoveredEvents.ContainsKey(index))
        {
            var e = _discoveredEvents[index];
            _discoveredEvents.Remove(index);
            Destroy(e.gameObject);
            return true;
        }
        if (_undiscoveredEvents.ContainsKey(index))
        {
            var e = _undiscoveredEvents[index];
            _undiscoveredEvents.Remove(index);
            Destroy(e.gameObject);
            return true;
        }
        return false;
    }

    public bool TryGetByIndex(int index, out GameEvent gameEvent)
    {
        if (_discoveredEvents.ContainsKey(index))
        {
            gameEvent = _discoveredEvents[index];
            return true;
        }
        if (_undiscoveredEvents.ContainsKey(index))
        {
            gameEvent = _undiscoveredEvents[index];
            return true;
        }
        gameEvent = null;
        return false;
    }

    public int CountCurrentEventsOfType(GameEvent.EventType[] eventTypes)
    {
        return _discoveredEvents.Count(e => eventTypes.Contains(e.Value.type)) + _undiscoveredEvents.Count(e => eventTypes.Contains(e.Value.type));
    }

    private void DiscoverEvents()
    {
        HashSet<int> toRemove = new HashSet<int>();
        foreach (var (k, e) in _undiscoveredEvents)
        {
            var count = _eventSpotters
                .Count(s => (s.transform.position - e.transform.position).sqrMagnitude < (s.range + e.range) * (s.range + e.range));
            e.time -= Time.deltaTime * count;
            if (e.time <= 0.0f)
            {
                toRemove.Add(k);
                e.time = GetEventTime(e.type, e.severity);
                e.active = true;
                _discoveredEvents[k] = e;
                EventLog.Instance.LogMessage(e.GetDiscoveredMessage(),new []{e.location.position});
            }
        }
        foreach (var i in toRemove)
        {
            _undiscoveredEvents.Remove(i);
        }
    }

    // Returns the required time for an event of a certain severity
    public static float GetEventTime(GameEvent.EventType type, float severity)
    {
            switch (type)
        {
            case GameEvent.EventType.Fire:
                return 20;
            case GameEvent.EventType.Rescue:
                return 15;
            case GameEvent.EventType.CarAccident:
                return 10;
            case GameEvent.EventType.PeopleInNeed:
                return 15;
            case GameEvent.EventType.Custom:
                return severity;
        }
        return severity;
    }
    
    public void RegisterSpotter(EventSpotter spotter)
    {
        _eventSpotters.Add(spotter);
    }
    
    public void UnRegisterSpotter(EventSpotter spotter)
    {
        _eventSpotters.Remove(spotter);
    }
}