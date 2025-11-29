using BepInEx;

namespace UILibExamples {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib-examples", "UI Lib Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        BasicWindow basic;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            basic = new BasicWindow();
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            basic.Show();
        }
    }
}
