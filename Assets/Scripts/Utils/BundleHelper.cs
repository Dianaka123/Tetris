using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class BundleHelper 
    {
        public static AssetBundle Load(string bundleName, string tag)
        {
           return AssetBundle.LoadFromFile(Path.Combine( Application.streamingAssetsPath,$"{bundleName}.{tag}"));
        }
        
        public static void Unload(AssetBundle assetBundle)
        {
            assetBundle.Unload(true);
        }
        
        public static AssetBundle LoadWithoutTag(string bundleName,string tag)
        {
            return AssetBundle.LoadFromFile(Path.Combine( Application.streamingAssetsPath,$"{bundleName}"));
        }
        
    }
}