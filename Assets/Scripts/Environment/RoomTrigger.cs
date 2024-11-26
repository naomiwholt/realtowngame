using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public GameObject[] Walls; // Assign walls of the room in the Inspector
    public string RoomName; // Assign a name to the room in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomVisibilityManager.Instance.PlayerEnteredRoom(this);
        }
        Debug.Log("Player entered " + RoomName);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomVisibilityManager.Instance.PlayerExitedRoom(this);
        }
    }
}
