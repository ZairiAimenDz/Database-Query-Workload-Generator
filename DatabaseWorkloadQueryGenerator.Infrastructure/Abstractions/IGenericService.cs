using DatabaseWorkloadQueryGenerator.Application.Filters;
using DatabaseWorkloadQueryGenerator.Application.Wrappers;
using DatabaseWorkloadQueryGenerator.Core.Entities;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Abstractions;

/// <summary>
/// Generic service for implementing common CRUD operations by default
/// </summary>
/// <typeparam name="TEntity">The type of the entity used in the database table</typeparam>
/// <typeparam name="TEntityDto">The type of the entity read dto</typeparam>
/// <typeparam name="TEntityCreateDTO">The type used to create a new entity</typeparam>
/// <typeparam name="TEntityUpdateDTO">The type used to updated an existing entity</typeparam>
/// <typeparam name="TEntityId">The type of the id of the database entity</typeparam>
public interface IGenericService<TEntity, TEntityDto, TEntityCreateDTO, TEntityUpdateDTO, TEntityId> where TEntity : BaseModel<TEntityId>
{
    /// <summary>
    /// Returns paged and filtered elements
    /// </summary>
    /// <param name="filter">The filter to apply for the items to return</param>
    /// <returns></returns>
    Task<Response<Page<TEntityDto>>> GetFilteredDataAsync(RequestFilter filter);

    /// <summary>
    /// Returns all the elements
    /// </summary>
    Task<Response<List<TEntityDto>>> GetAllAsync();

    /// <summary>
    /// Gets an entity based on its id
    /// </summary>
    /// <param name="id">The id of the entity to return</param>
    /// <returns></returns>
    Task<Response<TEntityDto>> GetByIdAsync(TEntityId id);

    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="dto">The new entity to create</param>
    /// <returns></returns>
    Task<Response<TEntityDto>> CreateAsync(TEntityCreateDTO dto);

    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    /// <param name="id">The id of the entity to update</param>
    /// <param name="dto">The entity details to update</param>
    /// <returns></returns>
    Task<Response<TEntityDto>> UpdateAsync(TEntityId id, TEntityUpdateDTO dto);

    /// <summary>
    /// Deletes an entity in the database
    /// </summary>
    /// <param name="id">The id of the entity to delete</param>
    /// <returns></returns>
    Task<Response> DeleteAsync(TEntityId id);

    /// <summary>
    /// Soft deletes / archives an entity in the database
    /// </summary>
    /// <param name="id">The id of the entity to archive</param>
    /// <returns></returns>
    Task<Response> ArchiveAsync(TEntityId id);

    /// <summary>
    /// Unarchives a previously archived entity
    /// </summary>
    /// <param name="id">The id of the entity to unarchive</param>
    /// <returns>The unarchived entity</returns>
    Task<Response<TEntityDto>> UnarchiveAsync(TEntityId id);
}
