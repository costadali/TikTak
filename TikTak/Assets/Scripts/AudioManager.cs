using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] AudioSource gameEndedSound;
	[SerializeField] AudioSource placeBoardPieceSound;

	void Awake()
	{
		GameManager.OnGameEnded += OnGameEnded;
		GameManager.OnBoardPiecePlaced += OnBoardPiecePlaced;
	}

	private void OnDestroy()
	{
		GameManager.OnGameEnded -= OnGameEnded;
		GameManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;
	}

	void OnBoardPiecePlaced(Vector2Int obj) => placeBoardPieceSound.Play();

	void OnGameEnded(PlayerID winner, bool matchWasDraw) => gameEndedSound.Play();
}