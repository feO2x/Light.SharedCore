using System;
using System.Threading;
using System.Threading.Tasks;

namespace Light.SharedCore.DatabaseAccessAbstractions;

/// <summary>
/// <para>
/// Represents the base interface for an asynchronous session to a database that manipulates data. This means that
/// the implementation will either use a single dedicated transaction in the case of ADO.NET or Micro-ORMs
/// to ensure ACID properties, or the change tracking capabilities of a full ORM like Entity Framework Core.
/// </para>
/// <para>
/// The connection to the database can be terminated by calling
/// <see cref="IAsyncDisposable.DisposeAsync" />, the underlying transaction
/// will be automatically rolled back if <see cref="SaveChangesAsync" /> was not called beforehand.
/// </para>
/// <para>
/// If you don't want the caller to explicitly commit the changes, consider deriving your session from
/// <see cref="IAsyncDisposable" /> directly.
/// </para>
/// </summary>
/// <remarks>
/// Conceptually, a session is identical to a "Unit of Work" as defined in "Patterns of
/// Enterprise Application Architecture" by Martin Fowler et al. It manages the connection
/// to the database and represents a transaction. Strictly speaking, a Unit of Work also needs
/// to do Change Tracking which plain ADO.NET and all Micro-ORMs do not support. For this reason,
/// we chose the term "session" instead of "Unit of Work", also because it is simpler to use in daily life.
/// </remarks>
public interface ISession : IAsyncDisposable
{
    /// <summary>
    /// Commits all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The token to cancel this asynchronous operation (optional).</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
