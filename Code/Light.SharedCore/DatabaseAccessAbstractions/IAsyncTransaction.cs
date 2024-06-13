using System;
using System.Threading;
using System.Threading.Tasks;

namespace Light.SharedCore.DatabaseAccessAbstractions;

/// <summary>
/// Represents an asynchronous transaction that can be committed. The transaction should always be disposed.
/// A rollback is performed automatically on <see cref="IAsyncDisposable.DisposeAsync" /> when <see cref="CommitAsync"/> was not
/// called beforehand.
/// </summary>
public interface IAsyncTransaction : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Commits the changes made during this transaction to the database.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel this asynchronous operation (optional).</param>
    Task CommitAsync(CancellationToken cancellationToken = default);
}