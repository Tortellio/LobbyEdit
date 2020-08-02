using Rocket.API;

namespace Tortellio.LobbyEdit
{
    public class Config : IRocketPluginConfiguration
    {
        public bool HidePlugins;
        public bool EditPlugins;
        public string[] Plugins;
        public bool IsVanilla;
        public bool HideRocket;
        /*public bool HideWorkshop;
        public bool EditWorkshop;
        public string[] Workshop;*/
        public bool HideConfig;
        public bool IsPVP;
        public bool IsBattlEyeSecure;
        public string Mode;
        public bool HasCheats;
        public string Perspective;
        public bool IsGold;
        public string GameMode;
        public bool HideThumbnail;
        public bool EditThumbnail;
        public string Thumbnail;

        public void LoadDefaults()
        {
            IsVanilla = false;
            HideRocket = false;
            HideConfig = true;
            HidePlugins = false;
            EditPlugins = true;
            Plugins = new string[]
            {
                "FEATURES:",
                "TPA",
                "HOME"
            };
            /*HideWorkshop = false;
            EditWorkshop = true;
            Workshop = new string[]
            {
                "0"
            };*/
            HideThumbnail = false;
            EditThumbnail = false;
            Thumbnail = "url.com";
            IsPVP = false;
            GameMode = "Zombie Slayer";
            HasCheats = false;
            Mode = "normal";
            Perspective = "both";
            IsGold = false;
            IsBattlEyeSecure = true;
        }
    }
}
