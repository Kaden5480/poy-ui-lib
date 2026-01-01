using BepInEx;
using UILib;

namespace UILibExamples {
    [BepInDependency("com.github.Kaden5480.poy-ui-lib")]
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib-examples", "UILib Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        private Examples examples;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            // You can add custom themes to UILib in Awake like so
            Theme custom = new Theme("Example Theme");

            // Change the background to something else
            custom.background = Colors.HSL(120f, 50f, 50f);

            // For register to work, the theme name must be unique
            Theme.Register(custom);

            // Building all the examples at once here isn't exactly optimal
            // It's generally a good idea to create the UIs as they're needed (where possible)
            //
            // For example, if you have a shortcut which opens a UI, make it on the
            // first time that shortcut is triggered.
            UIRoot.onInit.AddListener(() => {
                examples = new Examples();
            });
        }
    }
}
