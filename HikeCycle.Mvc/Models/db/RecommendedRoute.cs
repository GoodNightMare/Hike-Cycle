using System.ComponentModel.DataAnnotations.Schema;

[Table("recommended_routes")]
public class RecommendedRoute {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Province { get; set; } = "";
    public string? Duration { get; set; }
    public string? Distance { get; set; }
    public string? Level { get; set; }
    public string? Highlight { get; set; }
    public string? Suitable { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}