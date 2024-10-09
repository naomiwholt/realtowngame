using UnityEditor;
using UnityEngine;

public class SortingOrderBaker : EditorWindow
{
    [MenuItem("Tools/Bake Sorting Order")]
    public static void ShowWindow()
    {
        GetWindow<SortingOrderBaker>("Bake Sorting Order");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Bake Sorting Order for Selected Objects"))
        {
            BakeSortingOrder();
        }
    }

    private static void BakeSortingOrder()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(obj.transform.position.y * -1);
                EditorUtility.SetDirty(obj); 
            }
        }
    }
}
