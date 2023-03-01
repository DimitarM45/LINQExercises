namespace MusicHub;

using Data;
using Initializer;

using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

public class StartUp
{
    public static void Main()
    {
        MusicHubDbContext context =
            new MusicHubDbContext();

        DbInitializer.ResetDatabase(context);
    }

    //Problem 1

    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        var albums = context.Albums
            .AsNoTracking()
            .Where(a => a.ProducerId == producerId)
            .Select(a => new
            {
                a.Name,
                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                ProducerName = a.Producer!.Name,
                Songs = a.Songs
                                .Select(s => new
                                {
                                    s.Name,
                                    s.Price,
                                    SongWriterName = s.Writer.Name
                                })
                                .OrderByDescending(s => s.Name)
                                .ThenBy(s => s.SongWriterName)
                                .ToArray(),
                TotalAlbumPrice = a.Price
            })
            .ToArray()
            .OrderByDescending(a => a.TotalAlbumPrice);

        StringBuilder stringBuilder = new StringBuilder();

        foreach (var album in albums)
        {
            stringBuilder.AppendLine($"-AlbumName: {album.Name}");
            stringBuilder.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
            stringBuilder.AppendLine($"-ProducerName: {album.ProducerName}");
            stringBuilder.AppendLine($"-Songs:");

            for (int i = 0; i < album.Songs.Length; i++)
            {
                stringBuilder.AppendLine($"---#{i + 1}");
                stringBuilder.AppendLine($"---SongName: {album.Songs[i].Name}");
                stringBuilder.AppendLine($"---Price: {album.Songs[i].Price:f2}");
                stringBuilder.AppendLine($"---Writer: {album.Songs[i].SongWriterName}");
            }

            stringBuilder.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //Problem 2

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        var songs = context.Songs
            .ToArray()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                s.Name,
                Performers = s.SongPerformers
                    .Select(sp => new { PerformerName = $"{sp.Performer.FirstName} {sp.Performer.LastName}" })
                    .OrderBy(p => p.PerformerName)
                    .ToArray(),
                WriterName = s.Writer.Name,
                AlbumProducer = s.Album!.Producer!.Name,
                Duration = s.Duration.ToString("c")
            })
            .OrderBy(s => s.Name)
            .ThenBy(s => s.WriterName)
            .ToArray();

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < songs.Length; i++)
        {
            stringBuilder.AppendLine($"-Song #{i + 1}");
            stringBuilder.AppendLine($"---SongName: {songs[i].Name}");
            stringBuilder.AppendLine($"---Writer: {songs[i].WriterName}");

            foreach (var performer in songs[i].Performers)
                stringBuilder.AppendLine($"---Performer: {performer.PerformerName}");

            stringBuilder.AppendLine($"---AlbumProducer: {songs[i].AlbumProducer}");
            stringBuilder.AppendLine($"---Duration: {songs[i].Duration}");
        }

        return stringBuilder.ToString().TrimEnd();
    }
}
