using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Models.UserEntities
{
    public class SendPhoneCodeDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = "";
    }
}
