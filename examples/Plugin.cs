using BepInEx;
using UILib;

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
            UIRoot.onInit.AddListener(() => {
                examples = new Examples();
            });
        }
    }
}
