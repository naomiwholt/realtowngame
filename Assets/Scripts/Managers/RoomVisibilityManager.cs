using System.Collections.Generic;
using UnityEngine;

public class RoomVisibilityManager : MonoBehaviour
{
    public static RoomVisibilityManager Instance;

    private List<GameObject[]> activeWalls = new List<GameObject[]>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayerEnteredRoom(RoomTrigger roomTrigger)
    {
        // Hide walls for the entered room
        foreach (var wall in roomTrigger.Walls)
        {
            wall.SetActive(false);
        }

        // Keep track of active walls in case you want to revert changes later
        activeWalls.Add(roomTrigger.Walls);
        Debug.Log("Player entered " + roomTrigger.RoomName);
    }

    public void PlayerExitedRoom(RoomTrigger roomTrigger)
    {
        // Show walls for the exited room
        foreach (var wall in roomTrigger.Walls)
        {
            wall.SetActive(true);
        }

        // Remove from active wall tracking
        activeWalls.Remove(roomTrigger.Walls);
        Debug.Log("Player exited " + roomTrigger.RoomName);
    }
}
