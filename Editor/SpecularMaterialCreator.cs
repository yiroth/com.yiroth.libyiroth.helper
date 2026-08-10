using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ashrose.Helper.Editor
{
    public class SpecularMaterialCreator
    {
        private static readonly int WorkflowMode = Shader.PropertyToID("_WorkflowMode");

        [MenuItem("Assets/Create/Rendering/Material (Complex Specular)", false, 200)]
        public static void CreateComplexSpecularMaterial()
        {
            // Create the Material with the Complex Lit Shader
            Shader complexLit = Shader.Find("Universal Render Pipeline/Complex Lit");
            if (complexLit == null)
            {
                Debug.LogError("Complex Lit shader not found. Ensure URP is installed.");
                return;
            }
    
            Material mat = new Material(complexLit);
    
            // Set it to Specular Workflow by default
            mat.SetInt(WorkflowMode, 0);
            
            // Enable the Specular Highlights and Reflections
            mat.EnableKeyword("_SPECULAR_SETUP");
            mat.DisableKeyword("_METALLIC_SETUP");

            // Save the asset in the current folder
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") path = "Assets";
            else if (Path.GetExtension(path) != "") path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Complex Specular.mat");
    
            AssetDatabase.CreateAsset(mat, assetPathAndName);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = mat;
        }
    }
}
