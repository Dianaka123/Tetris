using System;
using System.Threading;
using Infra.Controllers.Core;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace UnityAcademy.TreeOfControllersExample
{
    public class GameContextView : ContextView, IGameRoot, IRootForGameObjects
    {
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private GridManager gridManager;
        
        private GameRootController _gameRootController;
        private CancellationTokenRegistration _tokenRegistration;
        private CancellationTokenSource _tokenSource;
        
        private void Awake()
        {
            try
            {
                context = new GameContext(this);
                context.Start();
            }
            catch (Exception exception)
            {
                Debug.LogError("Start failed" + exception.ToString());
            }
        }

        private async void Start()
        {
            _tokenSource = new CancellationTokenSource();
            try
            {
                _gameRootController = (context as GameContext)?.CreateController<GameRootController>();
                _gameRootController?.Initialize(null, _tokenSource.Token);

                _tokenRegistration = _tokenSource.Token.Register(StopRootController, _gameRootController, true);

                await _gameRootController.StartAsync();
            }
            catch (Exception e)
            {
                
            }
        }

        protected override void OnDestroy()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            _tokenRegistration.Dispose();

            base.OnDestroy();
        }

        private async void StopRootController(object controller)
        {
            try
            {
                if (controller is ControllerBase rootController)
                    try
                    {
                        await rootController.StopAsync();
                    }
                    finally
                    {
                        rootController.Dispose();
                    }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to stop Root controller.");
            }
        }

        public IRootForGameObjects RootForGameObjects => this;
        public ISpawnManager Spawner => spawnManager;
        public IGridManager GridManager => gridManager;
        public Transform ContainerTransform => transform;
    }
}