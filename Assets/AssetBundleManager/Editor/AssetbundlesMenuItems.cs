using UnityEngine;
using UnityEditor;

namespace AssetBundles
{
    public class AssetBundlesMenuItems
    {
        const string k_SimulationMode = "Assets/AssetBundles/Simulation Mode";
        const string k_BuildAssetBundles = "Assets/AssetBundles/Build AssetBundles";

        [MenuItem("Assets/AssetBundles/Delete Asset Bundle")]
        static void DeleteAsset()
        {

            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (string bundle in names)
            {
                string assetName = bundle.StartsWith("themes/") ? "themeData" : "character";

                //Debug.Log(Application.dataPath + "/" + bundle);
                //Debug.Log(System.IO.File.Exists(Application.dataPath + "/" + bundle));

                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundle, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogError("There is no asset with name \"" + assetName + "\" in " + bundle);
                    Debug.Log(AssetDatabase.DeleteAsset(bundle));
                }
            }
        }

        [MenuItem("Assets/AssetBundles/Get Asset Bundle names")]
        static void GetNames()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (string name in names)
                Debug.Log("Asset Bundle: " + name);
        }

        [MenuItem(k_SimulationMode)]
        public static void ToggleSimulationMode()
        {
            AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
        }

        [MenuItem(k_SimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(k_SimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
            return true;
        }

        [MenuItem(k_BuildAssetBundles)]
        static public void BuildAssetBundles()
        {
            BuildScript.BuildAssetBundles();
        }
    }
}