using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] private float waterIncrease = 0.2f;

	// Werte zurzeitige Runde
	private int civiliansSaved = 0;
	private float propertyDamage = 0;
	private int housesDestroyed = 0;
	
	// Werte insgesamt
	private int civiliansSavedTotal = 0;
	private float propertyDamageTotal = 0;
	private int housesDestroyedTotal = 0;
	
	// Katastrophen
	private int disastersPrevented = 0;
	private int disastersPreventedTotal = 0;
	private int disastersFailed = 0;

	[Tooltip("How long does it take until the water advances 1 step.")]
	public float waterAdvanceTime = 300.0f;
	private float timeUntilWaterAdvances = 0f;

	public int eventsPerAdvance = 10;

	public bool gameIsPlaying = false;

	[Header("References:")]
	public InWorldMessage inWorldMessagePrefab;

	[Space(5)]
	public TMP_Text timerLabel;

	[SerializeField] GameObject timerObject;

	[Space(5)] [SerializeField] private DamageManager damageManager;

	public event EventHandler<float> WaterIncreased;
	public event Action AdvanceWaterStart;
	public event Action GameHasEnded;
	public void OnWaterIncreased(float newWaterHeight) { WaterIncreased?.Invoke(this, newWaterHeight); }
	public void OnAdvanceWaterStart() { AdvanceWaterStart?.Invoke(); }
	public void OnGameEnd() { GameHasEnded?.Invoke(); }

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		AdvanceWater(true);
	}

	private void Update()
	{
		if (gameIsPlaying)
		{
			timeUntilWaterAdvances -= Time.deltaTime;

			if (timeUntilWaterAdvances <= 0)
			{
				AdvanceWater();
			}
			timerLabel.text = System.TimeSpan.FromSeconds(timeUntilWaterAdvances).ToString("mm':'ss");
		}
		timerObject.SetActive(gameIsPlaying);
	}

	public void AdvanceWater(bool initialUpdate = false)
	{
		timeUntilWaterAdvances = 0;

		timeUntilWaterAdvances = waterAdvanceTime;
		gameIsPlaying = false;
		if (!initialUpdate)
		{
			if (NavManager.Instance.waterSteps >= NavManager.Instance.waterCap)
			{
				OnGameEnd();
				return;
			}
			
			var damage = damageManager.GetTotalValueDamage();
			propertyDamage = damage - propertyDamageTotal;
			propertyDamageTotal = damage;

			civiliansSavedTotal += civiliansSaved;
			disastersPreventedTotal += disastersPrevented;
			housesDestroyedTotal += housesDestroyed;
			//UI Counter deaktivieren
			OnAdvanceWaterStart();
		}
		NavManager.Instance.UpdateWater(() => { gameIsPlaying = initialUpdate; OnWaterIncreased(NavManager.Instance.WaterHeight); }, !initialUpdate);
		
		EventGenerator.Instance.GenerateEvents(eventsPerAdvance);
		civiliansSaved = 0;
		disastersPrevented = 0;
		housesDestroyed = 0;
	}

	public void HouseDestroyed(int id)
	{
		housesDestroyed++;
		if (EventManager.Instance.LocationDestroyed(id)) disastersFailed++;
	}
	
	public void SaveCivilian(Vector3 position)
	{
		civiliansSaved += 1;

		InWorldMessage message = Instantiate(inWorldMessagePrefab.gameObject, this.transform).GetComponent<InWorldMessage>();
		message.ShowMessage(position, "Civilians Saved");
	}

	public void EventFinished()
	{
		disastersPrevented++;
	}

	//Getter
	public int GetCiviliansTotal()
	{
		return EventManager.Instance.CountCurrentEventsOfType(new [] { GameEvent.EventType.Rescue, GameEvent.EventType.CarAccident}) + GetCiviliansSaved();
	}
	
	public int GetCiviliansSaved()
	{
		return civiliansSaved;
	}
	public int GethousesDestroyed()
	{
		return housesDestroyed;
	}
	public int GetciviliansSavedTotal()
	{
		return civiliansSavedTotal;
	}
	public int GethousesDestroyedTotal()
	{
		return housesDestroyedTotal;
	}

	public int GetDisasterPrevented()
	{
		return disastersPrevented;
	}
	
	public int GetDisastersPreventedTotal()
	{
		return disastersPreventedTotal;
	}
	public int GetAmountDisasters()
	{
		return disastersPrevented + disastersFailed +
		       EventManager.Instance.CountCurrentEventsOfType(new []{GameEvent.EventType.Fire, GameEvent.EventType.Rescue, GameEvent.EventType.CarAccident, GameEvent.EventType.PeopleInNeed});
	}
	
	public int GetpropertyDamage()
	{
		return Mathf.RoundToInt(propertyDamage);
	}
	public int GetpropertyDamageTotal()
	{
		return Mathf.RoundToInt(propertyDamageTotal);
	}
}
