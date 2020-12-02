using System;
using System.Collections.Specialized;
using DefaultNamespace;
using JetBrains.Annotations;
using ScriptableObject;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace UnityAcademy.TreeOfControllersExample
{
    public struct Shape
    {
        public readonly Vector2Int Size;

        /// <summary>
        /// The mask is written in reverse order.
        /// <example>
        /// <para>Litter L we see like - 0b00101011</para>
        /// <para>But we should write - 0b00110101</para>
        /// </example> 
        /// </summary>
        public int Mask { get; private set; }

        public Shape(Vector2Int size, int? predefinedMask = null)
        {
            Mask = predefinedMask ?? 0;
            Size = size;
        }

        public Shape SetBlock(int x, int y, bool exists)
        {
            Mask |= (1 << GetRawIndex(x, y));
            return this;
        }

        public bool GetBlock(int x, int y) => (Mask & (1 << GetRawIndex(x, y))) != 0;

        private int GetRawIndex(int x, int y) => y * Size.x + x;
    }
    
    
    public class SpawnManager : MonoBehaviour, ISpawnManager
    {
        [SerializeField] private int countOfColoredBlocks = 7;
        [SerializeField] private Block block;
        public IGridManager GridManager { get; set; }


        private readonly string nameOfBlock = "Block";
        private readonly string colorTag = "color";
        private readonly string shadowTag = "shadow";

        private readonly Shape[] _shapes = new []
        {
            new Shape( new Vector2Int(2, 3), 0b00110101),//L
            new Shape( new Vector2Int(1, 4), 0b00001111),//I
            new Shape( new Vector2Int(2, 3),  0b00111010),//J
            new Shape( new Vector2Int(2, 2), 0b00001111),//O
            new Shape( new Vector2Int(3, 2), 0b00010111),//T
            new Shape( new Vector2Int(3, 2), 0b00110011),//Z
            new Shape( new Vector2Int(3, 2), 0b00011110),//S
        };

        private AssetBundle assetBundle;
        private int nextShapeIndex;
        private int nextColorIndex;


        private void Start()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            assetBundle = BundleLoader.Load(block.BundleName, colorTag);
            PredictSpawnShape();
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            var blockAsset = assetBundle.LoadAsset<GameObject>($"{nameOfBlock}{nextColorIndex}");
            CreateBlocks(_shapes[nextShapeIndex], blockAsset);
            
            PredictSpawnShape();
        }

        private void CreateBlocks(Shape shape, GameObject color)
        {
            var container = new GameObject("Container");
            container.transform.parent = transform;

            var behaviour = container.AddComponent(typeof(BlockBehaviour)) as BlockBehaviour;
            behaviour.GridManager = GridManager;
            behaviour.Shape = shape;
            behaviour.Block = color;
            behaviour.SpawnManager = this;
        }

        private void PredictSpawnShape()
        {
           nextShapeIndex = Random.Range(0, _shapes.Length - 1);
           nextColorIndex = Random.Range(1, countOfColoredBlocks);
        }
        
        private void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }
}