using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using SDG.Unturned;
using Steamworks;
using SDG.Framework.Modules;
using NuGet.Protocol.Plugins;
using System.Linq;

[assembly: PluginMetadata("Tortellio.LobbyEdit", Author = "Tortellio", DisplayName = "LobbyEdit",
    Website = "https://github.com/Tortellio/LobbyEdit")]
namespace Tortellio.LobbyEdit
{
    public class LobbyEdit : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<LobbyEdit> m_Logger;
        private readonly IPluginActivator m_PluginActivator;
        public LobbyEdit(
            IConfiguration configuration, 
            IStringLocalizer stringLocalizer,
            ILogger<LobbyEdit> logger,
            IPluginActivator pluginActivator,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_PluginActivator = pluginActivator;
        }

        protected override async UniTask OnLoadAsync()
        {
			await UniTask.SwitchToMainThread();

            Level.onPostLevelLoaded += OnPostLevelLoaded;
            m_Logger.LogInformation("LobbyEdit by Tortellio has been loaded.");
            if (Level.isLoaded)
            {
                m_Logger.LogInformation(m_StringLocalizer["PLUGIN_EVENTS:OVERRIDE_START"]);
                EditLobby();
                m_Logger.LogInformation(m_StringLocalizer["PLUGIN_EVENTS:OVERRIDE_STOP"]);
            }
            await UniTask.SwitchToThreadPool();
        }

        protected override async UniTask OnUnloadAsync()
        {
            await UniTask.SwitchToMainThread();

            Level.onPostLevelLoaded -= OnPostLevelLoaded;
            m_Logger.LogInformation("LobbyEdit by Tortellio has been unloaded.");
        }
        private async void OnPostLevelLoaded(int level)
        {
            await UniTask.SwitchToMainThread();

            m_Logger.LogInformation(m_StringLocalizer["PLUGIN_EVENTS:OVERRIDE_START"]);
            EditLobby();
            m_Logger.LogInformation(m_StringLocalizer["PLUGIN_EVENTS:OVERRIDE_STOP"]);

            await UniTask.SwitchToThreadPool();
        }
        private void EditLobby()
        {
            string mode;
            string perspective;
            string thumbnail;
            bool workshop = Provider.getServerWorkshopFileIDs().Count > 0;

            #region Plugins
            if (m_Configuration.GetSection("Hide:Rocket").Get<bool>())
            {
                SteamGameServer.SetBotPlayerCount(0);
            }

            if (!m_Configuration.GetSection("Hide:Plugins").Get<bool>())
            {
                if (m_Configuration.GetSection("Edit:Plugins").Get<bool>()) { SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", m_Configuration.GetSection("PluginsOverride").Get<string[]>())); }
                else { SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", m_PluginActivator.ActivatedPlugins.ToList().Select(p => p.DisplayName).ToArray())); }
            }
            else
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }

            if (m_Configuration.GetSection("GameTags:Vanilla").Get<bool>())
            {
                SteamGameServer.SetBotPlayerCount(0);
                SteamGameServer.SetKeyValue("rocketplugins", "");
                SteamGameServer.SetKeyValue("rocket", "");
            }
            else
            {
                if (!m_Configuration.GetSection("Hide:Rocket").Get<bool>())
                {
                    SteamGameServer.SetBotPlayerCount(1);
                }

                if (!m_Configuration.GetSection("Hide:Plugins").Get<bool>() && !m_Configuration.GetSection("Edit:Plugins").Get<bool>())
                {
                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", m_PluginActivator.ActivatedPlugins.ToList().Select(p => p.DisplayName).ToArray()));
                }

                string version = ModuleHook.modules.Find(a => a.config.Name == "Rocket.Unturned")?.config.Version ?? "4.9.3.3";
                SteamGameServer.SetKeyValue("rocket", version);
            }
            #endregion

            #region Workshops
            /*if (Configuration.Instance.HideWorkshop)
            {
                workshop = false;
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
            }
            else if (Configuration.Instance.EditWorkshop)
            {
                workshop = true;
                string txt = string.Join(",", Configuration.Instance.Workshop);
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
            }*/
            #endregion

            #region Configs
            if (m_Configuration.GetSection("Hide:Config").Get<bool>()) { SteamGameServer.SetKeyValue("Browser_Config_Count", "0"); }
            else { SteamGameServer.SetKeyValue("Browser_Config_Count", GetConfigurationCount().ToString()); }

            switch (m_Configuration["GameTags:Mode"].ToLower().Trim())
            {
                case "easy":
                    mode = "EZY";
                    break;
                case "hard":
                    mode = "HRD";
                    break;
                default:
                    mode = "NRM";
                    break;
            }

            switch (m_Configuration["GameTags:Perspective"].ToLower().Trim())
            {
                case "first":
                    perspective = "1Pp";
                    break;
                case "third":
                    perspective = "3Pp";
                    break;
                case "vehicle":
                    perspective = "4Pp";
                    break;
                default:
                    perspective = "2Pp";
                    break;
            }
            #endregion

            #region Thumbnail
            if (!m_Configuration.GetSection("Hide:Thumbnail").Get<bool>())
            {
                switch (m_Configuration.GetSection("Edit:Thumbnail").Get<bool>())
                {
                    case true:
                        thumbnail = m_Configuration["ThumbnailOverride"];
                        break;
                    case false:
                        thumbnail = Provider.configData.Browser.Thumbnail;
                        break;
                    default:
                        thumbnail = Provider.configData.Browser.Thumbnail;
                        break;
                }
            }
            else
            {
                thumbnail = "";
            }
            #endregion

            #region GameTags
            string tags = string.Concat(new string[]
            {
                m_Configuration.GetSection("GameTags:PVP").Get<bool>() ? "PVP" : "PVE",
                ",<gm>",
                m_Configuration["GameTags:GameMode"],
                "</gm>,",
                m_Configuration.GetSection("GameTags:Cheats").Get<bool>() ? "CHy" : "CHn",
                ",",
                mode,
                ",",
                perspective,
                ",",
                workshop ? "WSy" : "WSn",
                ",", m_Configuration.GetSection("GameTags:Gold").Get<bool>() ? "GLD" : "F2P",
                ",", m_Configuration.GetSection("GameTags:BattlEyeSecure").Get<bool>() ? "BEy" : "BEn",
                ",<tn>", thumbnail, "</tn>"
            });
            SteamGameServer.SetGameTags(tags);
            #endregion
        }
        private int GetConfigurationCount()
        {
            return (string.Join(",", typeof(ModeConfigData).GetFields()
                .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
                .Select(x => x is bool v ? v ? "T" : "F" : (string.Empty + x)).ToArray()).Length - 1) / 120 + 1;
        }
    }
}
