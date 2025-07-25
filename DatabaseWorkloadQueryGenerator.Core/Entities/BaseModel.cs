using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Core.Entities;


/// <summary>
/// Base model for all entities in the database
/// </summary>
/// <typeparam name="TId">The type of the table primary key</typeparam>
public class BaseModel<TId>
{
    /// <summary>
    /// Primary key of the table
    /// </summary>
    public required TId Id { get; set; }

    /// <summary>
    /// The id of the user that created the entity
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// When was the entity created
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// The id of the last user that updated the entity
    /// </summary>
    public Guid? LastUpdatedBy { get; set; }

    /// <summary>
    /// When was the entity last updated
    /// </summary>
    public DateTime? LastUpdatedAtUtc { get; set; }

    /// <summary>
    /// The id of the user that soft deleted / archived the entity
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// When was the entity soft deleted / archived
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>
    /// Is the entity soft deleted / archived
    /// </summary>
    /// <remarks>
    /// If the entity has been soft deleted, the UpdatedAt and UpdatedBy properties will no longer change if we ever update the entity
    /// they will only change when DeletedAt is null
    /// </remarks>
    [NotMapped]
    public bool IsDeleted
    {
        get => DeletedAtUtc != null;
    }

    #region Helper Methods

    /// <summary>
    /// Updates the entity properties to be marked as soft deleted
    /// </summary>
    /// <param name="userId">The id of the user that deleted the entity</param>
    public void MarkDeleted(Guid userId)
    {
        // Set the deleted at time
        DeletedAtUtc = DateTime.UtcNow;

        // Set the id of the user that deleted the entity
        DeletedBy = userId;
    }

    /// <summary>
    /// Updates the entity properties to cancel soft-deletion
    /// </summary>
    public void MarkNonDeleted()
    {
        DeletedAtUtc = null;
        DeletedBy = null;
    }

    #endregion
}


/// <summary>
/// Entities that implement this interface will have a guard against deletion, which will be specified using a DisableDeletion property
/// </summary>
public interface IDeletionGuard
{
    /// <summary>
    /// Whether we disable the deletion of this instance or not
    /// </summary>
    bool DisableDeletion { get; set; }

    /// <summary>
    /// Checks if the entity can be deleted by the user with the specified id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool CanBeDeleted(Guid userId);
}


/// <summary>
/// Entities that implement this interface will have a guard against edition, which will be specified using a DisableEdition property
/// </summary>
public interface IEditionGuard
{
    /// <summary>
    /// Whether we disable the edition of this instance or not
    /// </summary>
    bool DisableEdition { get; set; }

    /// <summary>
    /// Checks if the entity can be edited by the user with the specified id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool CanBeEdited(Guid userId);
}