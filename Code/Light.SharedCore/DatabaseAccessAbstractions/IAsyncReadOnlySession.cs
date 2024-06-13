using System;

namespace Light.SharedCore.DatabaseAccessAbstractions;

/// <summary>
/// Represents an asynchronous session to the database that
/// is only used to read data. This usually means that no explicit
/// transaction is needed for this session, although implementations can support
/// it to allow using READ UNCOMMITED isolation levels for queries, for example.
/// The connection to the database can be terminated by calling
/// <see cref="IAsyncDisposable.DisposeAsync" /> (or <see cref="IDisposable.Dispose" />).
/// If you want to manipulate data, use <see cref="IAsyncSession" />.
/// </summary>
/// <remarks>
/// Conceptually, a session is identical to a "Unit of Work" as defined in "Patterns of
/// Enterprise Application Architecture" by Martin Fowler et al. It manages the connection
/// to the database and represents a transaction. Strictly speaking, a Unit of Work also needs
/// to do Change Tracking which plain ADO.NET and all Micro-ORMs do not support. For this reason,
/// we chose the term "session" instead of "Unit of Work", also because it is simpler to use in daily life.
/// </remarks>
public interface IAsyncReadOnlySession : IDisposable, IAsyncDisposable;