using System;
using System.Threading.Tasks;
using Game.Gameplay;
using Game.MainUI;
using Infra.Controllers.Core;
using Infra.Controllers.Events;
using UnityAcademy.TreeOfControllersExample;
using UnityEngine;

namespace Game
{
    public sealed class GameRootController : RootController
    {
        public GameRootController(
            IControllerFactory controllerFactory)
            : base(controllerFactory)
        {
            GameContextControllerFactoryProvider.Setup(controllerFactory);
        }

        protected override void OnInitialize()
        {
        }

        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();
            await CreateAndStartFeatures();
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
    }
}