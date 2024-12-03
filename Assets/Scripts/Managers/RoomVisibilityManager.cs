using System.Collections.Generic;
using UnityEngine;

public class RoomVisibilityManager : MonoBehaviour
{
    public static RoomVisibilityManager Instance;

    private List<GameObject[]> activeWalls = new List<GameObject[]>();
    public List<GameObject> allWalls = new List<GameObject>();

    public string ActiveRoomName { get; private set; }
    public void Initialise()
    {
        //will need to change this to a more generaic background name for all scvenes in the game later on
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
            wall.SetActive(true);
        }
    }

    public void PlayerEnteredRoom(RoomTrigger roomTrigger)
    {
        TurnOnAllWalls();
        // Hide walls for the entered room
        foreach (var wall in roomTrigger.Walls)
        {
            wall.SetActive(false);
        }

        ActiveRoomName = roomTrigger.RoomName;
        // Keep track of active walls in case you want to revert changes later
        activeWalls.Add(roomTrigger.Walls);
        Debug.Log("Player entered " + roomTrigger.RoomName);
    }

}
