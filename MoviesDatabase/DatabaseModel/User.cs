using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel
{
    [Table("Users")]
    public class User
    {
        [Key]
        public ulong Id { get; set; }
        [Required][Index]
        public string ApiIdentifier { get; set; }
        [Required]
        public UserSettings Settings { get; set; }
    }
}
