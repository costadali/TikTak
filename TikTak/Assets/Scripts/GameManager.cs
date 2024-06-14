using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardSymbol
{
	X,
	O
	// Could expand and add whatever symbols we want in the future (Triangles, Squares, etc.)
}

public class GameManager : MonoBehaviour
{
	[SerializeField] BoardManager boardManager;
	bool singlePlayerGameMode;
	int activePlayerIndex;
	List<PlayerID> players = new();

	// Properties
	public bool IsAITakingTurn { get; private set; }
	public bool IsGameRunning { get; private set; } // Helps prevent players from placing pieces even after the game has already ended
	public PlayerID ActivePlayer => players[activePlayerIndex];

	// Events
	public delegate void GameEnded(PlayerID winner, bool matchWasDraw);
	public event GameEnded OnGameEnded;
	public event System.Action OnGameStarted;
	public event System.Action<PlayerID> OnActivePlayerChanged;

	void Awake() => boardManager.OnBoardPiecePlaced += OnBoardPiecePlaced;

	void OnDestroy() => boardManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;

	void OnBoardPiecePlaced(Vector2Int boardPosition)
	{
		if (boardManager.DidActivePlayerWinThisTurn(boardPosition)) // Check if there's a winner!
		{
			OnGameEnded?.Invoke(ActivePlayer, false);
			IsGameRunning = false;
		}
		else if (boardManager.GetVacantSpaces().Count == 0) // Check if it's a draw
		{
			OnGameEnded?.Invoke(ActivePlayer, true);
			IsGameRunning = false;
		}
		else // Keep playing
		{
			MoveToNextPlayerTurn();
		}
	}

	public void StartGame(bool isSinglePlayer)
	{
		singlePlayerGameMode = isSinglePlayer;
		IsGameRunning = true;
		CreatePlayers();
		activePlayerIndex = DetermineStartingPlayerIndex();
		OnActivePlayerChanged?.Invoke(ActivePlayer);
		OnGameStarted?.Invoke();
	}

	void CreatePlayers() // I'm just simply creating 2 players here, but we could also have the players enter their usernames on startup, or in a multiplayer game just use their provided ID, etc.
	{
		players.Clear();
		players.Add(new PlayerID(0, "Player 1", BoardSymbol.X, false)); // Player 1 will always be human controlled for this game
		players.Add(new PlayerID(1, "Player 2", BoardSymbol.O, singlePlayerGameMode)); // Player 2 will be either human/AI controlled depending on the game mode
	}

	void MoveToNextPlayerTurn()
	{
		activePlayerIndex++;
		if (activePlayerIndex >= players.Count)
		{
			activePlayerIndex = 0;
		}
		OnActivePlayerChanged?.Invoke(ActivePlayer);
		if (ActivePlayer.IsAI)
		{
			StartCoroutine(AITakeTurnCo());
		}
	}

	IEnumerator AITakeTurnCo()
	{
		IsAITakingTurn = true;
		yield return new WaitForSeconds(1f); // Simulate AI thinking for a second
		IsAITakingTurn = false;
		boardManager.PlaceBoardPiece(boardManager.PickRandomSpotOnBoard());
	}

	int DetermineStartingPlayerIndex()
	{
		if (singlePlayerGameMode)
		{
			return 0; // When playing against the AI, the player will always get to go first
			// Could also just randomly pick a starting player like this: return Random.Range(0, players.Count);
		}
		else
		{
			return players.FindIndex(x => x.SymbolToUse == BoardSymbol.X); // In 2 player mode, X will go first
		}
	}
}