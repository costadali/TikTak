using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board_UI : MonoBehaviour
{
	[SerializeField] GameObject endOfGameMenu;
	[SerializeField] TextMeshProUGUI activePlayer;
	[SerializeField] TextMeshProUGUI gameResultText;

	void Awake()
	{
		GameManager.OnGameEnded += OnGameEnded;
		GameManager.OnActivePlayerChanged += OnActivePlayerChanged;
	}

	void OnDestroy()
	{
		GameManager.OnGameEnded -= OnGameEnded;
		GameManager.OnActivePlayerChanged -= OnActivePlayerChanged;
	}

	void OnActivePlayerChanged(PlayerID player)
	{
		activePlayer.text = $"{player.Name}'s turn ({player.SymbolToUse})";
	}

	public void OnGameEnded(PlayerID winner, bool matchWasDraw)
	{
		endOfGameMenu.SetActive(true);
		gameResultText.text = matchWasDraw ? "Draw!" : $"{winner.Name} wins!";
	}
}