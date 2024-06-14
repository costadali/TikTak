using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Allows the player to interact with the board, and controls a placeholder object to indicate where the player is about to place their piece
/// </summary>
public class BoardInteractor : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] GameManager gameManager;
	[SerializeField] BoardManager boardManager;
	[SerializeField] GameObject placeholderPiece; // Shows the player where they are going to place the piece on the board
	[SerializeField] Transform placeholderPieceCenterTransform;
	[SerializeField] ParticleSystem placeParticleEffects;
	Vector3 cursorWorldPosition;
	Vector2Int cursorGridPosition;

	async void Awake() // Awake can also be async!
	{
		await PlaceHolderRotateEffect();
	}

	void Update()
	{
		SetGridPosition();
		CheckIfPlayerClicked();
		UpdatePlaceholderObject();
	}

	async Task PlaceHolderRotateEffect() // I chose to use an async function, but you could just as easily use a coroutine instead
	{
		while (!destroyCancellationToken.IsCancellationRequested)
		{
			placeholderPieceCenterTransform.DORotate(new Vector3(0f, 180f, 0f), .5f);
			await Task.Delay(500);
			placeholderPieceCenterTransform.DORotate(new Vector3(0f, 360f, 0f), .5f);
			await Task.Delay(500);
		}
	}

	void CheckIfPlayerClicked()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0) && CanPlace())
		{
			bool placed = boardManager.PlaceBoardPiece(cursorGridPosition);
			if (placed)
			{
				placeParticleEffects.Play(); // Play some particles when the player places a piece!
			}
		}
	}

	void SetGridPosition()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		Plane plane = new(Vector3.up, Vector3.zero);
		plane.Raycast(ray, out float dist);
		cursorWorldPosition = ray.GetPoint(dist);
		cursorGridPosition = new Vector2Int((int)cursorWorldPosition.x, (int)cursorWorldPosition.z);
	}

	void UpdatePlaceholderObject()
	{
		if (CanPlace() && boardManager.GetPieceAtPosition(cursorGridPosition, out BoardSymbol ? symbol) && symbol == null)
		{
			placeholderPiece.gameObject.SetActive(true);
			placeholderPiece.transform.position = new Vector3(cursorGridPosition.x, 0f, cursorGridPosition.y);
			placeParticleEffects.transform.position = new Vector3(cursorGridPosition.x + .5f, 0f, cursorGridPosition.y + .5f);
		}
		else
		{
			placeholderPiece.gameObject.SetActive(false);
		}
	}

	bool CanPlace()
	{
		// Make sure not to register any clicks below 0 since the board will never go there. Protects against an edge case where the users clicks anywhere from -.999 to 0
		return !gameManager.IsAITakingTurn && gameManager.IsGameRunning && cursorWorldPosition.x > 0f && cursorWorldPosition.z > 0f;
	}
}