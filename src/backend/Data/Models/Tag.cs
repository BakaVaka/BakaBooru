namespace Server.Data.Models;

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<Picture> Picutres { get; set; }
}