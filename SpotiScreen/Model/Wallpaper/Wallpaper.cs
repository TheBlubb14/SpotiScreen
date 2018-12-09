using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace SpotiScreen.Model
{
    public class Wallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int MAX_PATH = 260;
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPI_GETDESKWALLPAPER = 0x73;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public string Image { get; set; }

        public Style Style { get; set; }

        public Wallpaper(string image, Style style)
        {
            Image = image;
            Style = style;
        }

        public static Wallpaper Get()
        {
            var regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            var wallpaperStyle = regKey.GetValue(@"WallpaperStyle").ToString();
            var tileWallpaper = regKey.GetValue(@"TileWallpaper").ToString();
            var imagePath = new string('\0', MAX_PATH);

            Style style;

            if (wallpaperStyle == "10" && tileWallpaper == "0")
                style = Style.Fill;
            else if (wallpaperStyle == "6" && tileWallpaper == "0")
                style = Style.Fit;
            else if (wallpaperStyle == "22" && tileWallpaper == "0")
                style = Style.Span;
            else if (wallpaperStyle == "2" && tileWallpaper == "0")
                style = Style.Stretch;
            else if (wallpaperStyle == "6" && tileWallpaper == "1")
                style = Style.Tile;
            else // if (wallpaperStyle == "6" && tileWallpaper == "0")
                style = Style.Center;

            SystemParametersInfo(SPI_GETDESKWALLPAPER, imagePath.Length, imagePath, 0);
            return new Wallpaper(imagePath.Substring(0, imagePath.IndexOf('\0')), style);
        }

        public static void Set(Wallpaper wallpaper) => Set(wallpaper.Image, wallpaper.Style);

        public static void Set(string path, Style style)
        {
            var regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            switch (style)
            {
                case Style.Fill:
                    regKey.SetValue("WallpaperStyle", 10.ToString());
                    regKey.SetValue("TileWallpaper", 0.ToString());
                    break;

                case Style.Fit:
                    regKey.SetValue(@"WallpaperStyle", 6.ToString());
                    regKey.SetValue(@"TileWallpaper", 0.ToString());
                    break;

                case Style.Span:
                    regKey.SetValue("WallpaperStyle", 22.ToString());
                    regKey.SetValue("TileWallpaper", 0.ToString());
                    break;

                case Style.Stretch:
                    regKey.SetValue("WallpaperStyle", 2.ToString());
                    regKey.SetValue("TileWallpaper", 0.ToString());
                    break;

                case Style.Tile:
                    regKey.SetValue("WallpaperStyle", 0.ToString());
                    regKey.SetValue("TileWallpaper", 1.ToString());
                    break;

                case Style.Center:
                    regKey.SetValue("WallpaperStyle", 0.ToString());
                    regKey.SetValue("TileWallpaper", 0.ToString());
                    break;

            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
