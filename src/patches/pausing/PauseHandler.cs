using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace UILib.Patches {
    /**
     * <summary>
     * A pause handle.
     *
     * You shouldn't usually need to construct your own `PauseHandles`
     * as <see cref="Overlay">Overlays</see> already create them
     * internally.
     *
     * Whenever a `PauseHandle` is constructed it automatically
     * pauses the game.
     *
     * Closing a `PauseHandle` will let the game unpause,
     * but only once all `PauseHandles` have been closed.
     * There could even be many `PauseHandles` across many mods,
     * but as long as at least one exists
     * (and hasn't closed yet), the game will be paused.
     * </summary>
     */
    public class PauseHandle {
        /**
         * <summary>
         * Initializes a pause handle, telling the <see cref="PauseHandler"/>
         * that the game should pause.
         * </summary>
         */
        public PauseHandle() {
            PauseHandler.Add(this);
        }

        /**
         * <summary>
         * Closes this pause handle,
         * telling the <see cref="PauseHandler"/> that the game
         * no longer needs to be paused.
         * </summary>
         */
        public void Close() {
            PauseHandler.Remove(this);
        }
    }

    /**
     * <summary>
     * A class which helps deal with
     * pausing/resuming in Peaks of Yore.
     *
     * This also takes away focus from the game
     * allowing users to interact with UI when they otherwise
     * wouldn't be able to.
     *
     * This tries to handle different cases
     * where the game is paused, along with pausing
     * which has been caused by UIs in UILib. So
     * it could, in most cases, be something
     * you can rely on to determine whether the
     * user is in a UI.
     *
     * If it doesn't handle something, please let me
     * know.
     * </summary>
     */
    public static class PauseHandler {
        private static Logger logger = new Logger(typeof(PauseHandler));

        // Currently active handles
        private static List<PauseHandle> handles = new List<PauseHandle>();

        /**
         * <summary>
         * Invokes listeners when the game is paused.
         * </summary>
         */
        public static UnityEvent onPause { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when the game is unpaused.
         * </summary>
         */
        public static UnityEvent onUnpause { get; } = new UnityEvent();

        /**
         * <summary>
         * Indicates whether the game is currently paused
         * by the `PauseHandler`.
         * </summary>
         */
        public static bool isPaused { get => handles.Count > 0; }

        /**
         * <summary>
         * Adds a PauseHandle to pause the game.
         * </summary>
         * <param name="handle">The handle to add</param>
         */
        internal static void Add(PauseHandle handle) {
            handles.Add(handle);
        }

        /**
         * <summary>
         * Removes a PauseHandle.
         * </summary>
         * <param name="handle">The handle to remove</param>
         */
        internal static void Remove(PauseHandle handle) {
            handles.Remove(handle);
        }

#region Peaks Specific Stuff

        private static bool allowingMovement = true;
        private static bool triggeredPause = false;

        private static FieldInfo climbingDelay = AccessTools.Field(
            typeof(InGameMenu), "climbingDelay"
        );

        /**
         * <summary>
         * Sets whether the player is allowed to move.
         * </summary>
         * <param name="allow">Whether the player should be able to move</param>
         */
        private static void AllowMovement(bool allow) {
            allowingMovement = allow;

            if (Cache.playerManager != null) {
                Cache.playerManager.AllowPlayerControl(allow);
            }

            if (Cache.peakSummited != null) {
                Cache.peakSummited.DisableEverythingButClimbing(!allow);
            }
        }

        /**
         * <summary>
         * Checks whether the player is currently in a menu.
         * </summary>
         * <returns>True if they are, false otherwise</returns>
         */
        private static bool IsInMenu() {
            if (Cache.inGameMenu == null) {
                logger.LogDebug("InGameMenu is null for some reason");
                return false;
            }

            return Cache.inGameMenu.isMainMenu == true
                || Cache.inGameMenu.inMenu == true;
        }

        /**
         * <summary>
         * The method to run every frame to deal
         * with the spaghetti.
         * </summary>
         */
        internal static void Update() {
            // Do nothing in certain loading states
            if (InGameMenu.isLoading == true
                || EnterPeakScene.enteringPeakScene == true
                || EnterPeakScene.enteringAlpScene == true
                || EnterRoomSegmentScene.enteringScene == true
            ) {
                return;
            }

            // Disabling movement has to be done every frame, yes
            if (isPaused == true) {
                // Invoke listeners only on the first
                // time this runs
                if (triggeredPause == false) {
                    triggeredPause = true;
                    onPause.Invoke();
                }

                AllowMovement(false);
                InGameMenu.isCurrentlyNavigationMenu = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InGameMenu.hasBeenInMenu = true;
                climbingDelay.SetValue(Cache.inGameMenu, 0f);
            }

            // Otherwise, only disable pausing once
            else if (isPaused == false
                && allowingMovement == false
                && IsInMenu() == false
            ) {
                AllowMovement(true);
                InGameMenu.isCurrentlyNavigationMenu = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                // Reset state and invoke listeners
                triggeredPause = false;
                onUnpause.Invoke();
            }
        }

#endregion

    }
}
