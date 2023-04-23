using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndScreenEnabler : MonoBehaviour
{
	[SerializeField] private EndScreen screen;

	private GrimmaFloodfighters inputActions;

	private void Awake()
	{
		inputActions = new GrimmaFloodfighters();

		inputActions.Player.Escape.performed += OnEscape;
		inputActions.Player.Cheat.performed += _ => GameManager.instance.AdvanceWater();
	}

	void Start()
	{
		GameManager.instance.GameHasEnded += () => screen.ShowEndScreen();
		screen.gameObject.SetActive(false);
	}

	private void OnEscape(InputAction.CallbackContext obj)
	{
		if (screen.gameObject.activeSelf)
			screen.Hide();
		else
			screen.ShowPauseScreen();
	}

	private void OnEnable() { inputActions.Enable(); }
	private void OnDisable() { inputActions.Disable(); }
}
