using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardBuilder : MonoBehaviour
{
	[SerializeField] TileBase groundTile;
	[SerializeField] TileBase waterTile;
	[SerializeField] Tilemap groundTilemap;
	[SerializeField] Tilemap waterTilemap;
	int waterTileBuffer = 10; // Just a buffer to make sure that the ground tiles are always surrounded by water to make sure the user doesn't see the edge of the ocean

	void Awake() => GameManager.OnBoardUpdated += BuildBoard;

	void OnDestroy() => GameManager.OnBoardUpdated -= BuildBoard;

	void BuildBoard(int sizeOfBoard)
	{
		SetGroundTiles(sizeOfBoard);
		SetWaterTiles(sizeOfBoard);
	}

	void SetGroundTiles(int size)
	{
		groundTilemap.ClearAllTiles();
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
			}
		}
	}

	void SetWaterTiles(int size)
	{
		waterTilemap.ClearAllTiles();
		for (int x = 0 - waterTileBuffer; x < size + waterTileBuffer; x++)
		{
			for (int y = 0 - waterTileBuffer; y < size + waterTileBuffer; y++)
			{
				Vector3Int pos = new(x, y, 0);
				if (!groundTilemap.HasTile(pos)) waterTilemap.SetTile(pos, waterTile);
			}
		}
	}
}