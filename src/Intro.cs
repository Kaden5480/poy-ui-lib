using BepInEx.Configuration;
using UnityEngine;

using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib {
    /**
     * <summary>
     * Displays an introduction to UILib.
     * </summary>
     */
    internal class Intro {
        private Theme theme = new Theme();
        private Window window;

        /**
         * <summary>
         * Adds a label to an object
         * </summary>
         */
        private Label AddLabel(UIObject obj, string text, float width, int fontSize) {
            Label label = new Label(text, fontSize);
            label.SetSize(width, fontSize + 10f);
            obj.Add(label);
            return label;
        }

        private Label AddSmall(UIObject obj, string text, float width, int fontSize) {
            Label label = AddLabel(obj, text, width, fontSize);
            label.SetColor(theme.selectAltHighlight);
            return label;
        }

        /**
         * <summary>
         * Initializes the intro UI.
         * </summary>
         */
        internal Intro() {
            theme.fontLineSpacing = 4;

            window = new Window("UILib Intro", 650f, 550f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetElementSpacing(5);

            AddLabel(window, "Welcome to UILib!", 400f, 40);
            AddSmall(window,
                "This is a library built for making UIs more easily,"
                + " like the one you're seeing right now."
                + " It's worth checking over the details below to"
                + " familiarise yourself.",
                400f, 20
            ).SetSize(400f, 80f);

            window.Add(new Area(0f, 25f));

            // Keybinds
            AddLabel(window, "Useful Shortcuts", 200f, 28);

            foreach (string text in new[] {
                "Double clicking the title bar of a window maximises it.",
                "\"Alt + Left Click\" lets you drag a window from any position.",
                "\"Alt + Right Click\" lets you resize a window from any position.",
            }) {
                AddSmall(window, text, 500f, 20);
            }

            window.Add(new Area(0f, 25f));

            Area showArea = new Area();
            showArea.SetContentLayout(LayoutType.Horizontal);
            showArea.SetSize(400f, 40f);

            AddSmall(showArea, "Show this intro on startup", 200f, 18)
                .SetColor(theme.selectAltNormal);

            Toggle showOnStartup = new Toggle(Config.showIntro.Value);
            showOnStartup.SetSize(28f, 28f);
            showOnStartup.onValueChanged.AddListener((bool value) => {
                Config.showIntro.Value = value;
            });
            showArea.Add(showOnStartup);

            window.Add(showArea);

            window.Add(new Area(0f, 15f));

            Area links = new Area();
            links.SetContentLayout(LayoutType.Horizontal);
            links.SetElementSpacing(20);
            links.SetSize(400f, 40f);
            window.Add(links);

            UIButton developers = new UIButton("Developers",  16);
            developers.onClick.AddListener(() => {
                Application.OpenURL("https://kaden5480.github.io/docs/uilib/api/UILib.html");
            });
            developers.SetSize(80f, 25f);
            links.Add(developers);

            UIButton github = new UIButton("GitHub", 16);
            github.onClick.AddListener(() => {
                Application.OpenURL("https://github.com/Kaden5480/poy-ui-lib");
            });
            github.SetSize(80f, 25f);
            links.Add(github);
        }

        /**
         * <summary>
         * Shows the intro.
         * </summary>
         */
        internal void Show() {
            window.Show();
        }
    }
}
