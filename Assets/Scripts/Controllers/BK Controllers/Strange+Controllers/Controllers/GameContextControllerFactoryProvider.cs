namespace Infra.Controllers.Core
{
    public class GameContextControllerFactoryProvider
    {
        public static IControllerFactory ControllerFactory { get; private set; }

        public static void Setup(IControllerFactory controllerFactory)
        {
            ControllerFactory = controllerFactory;
        }
    }
}