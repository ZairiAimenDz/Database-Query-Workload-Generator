using System.ComponentModel.DataAnnotations;

namespace DatabaseWorkloadQueryGenerator.Application.Models.UserEntities
{
    public class PhoneChangeWithCodeDTO : SendPhoneCodeDTO
    {
        [Required]
        [Length(6, 6)]
        public string Token { get; set; } = "";
    }
}
