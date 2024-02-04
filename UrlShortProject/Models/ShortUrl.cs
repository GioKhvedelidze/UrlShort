namespace UrlShortProject.Models;

public class ShortUrl
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string CompressedUrl { get; set; } = "";
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}