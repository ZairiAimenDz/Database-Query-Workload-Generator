using System.ComponentModel.DataAnnotations;

namespace DatabaseWorkloadQueryGenerator.Application.Models.UserEntities
{
    public class EmailChangeWithCodeDTO : SendEmailResetCodeDTO
    {
        [Required]
        [Length(6, 6)]
        public string Token { get; set; } = "";
    }
}
