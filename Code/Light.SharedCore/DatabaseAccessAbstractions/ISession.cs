using System;

namespace Light.SharedCore.DatabaseAccessAbstractions;

/// <summary>
/// <para>
/// PLEASE REMEMBER: database calls should be performed asynchronously
/// by default, especially in service apps to avoid blocking threads.
/// Consider using the <see cref="IAsyncSession" /> interface instead.
/// </para>
/// <para>
/// Represents a synchronous session to a database. This means that
/// the implementation will either use a single dedicated transaction in the case of ADO.NET or Micro-ORMs
/// to ensure ACID properties, or the Change Tracking capabilities of a full ORM like Entity Framework Core. The
/// connection to the database can be terminated by calling
/// <see cref="IDisposable.Dispose" />. Changes can be saved or committed
/// to the database by calling <see cref="SaveChanges" />.
/// If your session does not manipulate data, consider deriving your session abstraction from
/// the <see cref="IDisposable" /> interface instead.
/// </para>
/// </summary>
/// <remarks>
/// Conceptually, a session is identical to a "Unit of Work" as defined in "Patterns of
/// Enterprise Application Architecture" by Martin Fowler et al. It manages the connection
/// to the database and represents a transaction. Strictly speaking, a Unit of Work also needs
/// to do Change Tracking which plain ADO.NET and all Micro-ORMs do not support. For this reason,
/// we chose the term "session" instead of "Unit of Work", also because it is simpler to use in daily life.
/// </remarks>
public interface ISession : IDisposable
{
    /// <summary>
    /// Writes or commits all changes that occurred during the session to the target database.
    /// </summary>
    void SaveChanges();
}