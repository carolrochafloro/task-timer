using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace task_timer.Models;

public class AppTask
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(400)]
    public string? Obs { get; set; }

    public DateTime Beginning { get; set; }

    public DateTime End { get; set; }

    public TimeSpan Duration { get; set; }

  
    [Required]
    [ForeignKey("Id")]
    public string AspNetUsersId { get; set; }

    [Required]
    [ForeignKey("Id")]
    public int CategoryId { get; set; }

    // The navigation properties will not be serialized

    [JsonIgnore]
    public Category? Category { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }

}
