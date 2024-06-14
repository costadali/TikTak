using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Main Menu, In-Game, and End of Game UI
/// </summary>
public class Game_UI : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
	[SerializeField] GameObject mainMenuUI;
	[SerializeField] GameObject inGameUI;
	[SerializeField] GameObject endOfGameMenu;
	[SerializeField] TextMeshProUGUI activePlayer;
	[SerializeField] Image activePlayerBackground;
	[SerializeField] TextMeshProUGUI gameResultText;
	[SerializeField] Image gameResultBackground;

	void Awake()
	{
		gameManager.OnGameStarted += OnGameStarted;
		gameManager.OnGameEnded += OnGameEnded;
		gameManager.OnActivePlayerChanged += OnActivePlayerChanged;
		inGameUI.SetActive(false);
		endOfGameMenu.SetActive(false);
	}

	void OnDestroy()
	{
		gameManager.OnGameStarted -= OnGameStarted;
		gameManager.OnGameEnded -= OnGameEnded;
		gameManager.OnActivePlayerChanged -= OnActivePlayerChanged;
	}

	void OnActivePlayerChanged(PlayerID player)
	{
		activePlayer.text = $"{player.Name}'s turn ({player.SymbolToUse})";
		activePlayerBackground.color = player.SymbolToUse == BoardSymbol.X ? Color.red : Color.blue;
	}

	void OnGameStarted()
	{
		inGameUI.SetActive(true);
		mainMenuUI.SetActive(false);
	}

	public void SelectGameMode(bool isSinglePlayer) // When the user presses either 1 Player or 2 Players (We could also just add listeners to the buttons through code as well)
	{
		gameManager.StartGame(isSinglePlayer);
	}

	public void OnGameEnded(PlayerID winner, bool matchWasDraw) // Button
	{
		inGameUI.SetActive(false);
		endOfGameMenu.SetActive(true);
		gameResultText.text = matchWasDraw ? "Draw!" : $"{winner.Name} wins!";
		if (matchWasDraw)
		{
			gameResultBackground.color = Color.green;
		}
		else
		{
			gameResultBackground.color = winner.SymbolToUse == BoardSymbol.X ? Color.red : Color.blue;
		}
	}

	public void ExitBackToMainMenu() // Button
	{
		mainMenuUI.SetActive(true);
		endOfGameMenu.SetActive(false);
	}

	public void ExitGame() => Application.Quit(); // Button
}