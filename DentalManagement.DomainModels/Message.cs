using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Name {get; set;} = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Messages { get; set; } = string.Empty;
        public DateTime Timestamp {  get; set; }
        public bool NewMessage { get; set; }
    }
}
