using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardSymbol
{
	X,
	O
	// Could expand and add whatever symbols we want in the future (Triangles, Squares, etc.)
}

// In a production environment we'd probably be using some sort of ID system, i.e. Their Steam player ID, etc., so I'm creating a PlayerID struct in case we want to store any player related data
public readonly struct PlayerID
{
	public readonly int Id;
	public readonly string Name;
	public readonly BoardSymbol SymbolToUse;
	public readonly bool IsAI;

	public PlayerID(int id, string name, BoardSymbol symbolToUse, bool isAI)
	{
		Id = id;
		Name = name;
		SymbolToUse = symbolToUse;
		IsAI = isAI;
	}
}

public class GameManager : MonoBehaviour
{
	[SerializeField] BoardPiece[] allPieceTypes;
	[SerializeField] int boardSize = 3; // Base board will just be a traditional 3x3
	bool singlePlayerGameMode;
	List<PlayerID> players = new();
	public PlayerID ActivePlayer => players[activePlayerIndex];
	int activePlayerIndex;
	Dictionary<Vector2Int, BoardSymbol?> Board = new();
	List<GameObject> boardPieceObjects = new();
	public delegate void GameEnded(PlayerID winner, bool matchWasDraw);
	public delegate void BoardUpdated(int sizeOfBoard);
	public static event GameEnded OnGameEnded;
	public static event BoardUpdated OnBoardUpdated;
	public static event System.Action<PlayerID> OnActivePlayerChanged;
	public static event System.Action<Vector2Int> OnBoardPiecePlaced;
	public bool IsAITakingTurn { get; private set; }
	public bool IsGameRunning { get; private set; } // Helps prevent players from placing pieces even after the game has already ended

	public void SelectGameMode(bool isSinglePlayer) // When the user presses a button. (We could also just add listeners to the buttons through code as well)
	{
		singlePlayerGameMode = isSinglePlayer;
		StartGame();
	}

	void StartGame()
	{
		IsGameRunning = true;
		CreatePlayers();
		InitializeBoard();
		activePlayerIndex = DetermineStartingPlayerIndex();
		OnActivePlayerChanged?.Invoke(ActivePlayer);
	}

	void InitializeBoard()
	{
		for (int i = 0; i < boardPieceObjects.Count; i++)
		{
			Destroy(boardPieceObjects[i]);
		}
		boardPieceObjects.Clear();
		Board.Clear();
		for (int x = 0; x < boardSize; x++)
		{
			for (int y = 0; y < boardSize; y++)
			{
				Board.Add(new Vector2Int(x, y), null);
			}
		}
		OnBoardUpdated?.Invoke(boardSize);
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
		PlaceBoardPiece(AIPickRandomSpotOnBoard());
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

	Vector2Int AIPickRandomSpotOnBoard()
	{
		List<Vector2Int> vacantSpaces = GetVacantSpaces();
		return vacantSpaces[Random.Range(0, vacantSpaces.Count)];
	}

	public void PlaceBoardPiece(Vector2Int boardPosition)
	{
		if (!Board.TryGetValue(boardPosition, out BoardSymbol? symbol) || symbol != null) return;

		Board[boardPosition] = ActivePlayer.SymbolToUse;
		BoardPiece pieceToPlace = GetBoardPieceFromSymbol(ActivePlayer.SymbolToUse);
		boardPieceObjects.Add(pieceToPlace.PlacePieceOnBoard(boardPosition));
		OnBoardPiecePlaced?.Invoke(boardPosition);

		if (DidActivePlayerWinThisTurn(boardPosition))
		{
			OnGameEnded?.Invoke(ActivePlayer, false);
			IsGameRunning = false;
		}
		else if (GetVacantSpaces().Count == 0) // Draw
		{
			OnGameEnded?.Invoke(ActivePlayer, true);
			IsGameRunning = false;
		}
		else
		{
			MoveToNextPlayerTurn();
		}
	}

	List<Vector2Int> GetVacantSpaces()
	{
		List<Vector2Int> vacantSpaces = new();
		foreach (var space in Board)
		{
			if (space.Value == null) vacantSpaces.Add(space.Key);
		}
		return vacantSpaces;
	}

	BoardPiece GetBoardPieceFromSymbol(BoardSymbol symbol) => System.Array.Find(allPieceTypes, x => x.Symbol == symbol);

	bool DidActivePlayerWinThisTurn(Vector2Int boardPosition) => CheckIfHorizontalVictory(boardPosition.y) || CheckIfVerticalVictory(boardPosition.x) || CheckIfLeftDiagonalVictory() || CheckIfRightDiagonalVictory();

	bool CheckIfHorizontalVictory(int rowIndexToCheck)
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (Board[new Vector2Int(i, rowIndexToCheck)] != ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfVerticalVictory(int columnIndexToCheck)
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (Board[new Vector2Int(columnIndexToCheck, i)] != ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfLeftDiagonalVictory()
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (Board[new Vector2Int(i, i)] != ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfRightDiagonalVictory()
	{
		int x = 0;
		int y = boardSize - 1;
		for (int i = 0; i < boardSize; i++)
		{
			if (Board[new Vector2Int(x, y)] != ActivePlayer.SymbolToUse) return false;
			x++;
			y--;
		}
		return true;
	}
}