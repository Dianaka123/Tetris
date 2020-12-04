using Game.Core;
using Game.Core.Common;
using Game.Core.Interfaces;
using Game.Gameplay.Block;
using Game.Gameplay.Interfaces;
using strange.extensions.mediation.impl;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Models
{
    public class SpawnManager : View, ISpawnManager
    {
        [Inject]
        public IGridManager GridManager { get; set; }

        [Inject]
        public ISoundManager SoundManager { get; set; }

        [Inject]
        public IShapeRandomizer ShapeRandomizer { get; set; }

        [Inject] 
        public ShapeVerticalMoveSignal ShapeVerticalMoveSignal { get; set; }
        
        [Inject]
        public ShapeHorizontalMoveSignal ShapeHorizontalMoveSignal { get; set; }
        
        [Inject]
        public GameOverSignal GameOverSignal { get; set; }

        [Inject] 
        public ShapeRotateSignal ShapeRotateSignal { get; set; }

        [Inject] 
        public LineFullSignal LineFullSignal { get; set; }

        private GameObject nextShape;
        private (ShapeContainer shape, GameObject block) randomShape;

        public void Spawn()
        {
            CreateMovingObject(randomShape.shape, randomShape.block);
            PredictNextSpawn();
            CreateNextShape();
        }

        public void Initialize() => PredictNextSpawn();

        private void PredictNextSpawn()
        {
            randomShape = ShapeRandomizer.RandomizeBlock();
        }

        private void CreateNextShape()
        {
            if (nextShape != null)
            {
                Destroy(nextShape);
            }
            nextShape = CreatePreviewObject(randomShape.shape, randomShape.block,
                new Vector3(-25, 0, 0));
        }

        private GameObject CreatePreviewObject(ShapeContainer shape, GameObject color, Vector3? translate = null)
        {
            var container = new GameObject("Container");
            container.transform.parent = transform;
            container.transform.position += translate.GetValueOrDefault();

            var behaviour = container.AddComponent<BlockInitializer>();
            behaviour.GridManager = GridManager;
            behaviour.ShapeContainer = shape;
            behaviour.Block = color;
            return container;
        }

        private void CreateMovingObject(ShapeContainer shape, GameObject color)
        {
            var container = CreatePreviewObject(shape, color);
            var mover = container.AddComponent<BlockMover>();
            mover.SpawnManager = this;
            mover.ShapeVerticalMoveSignal = ShapeVerticalMoveSignal;
            mover.ShapeHorizontalMoveSignal = ShapeHorizontalMoveSignal;
            mover.LineFullSignal = LineFullSignal;
            mover.ShapeRotateSignal = ShapeRotateSignal;
            mover.SoundManager = SoundManager;
            mover.GameOverSignal = GameOverSignal;
        }
    }
}