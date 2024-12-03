using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public GameObject[] Walls; // Assign walls of the room in the Inspector
    public string RoomName; // Assign a name to the room in the Inspector
    RoomVisibilityManager roomVisibilityManager = EssentialsManager._instance.roomVisibilityManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
       
            // Check for both the Player tag and the specific collider (e.g., by name or component)
            if (other.CompareTag("Player") && other is CircleCollider2D)
            {
                roomVisibilityManager.PlayerEnteredRoom(this);
                //   Debug.Log("Player entered " + RoomName);
            }
        
    }


}

