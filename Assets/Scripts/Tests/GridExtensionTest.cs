using Game.Core.Interfaces;
using Game.Gameplay.Interfaces;
using Game.Gameplay.Models;
using NSubstitute;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using UnityEngine;
using UnityEngine.Analytics;
using Utils;

namespace Tests
{
    public class CridExtensionTest
    {
        private IInjectionBinder _injectionBinder;

        [SetUp]
        public void SetUp()
        {
            var gridManager = Substitute.For<IGridManager>();
            var grid = Substitute.For<IGrid>();
            grid.Dimensions.Returns(new Vector2Int(10, 20));
            grid.Interval.Returns(0.05f);
            grid.BlockSize.Returns(2.8f);
            
            gridManager.Grid.Returns(grid);
            _injectionBinder = new InjectionBinder();
            _injectionBinder.Bind<IGridManager>().ToValue(gridManager);
            _injectionBinder.Bind<IShapeManager>().To<ShapeManager>();
        }
        
        [Test]
        [TestCase(5, 2, 1, ExpectedResult = false)]
        [TestCase(0, 2, 1, ExpectedResult = false)]
        [TestCase(0, 2, 0, ExpectedResult = false)]
        [TestCase(5, 2, 5, ExpectedResult = true)]
        [TestCase(0, 2, -3, ExpectedResult = true)]
        [TestCase(5, 2, 4, ExpectedResult = true)]

        public bool CheckSide_Correct(int xCoordinate, int sizeX, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            
            var result = gridManagerResolve.Grid.CheckSide(xCoordinate, sizeX, step);
            return result;
        }
        
        [Test]
        [TestCase(5, 2, 1, ExpectedResult = false)]
        [TestCase(0, 2, 1, ExpectedResult = false)]
        [TestCase(0, 2, 0, ExpectedResult = false)]
        [TestCase(18, 2, 0, ExpectedResult = false)]
        [TestCase(18, 2, 1, ExpectedResult = true)]
        [TestCase(18, 3, 0, ExpectedResult = true)]
        [TestCase(0, 2, -3, ExpectedResult = true)]

        public bool CheckGround_Correct(int yCoordinate, int sizeY, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            
            var result = gridManagerResolve.Grid.CheckGround(yCoordinate, sizeY, step);
            return result;
        }
        
        [Test]
        [TestCase(1, 0, 0, 1, ExpectedResult = false)]
        [TestCase(1, 0, 1, 1, ExpectedResult = false)]
        [TestCase(1, 0, 2, 1, ExpectedResult = false)]
        [TestCase(1, 0, 3, 1, ExpectedResult = false)]
        [TestCase(1, 0, 4, 1, ExpectedResult = false)]
        [TestCase(1, 0, 5, 1,ExpectedResult = false)]
        [TestCase(1, 0, 6, 1,ExpectedResult = false)]
        
        [TestCase(5, 6, 0, 2, ExpectedResult = true)]
        [TestCase(5, 5, 1, 2, ExpectedResult = true)]
        [TestCase(5, 6, 2, 2, ExpectedResult = true)]
        [TestCase(5, 7, 3, 2, ExpectedResult = true)]
        [TestCase(5, 7, 4, 2, ExpectedResult = true)]
        [TestCase(5, 7, 5, 2,ExpectedResult = true)]
        [TestCase(5, 7, 6, 2,ExpectedResult = true)]

        public bool CheckVerticalCollision_Correct(int x, int y, int shapeIndex, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            gridManagerResolve.SetBlock(new Vector2Int(5, 9),new GameObject("kaka") );
            var shapeManager = _injectionBinder.GetInstance<IShapeManager>();
            
            var shapeContainer = shapeManager.Shapes[shapeIndex];
            var result = gridManagerResolve.CheckVerticalCollision(new Vector2Int(x, y), shapeContainer.Shapes[0], step);
            return result;
        }

        [Test]
        [TestCase(1, 0, 0, 1, ExpectedResult = false)]
        [TestCase(1, 0, 1, 1, ExpectedResult = false)]
        [TestCase(1, 0, 2, 1, ExpectedResult = false)]
        [TestCase(1, 0, 3, 1, ExpectedResult = false)]
        [TestCase(1, 0, 4, 1, ExpectedResult = false)]
        [TestCase(1, 0, 5, 1,ExpectedResult = false)]
        [TestCase(1, 0, 6, 1,ExpectedResult = false)]
        
        [TestCase(4, 9, 0, 2, ExpectedResult = true)]
        [TestCase(4, 9, 1, 2, ExpectedResult = true)]
        [TestCase(4, 9, 2, 2, ExpectedResult = true)]
        [TestCase(4, 9, 3, 2, ExpectedResult = true)]
        [TestCase(4, 9, 4, 2, ExpectedResult = true)]
        [TestCase(4, 9, 5, 2,ExpectedResult = true)]
        [TestCase(4, 9, 6, 2,ExpectedResult = true)]

        public bool CheckHorizontalCollision_Correct(int x, int y, int shapeIndex, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            gridManagerResolve.SetBlock(new Vector2Int(5, 9),new GameObject("kaka") );
            var shapeManager = _injectionBinder.GetInstance<IShapeManager>();
            
            var shapeContainer = shapeManager.Shapes[shapeIndex];
            var result = gridManagerResolve.CheckVerticalCollision(new Vector2Int(x, y), shapeContainer.Shapes[0], step);
            return result;
        }
        
        [Test]
        [TestCase(-5, 2, 1, ExpectedResult = true)]
        [TestCase(0, -2, 1, ExpectedResult = true)]

        public bool CheckSide_UnCorrect(int xCoordinate, int sizeX, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            
            var result = gridManagerResolve.Grid.CheckSide(xCoordinate, sizeX, step);
            return result;
        }
        
        [Test]
        [TestCase(-5, 2, 1, ExpectedResult = true)]
        [TestCase(0, -2, 1, ExpectedResult = true)]

        public bool CheckGround_UnCorrect(int yCoordinate, int sizeY, int step)
        {
            var gridManagerResolve = _injectionBinder.GetInstance<IGridManager>();
            
            var result = gridManagerResolve.Grid.CheckSide(yCoordinate, sizeY, step);
            return result;
        }
    }
}
