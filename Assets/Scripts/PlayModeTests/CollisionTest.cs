using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.Core.Managers;
using Game.Gameplay.Block;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class CollisionTest
    {
        private BlockMover mover;
        private TickManager tickManager;
        private GridManager gridManager;
        
        [SetUp]
        public void Initial()
        {
            SceneManager.LoadScene(0);
        }

        [UnityTest]
        public IEnumerator CollisionWithGroundPasses()
        {
            yield return WaitForMover();
            var blockInitializer = mover.gameObject.GetComponent<BlockInitializer>();
            var shape = blockInitializer.CurrentShape;
            var lastY = gridManager.Grid.Dimensions.y - 1;
            var moveCount = lastY - shape.Size.y;
            for (var i = 0; i < moveCount; i++)
            {
                yield return new WaitForSeconds(0.1f);
                tickManager.DispatchVerticalMove(1);
            }
            Assert.AreEqual(lastY - shape.Size.y, mover._gridCoordinate.y);
        }
        
        [UnityTest]
        public IEnumerator CollisionWithSidePasses()
        {
            yield return WaitForMover();
            for (var i = 0; i < gridManager.Grid.Dimensions.x / 2; i++)
            {
                yield return new WaitForSeconds(0.1f);
                tickManager.DispatchHorizontalMove(1);
            }
            Assert.AreEqual(gridManager.Grid.Dimensions.x - 1, mover._gridCoordinate.x);
        }

        private IEnumerator WaitForMover()
        {
            mover = default;
            while (mover == null)
            {
                mover = Object.FindObjectOfType<BlockMover>();
                yield return null;
            }
            
            tickManager = Object.FindObjectOfType<TickManager>();
            tickManager.IsFreezed = true;

            gridManager = Object.FindObjectOfType<GridManager>();
        }
    }
}
