using System;
using System.Linq;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ModMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

using UILib.Components;
using UILib.Layouts;
using UILib.Misc;
using UILib.Patches;
using UIButton = UILib.Components.Button;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UILib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private Intro intro;
        private ThemePicker themePicker;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            UILib.Config.Init(Config);
            Patcher.Awake();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (AccessTools.AllAssemblies().FirstOrDefault(
                    a => a.GetName().Name == "ModMenu"
                ) != null
            ) {
                Register();
            }
        }

        /**
         * <summary>
         * Register with Mod Menu.
         * </summary>
         */
        private void Register() {
            ModInfo info = ModManager.Register(this);
            info.license = "GPL-3.0";

            info.Add(typeof(UILib.Config));

            // Button to display the intro
            info.onBuild.AddListener((ModView view) => {
                Area area = new Area();
                area.SetFill(FillType.All);
                area.SetContentLayout(LayoutType.Vertical);
                area.SetElementSpacing(20f);

                UIButton themeButton = new UIButton("Change Theme", 20);
                themeButton.SetSize(200f, 40f);
                themeButton.onClick.AddListener(() => {
                    if (themePicker == null) {
                        themePicker = new ThemePicker();
                    }

                    themePicker.ToggleVisibility();
                });

                UIButton introButton = new UIButton("Show Intro", 20);
                introButton.SetSize(200f, 40f);
                introButton.onClick.AddListener(() => {
                    if (intro == null) {
                        intro = new Intro();
                    }

                    intro.window.ToggleVisibility();
                });

                area.Add(themeButton);
                area.Add(introButton);

                view.Add("Extras", area);
            });
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            // Register built-in themes
            Theme.RegisterBuiltIn();

            UIRoot.Init();

            if (UILib.Config.showIntro.Value == true) {
                intro = new Intro();
                intro.window.Show();
            }
        }

        /**
         * <summary>
         * Executes when this plugin is destroyed.
         * </summary>
         */
        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes when a scene is loaded.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         * <param name="mode">The mode the scene loaded with</param>
         */
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Patcher.OnSceneLoaded(scene, mode);
            UIRoot.InitWM();
        }

        /**
         * <summary>
         * Executes when a scene is unloaded.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         * <param name="mode">The mode the scene unloaded with</param>
         */
        private void OnSceneUnloaded(Scene scene) {
            Patcher.OnSceneUnloaded(scene);
        }

        /**
         * <summary>
         * Executes every frame.
         * </summary>
         */
        private void Update() {
            UIRoot.Update();
            Patcher.Update();
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] UILib: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] UILib: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] UILib: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
