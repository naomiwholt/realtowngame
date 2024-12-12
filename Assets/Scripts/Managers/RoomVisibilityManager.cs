using System.Collections.Generic;
using UnityEngine;

public class RoomVisibilityManager : MonoBehaviour
{
   // public static RoomVisibilityManager Instance;

    private List<GameObject[]> activeWalls = new List<GameObject[]>();
    public List<GameObject> allWalls = new List<GameObject>();

    public string ActiveRoomName { get; private set; }

    public void Initialise()
    {
        // Find and store all walls with the tag "Wall" under the "Walls" parent object
        GameObject WallsParentObject = GameObject.Find("Walls");
        if (WallsParentObject != null)
        {
            foreach (Transform child in WallsParentObject.transform)
            {
                if (child.CompareTag("Wall"))
                {
                    allWalls.Add(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogWarning("Walls Parent Object not found in the scene!");
        }
    }

    public void TurnOnAllWalls()
    {
        foreach (var wall in allWalls)
        {
            ToggleWallSprite(wall, true);
        }
    }

    public void PlayerEnteredRoom(RoomTrigger roomTrigger)
    {
        TurnOnAllWalls();

        // Hide wall sprites for the entered room
        foreach (var wall in roomTrigger.Walls)
        {
            ToggleWallSprite(wall, false);
        }

        ActiveRoomName = roomTrigger.RoomName;

        // Keep track of active walls in case you want to revert changes later
        activeWalls.Add(roomTrigger.Walls);
        Debug.Log("Player entered " + roomTrigger.RoomName);
    }

    private void ToggleWallSprite(GameObject wall, bool isVisible)
    {
        SpriteRenderer spriteRenderer = wall.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isVisible; // Enable or disable the sprite
        }
        else
        {
            Debug.LogWarning($"GameObject {wall.name} does not have a SpriteRenderer component!");
        }
    }
}


