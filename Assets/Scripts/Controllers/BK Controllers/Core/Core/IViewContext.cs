using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Controllers.Core
{
    /// <summary>
    ///     When to use ShowModalAsync and LockAsync:
    ///     you have more than one concurrent element, generally popup or feature,
    ///     and have to avoid overlaying. These methods can't guarantee order.
    ///     When to use CreateChild:
    ///     you have to show something on locked ViewContext
    /// </summary>
    public interface IViewContext : IDisposable
    {
        int Id { get; }

        /// <summary>
        ///     How to use
        ///     await ViewContext.ShowModalAsync(() => do something..., token)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task ShowModalAsync(Func<Task> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     How to use
        ///     using (var unlockedContext = await viewContext.LockAsync(token))
        ///     {
        ///     if (!stopToken.IsCancellationRequested)
        ///     {
        ///     // do something ...
        ///     // unlockedContext.ShowModalAsync()
        ///     }
        ///     }
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        Task<IViewContext> LockAsync(CancellationToken cancellationToken = default);

        IViewContext CreateChild();
    }
}