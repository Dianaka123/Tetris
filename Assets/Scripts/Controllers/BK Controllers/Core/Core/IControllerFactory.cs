using System;

namespace Infra.Controllers.Core
{
    public interface IControllerFactory
    {
        T Create<T>() where T : ControllerBase;
        T Create<T>(Type type) where T : ControllerBase;
    }
}