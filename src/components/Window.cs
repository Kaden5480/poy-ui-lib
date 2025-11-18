using UnityEngine;

namespace UILib {
    public class Window : BaseComponent {
        private Canvas canvas;

        public Window(float width, float height) {
            canvas = new Canvas();
            canvas.AddChild(this);

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(width, height);

            root.AddComponent<UnityEngine.UI.Image>();
        }

        public override void DontDestroyOnLoad() {
            canvas.DontDestroyOnLoad();
        }
    }
}
