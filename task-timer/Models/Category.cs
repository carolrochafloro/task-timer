using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace task_timer.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [Required]
    [MaxLength(400)]
    public string? Description { get; set; }

    public string? ImgUrl { get; set; }

    [Required]
    [ForeignKey("Id")]
    public int UserId { get; set; }

    [JsonIgnore]
    public AppUser User { get; set; }
    public List<Task> Tasks { get; set; }

    public Category()
    {
       Tasks = new List<Task>();
    }


}
