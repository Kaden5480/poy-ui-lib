using System.Collections.Generic;

using UnityEngine.Events;

namespace UILib.Patches {
    /**
     * <summary>
     * An input lock.
     *
     * You shouldn't usually need to construct your own `InputLocks`,
     * as <see cref="Overlay">Overlays</see> have their own way
     * of <see cref="Overlay.SetInputLock">automatically managing them</see>.
     *
     * Whenever an `InputLock` is constructed, it automatically
     * prevents the use of some extra vanilla keybinds (such as the `InGameMenu`).
     * </summary>
     */
    public class InputLock {
        /**
         * <summary>
         * Initializes an input lock, telling the <see cref="InputHandler"/>
         * that the input should be locked.
         * </summary>
         */
        public InputLock() {
            InputHandler.Add(this);
        }

        /**
         * <summary>
         * Closes this input lock, telling the <see cref="InputHandler"/>
         * the input no longer needs locking.
         * </summary>
         */
        public void Close() {
            InputHandler.Remove(this);
        }
    }

    /**
     * <summary>
     * A class which helps deal with locking extra vanilla inputs.
     * </summary>
     */
    public static class InputHandler {
        private static Logger logger = new Logger(typeof(InputHandler));

        // Currently active locks
        private static List<InputLock> locks = new List<InputLock>();

        /**
         * <summary>
         * Invokes listeners when the input is locked.
         * </summary>
         */
        public static UnityEvent onLock { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when the input is unlocked.
         * </summary>
         */
        public static UnityEvent onUnlock { get; } = new UnityEvent();

        /**
         * <summary>
         * Indicates whether the `InputHandler` is currently locking extra vanilla inputs.
         * </summary>
         */
        public static bool isLocked { get => locks.Count > 0; }

        /**
         * <summary>
         * Adds a new input lock.
         * </summary>
         * <param name="inputLock">The input lock to add</param>
         */
        internal static void Add(InputLock inputLock) {
            if (inputLock == null) {
                return;
            }

            if (isLocked == false) {
                locks.Add(inputLock);
                onLock.Invoke();
            }
            else {
                locks.Add(inputLock);
            }
        }

        /**
         * <summary>
         * Removes an input lock.
         * </summary>
         * <param name="inputLock">The lock to remove</param>
         */
        internal static void Remove(InputLock inputLock) {
            if (inputLock == null || locks.Count < 1) {
                return;
            }

            if (locks.Count == 1) {
                locks.Remove(inputLock);
                onUnlock.Invoke();
            }
            else {
                locks.Remove(inputLock);
            }
        }
    }
}
