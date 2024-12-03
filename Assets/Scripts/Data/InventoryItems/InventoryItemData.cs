using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon; 
    public GameObject itemPrefab; // Prefab to instantiate in the scene but use specific prefab instance in methods below not this 
    public string description;
    public int maxStackSize; 

    public bool canRotate = true;
   
    public Sprite NorthSprite;
    public Sprite SouthSprite;

    Direction CurrentDirection = Direction.NorthEast;  
    public enum Direction
    {
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest
    }

    //this is to be called in Object placemnt script and will put in that specificc prefab instance
    public void RotateItem(GameObject currentPrefab)
    {
        if (canRotate)
        {
            CurrentDirection = RotateClockwise(CurrentDirection);
        }       
        //can i do this prettier? ehhh
        currentPrefab.GetComponent<SpriteRenderer>().sprite = GetDirectionSprite(CurrentDirection, currentPrefab.GetComponent<SpriteRenderer>());
    }

    public Direction RotateClockwise(Direction currentDirection)
    {
        // Incrementitem direction round the enum
        int nextDirection = ((int)currentDirection + 1) % System.Enum.GetValues(typeof(Direction)).Length;
        return (Direction)nextDirection;
    }


    public Sprite GetDirectionSprite(Direction direction, SpriteRenderer spriteRenderer)
    {
        Sprite currentDirection;        

        switch (direction)
        {
            case Direction.NorthEast:
                currentDirection = NorthSprite;
                spriteRenderer.flipX = false;
                return currentDirection;

            case Direction.SouthEast:
                currentDirection = SouthSprite;
                spriteRenderer.flipX = false;
                return currentDirection;

            case Direction.SouthWest:
                currentDirection = SouthSprite;
                spriteRenderer.flipX = true;
                return currentDirection;

            case Direction.NorthWest:
                currentDirection = NorthSprite;
                spriteRenderer.flipX = true;
                return currentDirection;

            default:
                return icon;
        }

    }



}

