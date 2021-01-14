using System;
using System.Threading.Tasks;
using Game.Core.Common;
using Game.Gameplay;
using Game.MainUI;
using Infra.Controllers.Core;
using Infra.Controllers.Events;
using UnityEngine;

namespace Game.Core
{
    public sealed class GameRootController : RootController
    {
        private readonly GameOverSignal _gameOverSignal;
        private readonly GameStartMenuSignal _gameStartMenuSignal;
        private readonly GameStartSignal _gameStartSignal;
        private readonly TickManager _tickManager;
        
        public GameRootController(
            IControllerFactory controllerFactory,
            GameOverSignal gameOverSignal,
            TickManager tickManager
            )
            : base(controllerFactory)
        {
            GameContextControllerFactoryProvider.Setup(controllerFactory);
            _tickManager = tickManager;
            _gameOverSignal = gameOverSignal;
        }

        protected override void OnInitialize()
        {
        }

        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();
            _gameOverSignal.AddListener(GameOverHandler);
            _gameStartMenuSignal.AddListener(StartMenuHandler);
            _gameStartSignal.AddListener(StartGameHandler);
            await CreateAndStartFeatures();
        }

        protected override Task OnStopAsync()
        {
            _gameOverSignal.RemoveListener(GameOverHandler);
            _gameStartMenuSignal.RemoveListener(StartMenuHandler);
            return base.OnStopAsync();
        }

        protected override void ThrowUnhandledEventAssert(IEvent e)
        {
            throw new NotImplementedException();
        }

        private async Task CreateAndStartFeatures()
        {
            var tasks = new[]
            {
                CreateAndStartAsync<GameplayController>(CancellationToken),
                CreateAndStartAsync<MainUIController>(CancellationToken)
            };
            await Task.WhenAll(tasks);
        }
        
        private void StartMenuHandler()
        {
            Debug.Log("StartMenu");
            _tickManager.IsFreezed = true;
        }
        
        private void StartGameHandler()
        {
            Debug.Log("Start");
            _tickManager.IsFreezed = false;
        }
        
        private void GameOverHandler()
        {
            Debug.Log("GameOver");
            _tickManager.IsFreezed = true;
        }
    }
}