using System.Threading.Tasks;
using Infra.Controllers.Core;

namespace Game.MainUI
{
    public class MainUIController: ControllerWithResultBase
    {
        public MainUIController(
            IControllerFactory controllerFactory) : base(controllerFactory)
        {
        }

        protected override Task OnStartAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }
    }
}