using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Background : UIObject {
        public UEImage image { get; private set; }

        public Background() {
            image = gameObject.AddComponent<UEImage>();
            SetAnchor(AnchorType.Middle, FillType.Fill);
        }

        public Background(Color color) : this() {
            image.color = color;
        }
    }
}
