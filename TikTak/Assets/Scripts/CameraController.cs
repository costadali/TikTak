using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls the camera and any camera effects
/// </summary>
public class CameraController : MonoBehaviour
{
	[SerializeField] BoardManager boardManager;
	[SerializeField] Volume volume;
	ChromaticAberration chromaticAberration;
	Tween blurEffectTween;

	void Awake()
	{
		volume.profile.TryGet(out ChromaticAberration c);
		chromaticAberration = c;
		boardManager.OnBoardUpdated += OnBoardUpdated;
		boardManager.OnBoardPiecePlaced += OnBoardPiecePlaced;
	}

	void OnDestroy()
	{
		boardManager.OnBoardUpdated -= OnBoardUpdated;
		boardManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;
	}

	void OnBoardUpdated(int sizeOfBoard)
	{
		float halfOfBoard = sizeOfBoard / 2f;
		transform.position = new Vector3(halfOfBoard, 8 + halfOfBoard, -4f); // Center the camera to the middle of the board (these are just some good settings I picked based on the camera I'm using)
	}

	void OnBoardPiecePlaced(Vector2Int pos)
	{
		// Play some cool effects!
		transform.DOShakePosition(.1f, new Vector3(.1f, 0f, .1f));
		if (blurEffectTween.IsActive())
		{
			blurEffectTween.Kill();
		}
		chromaticAberration.intensity.value = 1f;
		blurEffectTween = DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0f, .5f).OnKill(() => blurEffectTween = null);
	}
}