using System;

namespace Infra.Controllers.Core
{
    [Serializable]
    public class ControllerCreationException : Exception
    {
        public ControllerCreationException(Type controllerType) : base("Can't create controller: " + controllerType)
        {
            ControllerType = controllerType;
        }

        public Type ControllerType { get; }
    }
}