using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SpotifyNet;
using SpotiScreen.Model;
using System.IO;

namespace SpotiScreen.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public bool NeedAuthentication { get; set; }

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
                    var secret = JsonConvert.DeserializeObject<SpotifySecrets>(secretLocation);
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