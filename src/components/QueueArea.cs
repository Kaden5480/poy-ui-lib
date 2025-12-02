using System;

namespace UILib.Components {
    /**
     * <summary>
     * Like an <see cref="Area"/>, but also adds functionality to
     * remove the oldest children once a certain
     * limit is reached.
     * </summary>
     */
    public class QueueArea : Area {
        private int limit;

        /**
         * <summary>
         * Initializes a queue area.
         * </summary>
         * <param name="limit">The maximum number of children in the queue</param>
         */
        public QueueArea(int limit) {
            SetLimit(limit);
        }

        /**
         * <summary>
         * Checks the current child count, removing
         * children where they exceed the set `limit`.
         *
         * The oldest child will be removed first.
         * </summary>
         */
        private void Prune() {
            while (children.Count > limit && children.Count > 0) {
                children[0].Destroy();
            }
        }

        /**
         * <summary>
         * Sets this queue area to use a new `limit`.
         *
         * If the number of children exceeds the newly set limit,
         * they will be pruned (oldest removed first).
         * </summary>
         * <param name="limit">The new limit to use</param>
         */
        public void SetLimit(int limit) {
            this.limit = Math.Max(0, limit);
            Prune();
        }

        /**
         * <summary>
         * Adds a child to this area.
         *
         * If the number of children exceeds the set `limit`,
         * they will be pruned (oldest removed first).
         *
         * See <see cref="UIObject.Add(UIComponent, bool)"/> as this
         * would probably be the method you'd want to call instead.
         * </summary>
         * <param name="child">The child to add</param>
         * <param name="setTheme">Whether the child should inherit this object's theme</param>
         */
        public override void AddContent(UIComponent child, bool setTheme = true) {
            base.AddContent(child, setTheme);
            Prune();
        }
    }
}
