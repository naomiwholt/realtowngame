using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureReplacerTool : EditorWindow
{
    private Vector2 scrollPosition;
    private List<Texture2D> oldTextures = new List<Texture2D>();
    private Dictionary<Texture2D, Texture2D> textureMap = new Dictionary<Texture2D, Texture2D>();

    [MenuItem("Tools/Texture Replacer")]
    public static void ShowWindow()
    {
        GetWindow<TextureReplacerTool>("Texture Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Old Textures to New Textures", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Selected Textures"))
        {
            LoadTextures();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(400));

        List<Texture2D> keys = new List<Texture2D>(textureMap.Keys);
        foreach (Texture2D oldTexture in keys)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(oldTexture.name, GUILayout.Width(200));
            textureMap[oldTexture] = (Texture2D)EditorGUILayout.ObjectField(textureMap[oldTexture], typeof(Texture2D), false, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Replace Textures"))
        {
            ReplaceTextures();
        }
    }

    private void LoadTextures()
    {
        oldTextures.Clear();
        textureMap.Clear();

        foreach (Object obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                Texture2D texture = (Texture2D)obj;
                if (!oldTextures.Contains(texture))
                {
                    oldTextures.Add(texture);
                    textureMap.Add(texture, null);
                }
            }
        }

        if (oldTextures.Count == 0)
        {
            Debug.LogWarning("No textures selected. Please select texture files in the Project window.");
        }
    }

    private void ReplaceTextures()
    {
        foreach (var pair in textureMap)
        {
            Debug.Log($"Replace {pair.Key.name} with {(pair.Value != null ? pair.Value.name : "NULL")}");
            // Implement your replacement logic here.
            // This example simply logs the intended replacements.
        }
    }
}

