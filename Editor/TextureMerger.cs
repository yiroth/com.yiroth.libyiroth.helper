using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ashrose.Helper.Editor
{
    public class TextureMerger : EditorWindow
    {
        private Texture2D _colorTexture;
        private Texture2D _alphaTexture;

        [MenuItem("Tools/Texture Merger/Alpha Combiner")]
        public static void ShowWindow() => GetWindow<TextureMerger>("Alpha Combiner");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Combine RGB + Alpha", EditorStyles.boldLabel);
            _colorTexture = (Texture2D)EditorGUILayout.ObjectField("Color (RGB)", _colorTexture, typeof(Texture2D), false);
            _alphaTexture = (Texture2D)EditorGUILayout.ObjectField("Alpha (Grayscale)", _alphaTexture, typeof(Texture2D), false);

            EditorGUILayout.Space();

            GUI.enabled = (_colorTexture && _alphaTexture);
            if (GUILayout.Button("Merge and Save PNG"))
            {
                MergeTextures();
            }
            GUI.enabled = true;
        }

        private void MergeTextures()
        {
            int w = _colorTexture.width;
            int h = _colorTexture.height;

            // Ensure we use RGBA32 to support transparency
            Texture2D merged = new Texture2D(w, h, TextureFormat.RGBA32, false);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = _colorTexture.GetPixel(x, y);
                    // Grabbing the Red channel of the alpha texture as the mask
                    float a = _alphaTexture.GetPixel(x, y).r; 
                
                    merged.SetPixel(x, y, new Color(c.r, c.g, c.b, a));
                }
            }

            byte[] bytes = merged.EncodeToPNG();
            string path = AssetDatabase.GetAssetPath(_colorTexture);
            string newPath = Path.GetDirectoryName(path) + "/" + _colorTexture.name + "_Combined.png";
        
            File.WriteAllBytes(newPath, bytes);
            AssetDatabase.Refresh();
        
            Debug.Log("Merged texture saved to: " + newPath);
        }
    }
}

