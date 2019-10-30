using Rocket.API;

namespace Tortellio.LobbyEdit
{
    public class Config : IRocketPluginConfiguration
    {
        public bool PluginEnabled;
        public bool HidePlugins;
        public bool EditPlugins;
        public string[] Plugins;
        public bool IsVanilla;
        public bool InvisibleRocket;
        public bool HideWorkshop;
        public bool EditWorkshop;
        public string[] Workshop;
        public bool HideConfig;
        public bool IsPVP;
        public bool IsBattlEyeSecure;
        public string Mode;
        public bool HasCheats;
        public string Perspective;
        public bool IsGold;
        public string GameMode;

        public void LoadDefaults()
        {
            PluginEnabled = true;
            IsVanilla = false;
            InvisibleRocket = false;
            HidePlugins = false;
            EditPlugins = true;
            Plugins = new string[]
            {
                "FEATURES:",
                "TPA",
                "HOME"
            };
            HideWorkshop = false;
            EditWorkshop = true;
            Workshop = new string[]
            {
                "0"
            };
            HideConfig = true;
            IsPVP = false;
            IsBattlEyeSecure = true;
            Mode = "normal";
            HasCheats = false;
            Perspective = "both";
            IsGold = false;
            GameMode = "Zombie Slayer";
        }
    }
}
