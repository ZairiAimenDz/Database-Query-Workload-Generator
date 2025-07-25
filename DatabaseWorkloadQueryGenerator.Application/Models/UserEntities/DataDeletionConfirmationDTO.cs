using System.ComponentModel.DataAnnotations;

namespace DatabaseWorkloadQueryGenerator.Application.Models.UserEntities
{
    public class DataDeletionConfirmationDTO
    {
        /// <summary>
        /// User's Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
