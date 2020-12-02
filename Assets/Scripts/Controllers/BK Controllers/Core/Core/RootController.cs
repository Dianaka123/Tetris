using System.Diagnostics;
using System.Threading.Tasks;
using Infra.Controllers.Events;

namespace Infra.Controllers.Core
{
    public abstract class RootController : ControllerBase
    {
        protected RootController(IControllerFactory controllerFactory) : base(controllerFactory)
        {
        }

#if UNITY_EDITOR
        public static ControllerBase Instance { get; private set; }
#endif

        protected override Task OnStartAsync()
        {
            SetRootController(this);
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            SetRootController(null);
            return Task.CompletedTask;
        }

        protected sealed override bool HandleEvent(IEvent e)
        {
            ThrowUnhandledEventAssert(e);
            return true;
        }

        protected abstract void ThrowUnhandledEventAssert(IEvent e);

        [Conditional("UNITY_EDITOR")]
        private static void SetRootController(ControllerBase controller)
        {
#if UNITY_EDITOR
            Instance = controller;
#endif
        }
    }
}