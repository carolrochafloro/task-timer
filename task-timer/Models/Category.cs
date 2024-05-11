using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace task_timer.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(400)]
    public string? Description { get; set; }

    [Required]
    [ForeignKey("Id")]
    public string AspNetUsersId { get; set; }


    [JsonIgnore]
    public List<AppTask> appTasks { get; set; }
    public Category()
    {
        appTasks = new List<AppTask>();
    }

}
