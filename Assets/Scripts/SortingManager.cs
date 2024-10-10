using System.Collections.Generic;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    private List<SortingComponent> dynamicObjects = new List<SortingComponent>();
    private List<SortingComponent> staticObjects = new List<SortingComponent>();

    // Registers an object to the appropriate list
    public void RegisterObject(SortingComponent sortingComponent, bool isStatic)
    {
        if (isStatic)
        {
            staticObjects.Add(sortingComponent);
            UpdateSortingForStaticObject(sortingComponent);  // Update once on registration
        }
        else
        {
            dynamicObjects.Add(sortingComponent);
        }
    }

    // Unregister an object when it's destroyed or no longer relevant
    public void UnregisterObject(SortingComponent sortingComponent)
    {
        dynamicObjects.Remove(sortingComponent);
        staticObjects.Remove(sortingComponent);
    }

    private void LateUpdate()
    {
        // Update sorting for dynamic objects (like player or NPCs)
        foreach (var dynamicObject in dynamicObjects)
        {
            dynamicObject.UpdateSortingOrder();
        }
    }

    public void UpdateSortingForStaticObject(SortingComponent sortingComponent)
    {
        sortingComponent.UpdateSortingOrder();  // One-time update for static objects
    }
}
