using Newtonsoft.Json;
using SpotifyNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Timers;
using System.Windows.Media.Imaging;

namespace SpotiScreen.Model
{
    public class Screen : IDisposable
    {
        private const string PATH = "orginal-wallpaper.json";

        public TimeSpan RefreshRate
        {
            get => refreshRate;
            set
            {
                refreshRate = value;
                timer.Stop();
                timer.Interval = refreshRate.TotalMilliseconds;
                timer.Start();
            }
        }

        private readonly Timer timer;
        private readonly FileInfo imagePath = new FileInfo("current_album_cover.bmp");

        private TimeSpan refreshRate;
        private Spotify spotify;
        private HttpClient httpClient;

        public Screen(SpotifySecrets spotifySecrets)
        {
            BackupOriginalWallpaper();

            httpClient = new HttpClient();
            refreshRate = TimeSpan.FromSeconds(10);
            spotify = new Spotify(spotifySecrets);
            timer = new Timer();

            timer.AutoReset = true;
            timer.Interval = refreshRate.TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // TODO: check if song has changed
                var currentlyPlaying = await spotify.GetCurrentlyPlayingAsync();

                // nothing playing
                if (currentlyPlaying == null)
                    return;

                var url = currentlyPlaying.Item.Album.Images.FirstOrDefault(x => x.Width == currentlyPlaying.Item.Album.Images.Max(y => y.Width)).Url;

                File.WriteAllBytes(imagePath.FullName, await httpClient.GetByteArrayAsync(url));

                // TODO: user settiing Style 
                Wallpaper.Set(imagePath.FullName, Style.Tile);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Console.WriteLine(ex.ToString());
            }
        }

        private void BackupOriginalWallpaper()
        {
            // in case application didnt shutdown properly and couldnt restore original wallpaper
            File.WriteAllText(PATH, JsonConvert.SerializeObject(ReadBackupWallpaper() ?? Wallpaper.Get()));
        }

        private void RestoreOriginalWallpaper()
        {
            var backupWallpaper = ReadBackupWallpaper();

            if (backupWallpaper != null)
                Wallpaper.Set(backupWallpaper.Image, backupWallpaper.Style);
        }

        private Wallpaper ReadBackupWallpaper()
        {
            if (File.Exists(PATH))
                return JsonConvert.DeserializeObject<Wallpaper>(File.ReadAllText(PATH));
            else
                return null;
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();

            RestoreOriginalWallpaper();
        }
    }
}
