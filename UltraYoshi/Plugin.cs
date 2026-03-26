using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

namespace UltraYoshiSkulls
{
    public enum Yoshi
    {
        RedSkull,
        BlueSkull,
        Soap,
        Torch,
        Book,
        Rocket
    }

    public static class YoshiExtensions
    {
        public static string PrefabName(this Yoshi yoshi) => yoshi switch
        {
            Yoshi.RedSkull => "MyRedSkull",
            Yoshi.BlueSkull => "MyBlueSkull",
            Yoshi.Soap => "MySoap",
            Yoshi.Torch => "MyTorch",
            Yoshi.Book => "MyBook",
            Yoshi.Rocket => "MyRocket",
            _ => ""
        };
    }

    [BepInPlugin("io.selim.ultrayoshis", "Ultra Yoshis", "2.0.0")]
    public class UltraYoshiSkullsPlugin : BaseUnityPlugin
    {
        public static readonly Dictionary<Yoshi, GameObject> allYoshis = new Dictionary<Yoshi, GameObject>();

        // This links directly to your new YoshiConfig.cs file!
        public static readonly YoshiConfig Config = new YoshiConfig();

        public void Awake()
        {
            Config.Awake();
            
            string pluginFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string bundlePath = Path.Combine(pluginFolder, "custompropsbundle");

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Logger.LogError("FAILED TO LOAD ASSETBUNDLE!");
                return;
            }

            foreach (Yoshi yoshi in Enum.GetValues(typeof(Yoshi)))
            {
                GameObject loadedPrefab = bundle.LoadAsset<GameObject>(yoshi.PrefabName());
                if (loadedPrefab != null)
                {
                    allYoshis.Add(yoshi, loadedPrefab);
                }
                else
                {
                    Logger.LogWarning($"Could not find prefab '{yoshi.PrefabName()}' in the bundle.");
                }
            }

            Harmony harmony = new Harmony("io.selim.ultrayoshis");
            harmony.PatchAll();

            Logger.LogInfo("Ultra Yoshi Skulls Loaded!");
        }

        public static void CreateYoshi(Yoshi yoshiType, Transform masterTransform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!allYoshis.ContainsKey(yoshiType)) return;

            GameObject yoshiPrefab = allYoshis[yoshiType];
            GameObject spawnedYoshi = Instantiate(yoshiPrefab, masterTransform);
            spawnedYoshi.SetActive(true);
            
            spawnedYoshi.transform.localRotation = rotation;
            spawnedYoshi.transform.localPosition = position;
            spawnedYoshi.transform.localScale = scale;
        }
    }

    [HarmonyPatch(typeof(Skull), "Awake")]
    static class SkullPatch
    {
        static void Postfix(Skull __instance)
        {
            var itemIdentifier = __instance.GetComponent<ItemIdentifier>();
            if (itemIdentifier == null) return;
            var skullType = itemIdentifier.itemType;

            Yoshi typeToSpawn;
            if (skullType == ItemType.SkullRed) 
            {
                if (UltraYoshiSkullsPlugin.Config.IsRedSkullDisabled) return; 
                typeToSpawn = Yoshi.RedSkull;
            }
            else if (skullType == ItemType.SkullBlue) 
            {
                if (UltraYoshiSkullsPlugin.Config.IsBlueSkullDisabled) return; 
                typeToSpawn = Yoshi.BlueSkull;
            }
            else return;

            ModifyMaterial modifyMaterial;
            try { modifyMaterial = Traverse.Create(__instance).Field<ModifyMaterial>("mod").Value; }
            catch { return; }

            Renderer renderer;
            try
            {
                var traverse = Traverse.Create(modifyMaterial);
                traverse.Method("SetValues").GetValue();
                renderer = traverse.Field<Renderer>("rend").Value;
            }
            catch { return; }

            if (renderer)
            {
                renderer.enabled = false;

                UltraYoshiSkullsPlugin.CreateYoshi(
                    typeToSpawn,
                    renderer.transform,
                    new Vector3(0f, 0f, -0.06f),                 
                    Quaternion.Euler(110f, 0f, 0f),          
                    new Vector3(0.01f, 0.01f, 0.01f)         
                );
            }
        }
    }

    [HarmonyPatch(typeof(Soap), "Start")] 
    static class SoapPatch
    {
        static void Postfix(Soap __instance)
        {
            if (UltraYoshiSkullsPlugin.Config.IsSoapDisabled) return; 

            Renderer renderer = __instance.GetComponentInChildren<Renderer>();
            
            if (renderer)
            {
                renderer.enabled = false;

                UltraYoshiSkullsPlugin.CreateYoshi(
                    Yoshi.Soap,
                    __instance.transform, 
                    new Vector3(0f, -0.4f, 0f), 
                    Quaternion.Euler(0f, 180f, 0f),          
                    new Vector3(0.05f, 0.05f, 0.05f) 
                );
            }
        }
    }

    [HarmonyPatch(typeof(Readable), "Awake")]
    static class BookPatch
    {
        static void Postfix(Readable __instance)
        {
            if (UltraYoshiSkullsPlugin.Config.IsBookDisabled) return; 

            Renderer renderer = __instance.GetComponentInChildren<Renderer>();
            
            if (renderer)
            {
                renderer.enabled = false;

                UltraYoshiSkullsPlugin.CreateYoshi(
                    Yoshi.Book,
                    __instance.transform,
                    new Vector3(0f, 0f, 0f),                 
                    Quaternion.Euler(-90f, 180f, 0f),          
                    new Vector3(0.05f, 0.05f, 0.05f)         
                );
            }
        }
    }

    [HarmonyPatch(typeof(Torch), "Start")]
    static class TorchPatch
    {
        static void Postfix(Torch __instance)
        {
            if (UltraYoshiSkullsPlugin.Config.IsTorchDisabled) return; 

            Renderer renderer = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
            
            if (renderer)
            {
                renderer.enabled = false;

                UltraYoshiSkullsPlugin.CreateYoshi(
                    Yoshi.Torch,
                    __instance.transform,
                    new Vector3(0f, -0.5f, 0f),
                    Quaternion.Euler(0f, 90f, -0.6f),
                    new Vector3(0.05f, 0.05f, 0.05f)
                );
            }
        }
    }

    [HarmonyPatch(typeof(Grenade), "Awake")]
    static class GrenadePatch
    {
        static void Postfix(Grenade __instance)
        {
            if (UltraYoshiSkullsPlugin.Config.IsRocketDisabled) return; 

            if (__instance.rocket)
            {
                Renderer[] meshRenderers = __instance.gameObject.GetComponentsInChildren<MeshRenderer>();
                
                if (meshRenderers.Length > 0)
                {
                    foreach (Renderer renderer in meshRenderers)
                    {
                        renderer.enabled = false;
                    }

                    UltraYoshiSkullsPlugin.CreateYoshi(
                        Yoshi.Rocket,
                        __instance.transform, 
                        new Vector3(0f, 0f, 0f),                 
                        Quaternion.Euler(90f, 90f, 90f),
                        new Vector3(0.25f, 0.25f, 0.25f)         
                    );
                }
            }
        }
    }
}