using DG.Tweening;
using UnityEngine;

/// <summary>
/// I decided to store each piece in a scriptable object in case we want to add more pieces / data for each piece in the future
/// </summary>
[CreateAssetMenu(menuName = "BoardPiece")]
public class BoardPiece : ScriptableObject
{
	[field: SerializeField] public BoardSymbol Symbol { get; private set; } // This is a nice way of allowing us to set properties within the inspector
	[SerializeField] GameObject piecePrefab;

	public GameObject PlacePieceOnBoard(Vector2Int boardPosition)
	{
		GameObject obj = Instantiate(piecePrefab, new Vector3(boardPosition.x, 0f, boardPosition.y), Quaternion.identity);

		// Just a cool simple scale effect whenever it gets instantiated
		obj.transform.localScale = Vector3.zero;
		obj.transform.DOScale(Vector3.one, .1f).SetEase(Ease.OutBack);
		return obj;
	}
}