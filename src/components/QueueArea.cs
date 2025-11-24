using System;

namespace UILib.Components {
    /**
     * <summary>
     * Like an Area, but also adds functionality to
     * remove the oldest children once a certain
     * limit is reached.
     * </summary>
     */
    public class QueueArea : Area {
        private int maxCount;

        /**
         * <summary>
         * Initializes a QueueArea.
         * </summary>
         * <param name="maxCount">The maximum number of children in the queue</param>
         */
        public QueueArea(int maxCount) {
            this.maxCount = Math.Max(0, maxCount);
        }

        /**
         * <summary>
         * Adds a child to this Area, but also removes
         * the oldest one if there are too many.
         * </summary>
         * <param name="child">The child to add</param>
         */
        public override void Add(UIComponent child) {
            base.Add(child);

            if (children.Count >= maxCount) {
                children[0].Destroy();
            }
        }
    }
}
