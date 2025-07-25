using System.ComponentModel.DataAnnotations;

namespace DatabaseWorkloadQueryGenerator.Application.Models.UserEntities
{
    public class SendEmailResetCodeDTO
    {
        [Required, EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; } = "";
    }
}
