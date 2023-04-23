using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
	public string pauseGameTitle = "Game Paused";
	public string endGameTitle = "Final Results";

	[Header("UI Components:")]

	public TMP_Text title;
	[Space(5)]
	public TMP_Text civiliansSavedTotalText;
	public TMP_Text propertyDamagePreventedTotalText;
	public TMP_Text propertyDamageTotalText;
	public TMP_Text housesFloodedTotalText;
	public TMP_Text disastersPreventedText;

	private bool gameHasEnded = false;

	public void ShowEndScreen()
	{
		if (gameHasEnded)
			return;

		gameObject.SetActive(true);
		gameHasEnded = true;
		title.text = endGameTitle;
		UpdateUI();
	}

	public void ShowPauseScreen()
	{
		if (gameHasEnded || GameManager.instance.gameIsPlaying == false)
			return;

		GameManager.instance.gameIsPlaying = false;
		gameObject.SetActive(true);
		title.text = pauseGameTitle;
		UpdateUI();
	}

	private void UpdateUI()
	{
		var _gameManager = GameManager.instance;
		civiliansSavedTotalText.text = "Civilians saved: " + _gameManager.GetciviliansSavedTotal() + "/" + (_gameManager.GetciviliansSavedTotal() + _gameManager.GetCiviliansTotal());

		propertyDamagePreventedTotalText.text = "Disasters prevented: " + _gameManager.GetDisastersPreventedTotal() + "/" + _gameManager.GetAmountDisasters();

		propertyDamageTotalText.text = "Property damage: " + _gameManager.GetpropertyDamageTotal();

		housesFloodedTotalText.text = "Houses destroyed: " + _gameManager.GethousesDestroyedTotal();

		disastersPreventedText.text = "Overall disasters prevented: " + _gameManager.GetDisasterPrevented() + "/" + _gameManager.GetDisastersPreventedTotal();
	}

	public void Hide()
	{
		if (gameHasEnded)
			return;

		GameManager.instance.gameIsPlaying = true;
		gameObject.SetActive(false);
	}

	public void PlayAgain()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void MainMenu()
	{

		SceneManager.LoadScene("Menu");
	}
}
