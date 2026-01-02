using UnityEngine;
using UnityEditor;
using System.IO;

namespace LibYiroth.Helper.Editor
{
    public class UniversalPbrBaker : EditorWindow
    {
        private enum TexturePackType { Orm, Rma }

        private Texture2D _albedoMap;
        private Texture2D _maskMap;
        private TexturePackType _packType = TexturePackType.Orm;

        private float _smoothnessIntensity = 1.0f;
        private float _aoIntensity = 1.0f;

        [MenuItem("Tools/Texture Baker/Universal PBR Baker")]
        public static void ShowWindow() => GetWindow<UniversalPbrBaker>("PBR Baker");

        private void OnGUI()
        {
            _albedoMap = (Texture2D)EditorGUILayout.ObjectField("Albedo (Base Color)", _albedoMap, typeof(Texture2D), false);
            _maskMap = (Texture2D)EditorGUILayout.ObjectField("Mask Map (ORM/RMA)", _maskMap, typeof(Texture2D), false);

            _packType = (TexturePackType)EditorGUILayout.EnumPopup("Mask Type", _packType);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Adjustments", EditorStyles.boldLabel);
            _smoothnessIntensity = EditorGUILayout.Slider("Smoothness Ceiling", _smoothnessIntensity, 0, 1);
            _aoIntensity = EditorGUILayout.Slider("AO Strength", _aoIntensity, 0, 1);

            GUI.enabled = (Cardinal.IsValid(_albedoMap) && Cardinal.IsValid(_maskMap));
            if (GUILayout.Button("Bake Production Textures")) Bake();
            GUI.enabled = true;
        }

        private void Bake()
        {
            int w = _albedoMap.width; 
            int h = _albedoMap.height;

            Texture2D specMap = new Texture2D(w, h, TextureFormat.RGBA32, true);
            Texture2D aoMap = new Texture2D(w, h, TextureFormat.RGB24, true); 
            Texture2D cleanAlb = new Texture2D(w, h, TextureFormat.RGB24, true);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color alb = _albedoMap.GetPixel(x, y);
                    Color mask = _maskMap.GetPixel(x, y);

                    float ao, rough, metal;

                    // ROUTING CHANNELS BASED ON DROPDOWN
                    if (_packType == TexturePackType.Orm)
                    {
                        ao = mask.r;
                        rough = mask.g;
                        metal = mask.b;
                    }
                    else // RMA Mode
                    {
                        rough = mask.r;
                        metal = mask.g;
                        ao = mask.b;
                    }

                    // SPECULAR (RGB = Tint, A = Smoothness)
                    Color specColor = Color.Lerp(new Color(0.04f, 0.04f, 0.04f), alb, metal);
                    float smooth = (1.0f - rough) * _smoothnessIntensity;
                    specMap.SetPixel(x, y, new Color(specColor.r, specColor.g, specColor.b, smooth));

                    // AO (Standard RGB compatibility)
                    float finalAO = Mathf.Lerp(1.0f, ao, _aoIntensity);
                    aoMap.SetPixel(x, y, new Color(finalAO, finalAO, finalAO, 1));

                    // CLEAN ALBEDO (Black for Metal)
                    Color finalAlb = Color.Lerp(alb, Color.black, metal);
                    cleanAlb.SetPixel(x, y, new Color(finalAlb.r, finalAlb.g, finalAlb.b, alb.a));
                }
            }

            Save(specMap, "_Spec"); 
            Save(aoMap, "_AO"); 
            Save(cleanAlb, "_Albedo");

            AssetDatabase.Refresh();
            Debug.Log($"Successfully baked {_packType} set for {_albedoMap.name}");
        }

        private void Save(Texture2D tex, string suffix) 
        {
            string path = AssetDatabase.GetAssetPath(_albedoMap);
            string newPath = Path.GetDirectoryName(path) + "/" + _albedoMap.name + suffix + ".png";
            File.WriteAllBytes(newPath, tex.EncodeToPNG());
        }
    }
}