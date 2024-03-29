﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Light.SharedCore.DataAccessAbstractions;

/// <summary>
/// Represents an asynchronous session to a database. The
/// connection to the database can be terminated by calling
/// <see cref="IAsyncDisposable.DisposeAsync" /> (or <see cref="IDisposable.Dispose" />).
/// Changes can be saved or committed to the database by calling <see cref="SaveChangesAsync" />.
/// </summary>
/// <remarks>
/// Conceptually, a session is identical to a "Unit of Work".
/// The term "session" is just simpler to use in daily life.
/// </remarks>
public interface IAsyncSession : IAsyncReadOnlySession
{
    /// <summary>
    /// Writes or commits all changes that occurred during the session to the target database.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel this asynchronous operation (optional).</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}