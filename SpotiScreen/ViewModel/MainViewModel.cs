using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SpotifyNet;
using SpotiScreen.Model;
using System.IO;

namespace SpotiScreen.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // TODO: user setting
        private string secretLocation = @"..\..\..\spotify.secret";

        private Screen screen;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {

            }
            else
            {
                if (File.Exists(secretLocation))
                {
                    var secret = JsonConvert.DeserializeObject<SpotifySecrets>(File.ReadAllText(secretLocation));
                    screen = new Screen(secret);
                }
            }
        }

        public override void Cleanup()
        {
            screen?.Dispose();

            base.Cleanup();
        }
    }
}