using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the ambiance and sound effects
/// </summary>
public class AudioManager : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
	[SerializeField] BoardManager boardManager;
	[SerializeField] AudioSource gameEndedSound;
	[SerializeField] AudioSource placeBoardPieceSound;

	void Awake()
	{
		gameManager.OnGameEnded += OnGameEnded;
		boardManager.OnBoardPiecePlaced += OnBoardPiecePlaced;
	}

	private void OnDestroy()
	{
		gameManager.OnGameEnded -= OnGameEnded;
		boardManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;
	}

	void OnBoardPiecePlaced(Vector2Int obj) => placeBoardPieceSound.Play();

	void OnGameEnded(PlayerID winner, bool matchWasDraw) => gameEndedSound.Play();
}