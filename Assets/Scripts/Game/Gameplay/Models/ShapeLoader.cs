using Game.Gameplay.Interfaces;
using ScriptableObject;
using strange.extensions.mediation.impl;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Models
{
    public class ShapeLoader : View, IShapeLoader
    {
        private static readonly string nameOfBlock = "Block";
        private static readonly string colorTag = "color";

        [SerializeField] private Block block;
        
        private AssetBundle assetBundleColored;

        public int Count => 7;

        protected override void Start()
        {
            base.Start();
            assetBundleColored = BundleHelper.Load(block.BundleName, colorTag);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            BundleHelper.Unload(assetBundleColored);
        }
        
        public GameObject LoadBlock(int index) => 
            assetBundleColored.LoadAsset<GameObject>($"{nameOfBlock}{index + 1}");
    }
}