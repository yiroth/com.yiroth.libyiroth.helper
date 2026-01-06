using UnityEngine;
using UnityEditor;
using System.IO;

namespace LibYiroth.Helper.Editor
{
    public class UniversalPbrBaker : EditorWindow
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
        private static readonly int OcclusionMap = Shader.PropertyToID("_OcclusionMap");
        private static readonly int Surface = Shader.PropertyToID("_Surface");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        public enum TexturePackType { Orm, Rma }

        private Texture2D _albedoMap, _maskMap, _opacityMap;
        private TexturePackType _packType = TexturePackType.Orm;
        private float _smoothnessIntensity = 1.0f, _aoIntensity = 1.0f;
    
        [MenuItem("Tools/Texture Baker/Universal PBR Baker")]
        public static void ShowWindow() => GetWindow<UniversalPbrBaker>("PBR Baker");

        private void OnGUI()
        {
            _albedoMap = (Texture2D)EditorGUILayout.ObjectField("Albedo", _albedoMap, typeof(Texture2D), false);
            _opacityMap = (Texture2D)EditorGUILayout.ObjectField("Opacity Map (Alpha)", _opacityMap, typeof(Texture2D), false);
            _maskMap = (Texture2D)EditorGUILayout.ObjectField("Mask (ORM/RMA)", _maskMap, typeof(Texture2D), false);
            
            _packType = (TexturePackType)EditorGUILayout.EnumPopup("Input Mode", selected: _packType);
            _smoothnessIntensity = EditorGUILayout.Slider("Smoothness", _smoothnessIntensity, 0, 1);
            _aoIntensity = EditorGUILayout.Slider("AO Strength", _aoIntensity, 0, 1);
    
            if (GUILayout.Button("Bake & Auto-Assign") && _albedoMap && _maskMap) Bake();
        }

        private void Bake()
        {
            int w = _albedoMap.width; int h = _albedoMap.height;
            Texture2D specMap = new Texture2D(w, h, TextureFormat.RGBA32, true);
            Texture2D aoMap = new Texture2D(w, h, TextureFormat.RGB24, true); 
            // Important: Albedo must be RGBA32 to hold the Alpha channel
            Texture2D cleanAlb = new Texture2D(w, h, TextureFormat.RGBA32, true);
    
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color alb = _albedoMap.GetPixel(x, y);
                    Color m = _maskMap.GetPixel(x, y);
                    
                    // CHANNEL ROUTING
                    float ao = (_packType == TexturePackType.Orm) ? m.r : m.b;
                    float rough = (_packType == TexturePackType.Orm) ? m.g : m.r;
                    float metal = (_packType == TexturePackType.Orm) ? m.b : m.g;
    
                    // SPECULAR
                    Color spec = Color.Lerp(new Color(0.04f, 0.04f, 0.04f), alb, metal);
                    specMap.SetPixel(x, y, new Color(spec.r, spec.g, spec.b, (1.0f - rough) * _smoothnessIntensity));
                    
                    // AO
                    float fAO = Mathf.Lerp(1.0f, ao, _aoIntensity);
                    aoMap.SetPixel(x, y, new Color(fAO, fAO, fAO, 1));
    
                    // ALBEDO + OPACITY COMBINATION
                    Color finalColor = Color.Lerp(alb, Color.black, metal);
                    float alphaValue = 1.0f;
                    if (_opacityMap) {
                        alphaValue = _opacityMap.GetPixel(x, y).r; // Take Red from Opacity map
                    }
                    cleanAlb.SetPixel(x, y, new Color(finalColor.r, finalColor.g, finalColor.b, alphaValue));
                }
            }
    
            // Saving logic (same as before)
            string dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_albedoMap));
            string bName = _albedoMap.name;
            string sP = $"{dir}/{bName}_Spec.png"; string aP = $"{dir}/{bName}_AO.png"; string cP = $"{dir}/{bName}_Albedo.png";
    
            File.WriteAllBytes(sP, specMap.EncodeToPNG());
            File.WriteAllBytes(aP, aoMap.EncodeToPNG());
            File.WriteAllBytes(cP, cleanAlb.EncodeToPNG());
            AssetDatabase.Refresh();
    
            // Auto-Assign to Material
            if (Selection.activeObject is Material mat)
            {
                mat.SetTexture(BaseMap, AssetDatabase.LoadAssetAtPath<Texture2D>(cP));
                mat.SetTexture(SpecGlossMap, AssetDatabase.LoadAssetAtPath<Texture2D>(sP));
                mat.SetTexture(OcclusionMap, AssetDatabase.LoadAssetAtPath<Texture2D>(aP));
                
                // If we have an opacity map, tell the material to use Alpha Clipping or Transparency
                if (_opacityMap) {
                    mat.SetFloat(Surface, 1); // 1 = Transparent
                    mat.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt(ZWrite, 0);
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
    
                EditorUtility.SetDirty(mat);
            }
        }
    }
}