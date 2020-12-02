using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CreateAssetBundles 
    {

        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            var path = Application.streamingAssetsPath;
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            BuildPipeline.BuildAssetBundles(path, 
                BuildAssetBundleOptions.None, 
                BuildTarget.StandaloneWindows);
            
            AssetDatabase.Refresh();
        }
    }
}
