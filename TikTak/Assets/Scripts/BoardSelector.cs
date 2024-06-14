using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelector : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] GameManager gameManager;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0) && !gameManager.IsAITakingTurn && gameManager.IsGameRunning)
		{
			RaycastBoardSpace();
		}
	}

	void RaycastBoardSpace()
	{
		Vector3 mousePos = Input.mousePosition;
		Plane plane = new(Vector3.up, Vector3.zero);
		Ray ray = mainCamera.ScreenPointToRay(mousePos);
		plane.Raycast(ray, out float dist);
		Vector3 worldPos = ray.GetPoint(dist);
		Vector2Int boardPos = new((int)worldPos.x, (int)worldPos.z);
		gameManager.PlaceBoardPiece(boardPos);
	}
}