using UnityEngine;

public class GridManager : MonoBehaviour
{
 
    private Grid grid;  // Unity's Grid component

    public GameObject player;  // Reference to the player
 

    public Vector2Int ConvertWorldToGrid(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogWarning("Grid component is missing or has been destroyed.");
            return Vector2Int.zero;  // Handle the case where grid is missing
        }

        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }
}



