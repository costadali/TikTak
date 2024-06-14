using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the state of the board
/// </summary>
public class BoardManager : MonoBehaviour
{
	[SerializeField] int boardSize = 3; // Base board will just be a traditional 3x3
	[SerializeField] GameManager gameManager;
	[SerializeField] BoardPiece[] allPieceTypes;
	List<GameObject> boardPieceObjects = new();
	readonly Dictionary<Vector2Int, BoardSymbol?> board = new();

	// Events
	public delegate void BoardUpdated(int sizeOfBoard);
	public event BoardUpdated OnBoardUpdated;
	public event System.Action<Vector2Int> OnBoardPiecePlaced;

	void Awake() => gameManager.OnGameStarted += InitializeBoard;

	void OnDestroy() => gameManager.OnGameStarted -= InitializeBoard;

	public bool PlaceBoardPiece(Vector2Int boardPosition)
	{
		if (!board.TryGetValue(boardPosition, out BoardSymbol? symbol) || symbol != null) return false;

		board[boardPosition] = gameManager.ActivePlayer.SymbolToUse;
		BoardPiece pieceToPlace = GetBoardPieceFromSymbol(gameManager.ActivePlayer.SymbolToUse);
		boardPieceObjects.Add(pieceToPlace.PlacePieceOnBoard(boardPosition));
		OnBoardPiecePlaced?.Invoke(boardPosition);
		return true;
	}

	void InitializeBoard()
	{
		for (int i = 0; i < boardPieceObjects.Count; i++)
		{
			Destroy(boardPieceObjects[i]);
		}
		boardPieceObjects.Clear();
		board.Clear();
		for (int x = 0; x < boardSize; x++)
		{
			for (int y = 0; y < boardSize; y++)
			{
				board.Add(new Vector2Int(x, y), null);
			}
		}
		OnBoardUpdated?.Invoke(boardSize);
	}

	BoardPiece GetBoardPieceFromSymbol(BoardSymbol symbol) => System.Array.Find(allPieceTypes, x => x.Symbol == symbol);

	public bool GetPieceAtPosition(Vector2Int position, out BoardSymbol? symbol) // This is a good way to other classes from being able to directly access this board dictionary
	{
		symbol = null;
		return board.TryGetValue(position, out symbol);
	}

	public Vector2Int PickRandomSpotOnBoard()
	{
		List<Vector2Int> vacantSpaces = GetVacantSpaces();
		return vacantSpaces[Random.Range(0, vacantSpaces.Count)];
	}

	public List<Vector2Int> GetVacantSpaces()
	{
		List<Vector2Int> vacantSpaces = new();
		foreach (var space in board)
		{
			if (space.Value == null) vacantSpaces.Add(space.Key);
		}
		return vacantSpaces;
	}

	#region Win condition checks
	public bool DidActivePlayerWinThisTurn(Vector2Int boardPosition) => CheckIfHorizontalVictory(boardPosition.y) || CheckIfVerticalVictory(boardPosition.x) || CheckIfLeftDiagonalVictory() || CheckIfRightDiagonalVictory();

	bool CheckIfHorizontalVictory(int rowIndexToCheck)
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (board[new Vector2Int(i, rowIndexToCheck)] != gameManager.ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfVerticalVictory(int columnIndexToCheck)
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (board[new Vector2Int(columnIndexToCheck, i)] != gameManager.ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfLeftDiagonalVictory()
	{
		for (int i = 0; i < boardSize; i++)
		{
			if (board[new Vector2Int(i, i)] != gameManager.ActivePlayer.SymbolToUse) return false;
		}
		return true;
	}

	bool CheckIfRightDiagonalVictory()
	{
		int x = 0;
		int y = boardSize - 1;
		for (int i = 0; i < boardSize; i++)
		{
			if (board[new Vector2Int(x, y)] != gameManager.ActivePlayer.SymbolToUse) return false;
			x++;
			y--;
		}
		return true;
	}
	#endregion
}