using System;
using System.Threading.Tasks;
using Controllers;
using Infra.Controllers.Core;
using Infra.Controllers.Events;

namespace UnityAcademy.TreeOfControllersExample
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

        protected override void OnDispose()
        {
        }

        private async Task CreateAndStartFeatures()
        {
            var tasks = new[]
            {
                CreateAndStartAsync<GameController>(CancellationToken),
            };

            await Task.WhenAll(tasks);
        }
    }
}