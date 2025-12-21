using BepInEx;
using UILib;

namespace UILibExamples {
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

            UIRoot.onInit.AddListener(() => {
                examples = new Examples();
            });
        }
    }
}
