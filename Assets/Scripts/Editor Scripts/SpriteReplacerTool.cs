using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteReplacerTool : EditorWindow
{
    private Vector2 scrollPosition;
    private List<Sprite> oldSprites = new List<Sprite>();
    private Dictionary<Sprite, Sprite> spriteMap = new Dictionary<Sprite, Sprite>();

    [MenuItem("Tools/Sprite Replacer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteReplacerTool>("Sprite Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Old Sprites to New Sprites", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Selected Sprites"))
        {
            LoadSprites();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(400));

        List<Sprite> keys = new List<Sprite>(spriteMap.Keys);
        foreach (Sprite oldSprite in keys)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(oldSprite.name, GUILayout.Width(200));
            spriteMap[oldSprite] = (Sprite)EditorGUILayout.ObjectField(spriteMap[oldSprite], typeof(Sprite), false, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Replace Sprites"))
        {
            ReplaceSprites();
        }
    }

    private void LoadSprites()
    {
        oldSprites.Clear();
        spriteMap.Clear();

        foreach (Object obj in Selection.objects)
        {
            if (obj is Sprite)
            {
                Sprite sprite = (Sprite)obj;
                if (!oldSprites.Contains(sprite))
                {
                    oldSprites.Add(sprite);
                    spriteMap.Add(sprite, null);
                }
            }
        }

        if (oldSprites.Count == 0)
        {
            Debug.LogWarning("No sprites selected. Please select sprites in the Project window.");
        }
    }

    private void ReplaceSprites()
    {
        foreach (var pair in spriteMap)
        {
            Debug.Log($"Replace {pair.Key.name} with {(pair.Value != null ? pair.Value.name : "NULL")}");
            // Implement your replacement logic here.
            // This example simply logs the intended replacements.
        }
    }
}

