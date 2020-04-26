using System;
using System.Linq;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using SDG.Framework.Modules;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace Tortellio.LobbyEdit
{
    public class LobbyEdit : RocketPlugin<Config>
    {
        public static LobbyEdit Instance;
        public static string PluginName = "LobbyEdit";
        public static string PluginVersion = "1.0.0";
        protected override void Load()
        {
            if (Configuration.Instance.PluginEnabled)
            {
                Instance = this;
                Logger.Log("LobbyEdit has been loaded!");
                Logger.Log(PluginName + PluginVersion, ConsoleColor.Yellow);
                Logger.Log("Made by Tortellio", ConsoleColor.Yellow);
                if (Level.isLoaded)
                {
                    EditLobby();
                }
                Level.onPostLevelLoaded += OnPostLevelLoaded;
            }
            else
            {
                Logger.Log("LobbyEdit has been loaded!");
                Logger.Log(PluginName + PluginVersion, ConsoleColor.Yellow);
                Logger.Log("Made by Tortellio", ConsoleColor.Yellow);
                Logger.Log("LobbyEdit is disabled in Configuration", ConsoleColor.Red);
                UnloadPlugin();
            }
        }
        protected override void Unload()
        {
            Instance = null;
            Logger.Log("LobbyEdit has been unloaded!");
            Logger.Log("Visit Tortellio Discord for more! https://discord.gg/pzQwsew");
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
        }

        public static int GetWorkshopCount() =>
            (String.Join(",", Provider.getServerWorkshopFileIDs().Select(x => x.ToString()).ToArray()).Length - 1) / 120 + 1;
        public static int GetConfigurationCount() =>
            (String.Join(",", typeof(ModeConfigData).GetFields()
            .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
            .Select(x => x is bool v ? v ? "T" : "F" : (String.Empty + x)).ToArray()).Length - 1) / 120 + 1;

        public void OnPostLevelLoaded(int a) => EditLobby();

        public void EditLobby()
        {
            string mode;
            string perspective;
            bool workshop = Provider.getServerWorkshopFileIDs().Count > 0;

            #region Plugins
            if (Configuration.Instance.InvisibleRocket)
            {
                SteamGameServer.SetBotPlayerCount(0);
            }

            if (!Configuration.Instance.HidePlugins)
            {
                if (Configuration.Instance.EditPlugins)
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Configuration.Instance.Plugins));
                else
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
            }
            else
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }

            if (Configuration.Instance.IsVanilla)
            {
                SteamGameServer.SetBotPlayerCount(0);
                SteamGameServer.SetKeyValue("rocketplugins", "");
                SteamGameServer.SetKeyValue("rocket", "");
            }
            else
            {
                if (!Configuration.Instance.InvisibleRocket)
                {
                    SteamGameServer.SetBotPlayerCount(1);
                }
                if (!Configuration.Instance.HidePlugins && !Configuration.Instance.EditPlugins)
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
                string version = ModuleHook.modules.Find(a => a.config.Name == "Rocket.Unturned")?.config.Version ?? "0.0.0.69";
                SteamGameServer.SetKeyValue("rocket", version);
            }
            #endregion

            #region Workshops
            if (Configuration.Instance.HideWorkshop)
            {
                workshop = false;
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
            }
            else if (Configuration.Instance.EditWorkshop)
            {
                workshop = true;
                string txt = String.Join(",", Configuration.Instance.Workshop);
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", ((txt.Length - 1) / 120 + 1).ToString());

                int line = 0;
                for (int i = 0; i < txt.Length; i += 120)
                {
                    int num6 = 120;

                    if (i + num6 > txt.Length)
                        num6 = txt.Length - i;

                    string pValue2 = txt.Substring(i, num6);
                    SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + line, pValue2);
                    line++;
                }
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", GetWorkshopCount().ToString());
            }
            #endregion

            #region Configs
            if (Configuration.Instance.HideConfig)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            else
                SteamGameServer.SetKeyValue("Browser_Config_Count", GetConfigurationCount().ToString());

            switch (Configuration.Instance.Mode.ToLower().Trim())
            {
                case ("easy"):
                    mode = "EZY";
                    break;
                case ("hard"):
                    mode = "HRD";
                    break;
                default:
                    mode = "NRM";
                    break;
            }
            switch (Configuration.Instance.Perspective.ToLower().Trim())
            {
                case ("first"):
                    perspective = "1Pp";
                    break;
                case ("third"):
                    perspective = "3Pp";
                    break;
                case ("vehicle"):
                    perspective = "4Pp";
                    break;
                default:
                    perspective = "2Pp";
                    break;
            }
            #endregion

            #region Configuration
            string tags = String.Concat(new string[]
            {
                Configuration.Instance.IsPVP ? "PVP" : "PVE",
                ",",
                Configuration.Instance.HasCheats ? "CHy" : "CHn",
                ",",
                mode,
                ",",
                perspective,
                ",",
                Configuration.Instance.HideWorkshop ? "WSn" : "WSy",
                ",",
                Configuration.Instance.IsGold ? "GLD" : "F2P",
                ",",
                Configuration.Instance.IsBattlEyeSecure ? "BEy" : "BEn",
                ",<gm>",
                Configuration.Instance.GameMode,
                "</gm>",
            });
            SteamGameServer.SetGameTags(tags);
            #endregion
        }
    }
}