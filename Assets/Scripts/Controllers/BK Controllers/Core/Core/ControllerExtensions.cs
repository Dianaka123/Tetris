using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Controllers.Core
{
    public static class ControllerExtensions
    {
        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            object arg,
            IControllerFactory factory,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                arg,
                factory,
                resultCancellationToken,
                controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            object arg,
            IControllerFactory factory,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                arg,
                factory,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            object arg,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                arg,
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            object arg,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                arg,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            IControllerFactory factory,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                factory: factory,
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            IControllerFactory factory,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                factory: factory,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<ControllerResult>> CreateAndWaitResultAsync<T>(
            this ControllerBase thisController,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, ControllerResult>(
                controllerCancellationToken: controllerCancellationToken);
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            object arg,
            IControllerFactory factory,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                arg,
                factory,
                resultCancellationToken,
                controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            object arg,
            IControllerFactory factory,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                arg,
                factory,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            object arg,
            CancellationToken controllerCancellationToken,
            IViewContext viewContext = null)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                arg,
                controllerCancellationToken: controllerCancellationToken,
                viewContext: viewContext);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            object arg,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                arg,
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            IControllerFactory factory,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                factory: factory,
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            IControllerFactory factory,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                factory: factory,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            CancellationToken resultCancellationToken,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                resultCancellationToken: resultCancellationToken,
                controllerCancellationToken: controllerCancellationToken);
        }

        public static Task<IOperationResult<TResult>> CreateAndWaitResultAsync<T, TResult>(
            this ControllerBase thisController,
            CancellationToken controllerCancellationToken)
            where T : ControllerWithResultBase<TResult>
        {
            return thisController.CreateAndWaitResultAsyncInternal<T, TResult>(
                controllerCancellationToken: controllerCancellationToken);
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        private static async Task<IOperationResult<TResult>> CreateAndWaitResultAsyncInternal<T, TResult>(
            this ControllerBase thisController,
            object arg = default,
            IControllerFactory factory = default,
            CancellationToken resultCancellationToken = default,
            CancellationToken controllerCancellationToken = default,
            IViewContext viewContext = null)
            where T : ControllerWithResultBase<TResult>
        {
            var stopToken = thisController.CancellationToken;

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(stopToken, resultCancellationToken,
                controllerCancellationToken))
            {
                var token = cts.Token;

                if (token.IsCancellationRequested)
                    return new OperationResult<TResult>(new OperationCanceledException(token));

                T controller = null;

                try
                {
                    controller = await thisController.CreateAndStartAsyncInternal<T>(arg, factory,
                        controllerCancellationToken, viewContext);

                    try
                    {
                        var result = await controller.GetResult(token).ConfigureAwait(true);
                        try
                        {
                            await controller.StopAsync();
                        }
                        catch (OperationCanceledException operationCanceledException)
                        {
                            return new OperationResult<TResult>(operationCanceledException);
                        }
                        catch (Exception exception)
                        {
                            throw new StopControllerException(exception);
                        }

                        return new OperationResult<TResult>(result);
                    }
                    catch (OperationCanceledException operationCanceledException)
                    {
                        //TODO fix controllers StopAsync to remove this crutch
                        if (controller.State == ControllerState.Running) await controller.StopAsync();

                        return new OperationResult<TResult>(operationCanceledException);
                    }
                    catch (StopControllerException stopControllerException)
                    {
                        throw stopControllerException.Exception;
                    }
                }
                catch (OperationCanceledException e)
                {
                    return new OperationResult<TResult>(e);
                }
                catch
                {
                    if (controller != null &&
                        controller.State == ControllerState.Running)
                        await controller.StopAsync();

                    throw;
                }
                finally
                {
                    if (controller != null) thisController.DisposeAndRemoveController(controller);
                }
            }
        }
    }
}