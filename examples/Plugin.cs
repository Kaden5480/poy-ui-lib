using BepInEx;

namespace UILibExamples {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib-examples", "UI Lib Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        private Examples examples;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            examples = new Examples();
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            examples.Show();
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        private void Update() {
            examples.Update();
        }
    }
}
