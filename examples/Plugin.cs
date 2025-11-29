using BepInEx;

namespace UILibExamples {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib-examples", "UI Lib Examples", "0.1.0")]
    internal class Plugin : BaseUnityPlugin {
        private BasicWindow basic;
        private PauseHandles pauseHandles;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            // Basic window example
            basic = new BasicWindow();

            // Example explaining pause handles
            pauseHandles = new PauseHandles();
        }

        /**
         * <summary>
         * Executes when the plugin has started.
         * </summary>
         */
        private void Start() {
            basic.Show();
            pauseHandles.Show();
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        private void Update() {
            pauseHandles.Update();
        }
    }
}
