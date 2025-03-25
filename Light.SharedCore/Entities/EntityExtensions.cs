using System;

namespace Light.SharedCore.Entities;

/// <summary>
/// Provides extension methods for entities.
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    /// <para>
    /// Casts the entity to <see cref="IMutableId{T}" />. You would most likely do that when
    /// you want to set the ID of an entity after its creation.
    /// </para>
    /// <para>
    /// BE CAREFUL: you must not change the ID of your Entity when it should already be immutable.
    /// This is e.g. the case when the ID is used as a key within a dictionary.
    /// </para>
    /// <para>
    /// However, when inserting an entity into a database, the database will usually generate the ID
    /// of the entity at this point in time. Updating the ID after the insert is completely OK and
    /// is the usual scenario where this method should be called.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the ID of the entity.</typeparam>
    /// <param name="entity">The entity to be cast.</param>
    /// <returns>The cast entity</returns>
    /// <exception cref="InvalidCastException">Thrown when the specified entity cannot be cast to </exception>
    public static IMutableId<T> ToMutable<T>(this IEntity<T> entity) where T : IEquatable<T> => (IMutableId<T>) entity;
}
