namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Album
{
    public Album()
    {
        Songs = new HashSet<Song>();    
    }

    public int Id { get; set; }

    [MaxLength(40)]

    public string Name { get; set; }

    public DateTime ReleaseDate { get; set; }

    [NotMapped]

    public decimal Price 
        => Songs.Sum(s => s.Price);

    public int? ProducerId { get; set; }

    public Producer? Producer { get; set; }

    public ICollection<Song> Songs { get; set; }
}
