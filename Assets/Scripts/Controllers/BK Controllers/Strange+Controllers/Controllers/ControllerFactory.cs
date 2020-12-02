using System;
using Infra.Controllers.Core;
using strange.extensions.injector.api;

namespace BoardKings.Core.Controllers
{
    public class ControllerFactory : IControllerFactory
    {
        private readonly IInjectionBinder _injectionBinder;

        public ControllerFactory(IInjectionBinder injectionBinder)
        {
            _injectionBinder = injectionBinder;
        }

        public T Create<T>() where T : ControllerBase
        {
            return Create<T>(typeof(T));
        }

        public T Create<T>(Type type) where T : ControllerBase
        {
            var controller = (T) _injectionBinder.GetInstance(type);
            if (controller == null) throw new ControllerCreationException(type);

            return controller;
        }
    }
}