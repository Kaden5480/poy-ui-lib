using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

using UILib.Events;

namespace UILib.Patches {
    /**
     * <summary>
     * The types of lock modes which are supported.
     * </summary>
     */
    [Flags]
    public enum LockMode {
        /**
         * <summary>
         * Don't lock anything.
         * </summary>
         */
        None = 0,

        /**
         * <summary>
         * Pauses the game.
         * </summary>
         */
        Pause = 1 << 0,

        /**
         * <summary>
         * Brings the cursor out of its normal locked state,
         * allowing the user to move it around freely (instead of
         * being fixed to the center of the screen).
         * </summary>
         */
        FreeCursor = 1 << 1,

        /**
         * <summary>
         * Locks navigation inputs, such as toggling
         * the `InGameMenu`.
         * </summary>
         */
        Navigation = 1 << 2,

        /**
         * <summary>
         * The default lock mode.
         * </summary>
         */
        Default = FreeCursor | Pause,
    }

    /**
     * <summary>
     * A class used for controlling certain aspects of the game.
     * Such as pausing, freeing the cursor, and preventing certain
     * navigation interactions.
     * </summary>
     */
    public class Lock {
        /**
         * <summary>
         * The current mode this lock is in.
         * </summary>
         */
        public LockMode mode { get; private set; } = LockMode.None;

        /**
         * <summary>
         * Initializes a lock, registering itself
         * with the <see cref="LockHandler"/>.
         *
         * The default `mode` is to pause the game and
         * free the cursor.
         * </summary>
         * <param name="mode">The mode this lock should be in</param>
         */
        public Lock(LockMode mode = LockMode.Default) {
            this.mode = mode;
            LockHandler.Register(this);
        }

        /**
         * <summary>
         * Sets the mode this lock is in.
         * </summary>
         * <param name="mode">The mode to use</param>
         */
        public void SetMode(LockMode mode) {
            this.mode = mode;
        }

        /**
         * <summary>
         * Adds a mode to this lock.
         *
         * This retains the current mode, but applies
         * the provided `mode` as an extra mode on top.
         * </summary>
         * <param name="mode">The mode to add</param>
         */
        public void AddMode(LockMode mode) {
            this.mode |= mode;
        }

        /**
         * <summary>
         * Removes a mode from this lock.
         *
         * This retains the current mode, but removes
         * the provided `mode` from it.
         * </summary>
         * <param name="mode">The mode to remove</param>
         */
        public void RemoveMode(LockMode mode) {
            this.mode &= (~mode);
        }

        /**
         * <summary>
         * Closes this lock, unregistering it
         * from the <see cref="LockHandler"/>.
         *
         * This *must* be called, it's the only way
         * for locks to be unregistered.
         * </summary>
         */
        public void Close() {
            LockHandler.Unregister(this);
        }
    }

    /**
     * <summary>
     * Processes the currently active <see cref="Lock">Locks</see> to
     * determine what should be locked/unlocked.
     * </summary>
     */
    public static class LockHandler {
        private static Logger logger = new Logger(typeof(LockHandler));

        // All registered locks
        private static List<Lock> locks = new List<Lock>();

        /**
         * <summary>
         * Invokes listeners when the lock handler pauses the game.
         * </summary>
         */
        public static UIEvent onPause { get; } = new UIEvent(
            typeof(LockHandler), nameof(onPause)
        );

        /**
         * <summary>
         * Invokes listeners when the lock handler unpauses the game.
         * </summary>
         */
        public static UIEvent onUnpause { get; } = new UIEvent(
            typeof(LockHandler), nameof(onUnpause)
        );

        /**
         * <summary>
         * Invokes listeners when the lock handler frees the cursor,
         * allowing the user to move it around.
         * </summary>
         */
        public static UIEvent onCursorFree { get; } = new UIEvent(
            typeof(LockHandler), nameof(onCursorFree)
        );

        /**
         * <summary>
         * Invokes listeners when the lock handler is no longer
         * freeing the cursor.
         * </summary>
         */
        public static UIEvent onCursorLock { get; } = new UIEvent(
            typeof(LockHandler), nameof(onCursorLock)
        );

        /**
         * <summary>
         * Invokes listeners when the lock handler is locking navigation inputs.
         * </summary>
         */
        public static UIEvent onNavigationLock { get; } = new UIEvent(
            typeof(LockHandler), nameof(onNavigationLock)
        );

        /**
         * <summary>
         * Invokes listeners when the lock handler is no longer locking navigation inputs.
         * </summary>
         */
        public static UIEvent onNavigationUnlock { get; } = new UIEvent(
            typeof(LockHandler), nameof(onNavigationUnlock)
        );

        /**
         * <summary>
         * Initializes the lock handler's internal listeners
         * for managing different lock states.
         * </summary>
         */
        internal static void Init() {
            onUnpause.AddListener(() => {
                if (IsInMenu() == true) {
                    return;
                }

                AllowMovement(true);
                InGameMenu.isCurrentlyNavigationMenu = false;
            });

            onCursorLock.AddListener(() => {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            });
        }

#region Lock Management

        /**
         * <summary>
         * Whether the lock handler is currently pausing the game.
         * </summary>
         */
        public static bool isPaused { get; private set; } = false;

        /**
         * <summary>
         * Whether the lock handler is currently freeing
         * the cursor.
         * </summary>
         */
        public static bool isCursorFree { get; private set; } = false;

        /**
         * <summary>
         * Whether the lock handler is currently locking
         * navigation inputs.
         * </summary>
         */
        public static bool isNavigationLocked { get; private set; } = false;

        /**
         * <summary>
         * Invokes events depending on a comparison
         * between an old and new value.
         *
         * If they are the same, nothing happens.
         * If newValue is true (and oldValue is false), onTrueEvent is invoked.
         * If newValue is false (and oldValue is true), onFalseEvent is invoked.
         * </summary>
         * <param name="oldValue">The old value</param>
         * <param name="newValue">The new value</param>
         * <param name="onTrueEvent">The event to run when newValue is true</param>
         * <param name="onFalseEvent">The event to run when newValue is false</param>
         */
        private static void InvokeEvents(
            bool oldValue, bool newValue,
            UIEvent onTrueEvent, UIEvent onFalseEvent
        ) {
            if (oldValue == newValue) {
                return;
            }

            if (newValue == true) {
                onTrueEvent.Invoke();
                return;
            }

            onFalseEvent.Invoke();
        }

        /**
         * <summary>
         * Checks the currently registered locks to determine
         * what should be locked.
         * </summary>
         */
        private static void UpdateLocks() {
            bool isPaused = false;
            bool isCursorFree = false;
            bool isNavigationLocked = false;

            // Figure out what needs changing
            foreach (Lock @lock in locks) {
                if (@lock.mode.HasFlag(LockMode.Pause) == true) {
                    isPaused = true;
                }

                if (@lock.mode.HasFlag(LockMode.FreeCursor) == true) {
                    isCursorFree = true;
                }

                if (@lock.mode.HasFlag(LockMode.Navigation) == true) {
                    isNavigationLocked = true;
                }
            }

            // In these loading states, the game must be paused
            if (InGameMenu.isLoading == true
                || EnterPeakScene.enteringPeakScene == true
                || EnterPeakScene.enteringAlpScene == true
                || EnterRoomSegmentScene.enteringScene == true
            ) {
                isPaused = true;
            }

            // Invoke listeners
            InvokeEvents(
                LockHandler.isPaused, isPaused,
                onPause, onUnpause
            );

            InvokeEvents(
                LockHandler.isCursorFree, isCursorFree,
                onCursorFree, onCursorLock
            );

            InvokeEvents(
                LockHandler.isNavigationLocked, isNavigationLocked,
                onNavigationLock, onNavigationUnlock
            );

            // Update
            LockHandler.isPaused = isPaused;
            LockHandler.isCursorFree = isCursorFree;
            LockHandler.isNavigationLocked = isNavigationLocked;
        }

        /**
         * <summary>
         * Registers a lock.
         * </summary>
         * <param name="@lock">The lock to register</param>
         */
        internal static void Register(Lock @lock) {
            if (locks.Contains(@lock) == true) {
                logger.LogError($"Trying to register the same lock: {@lock}");
                return;
            }

            locks.Add(@lock);
        }

        /**
         * <summary>
         * Unregisters a lock.
         * </summary>
         * <param name="@lock">The lock to unregister</param>
         */
        internal static void Unregister(Lock @lock) {
            locks.Remove(@lock);
        }

#endregion

#region Pausing

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
            // Can't do anything in the credits
            if (Cache.inGameMenu != null && Cache.inGameMenu.isCredits == true) {
                return;
            }

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
            // Update the lock state
            UpdateLocks();

            // Disabling movement has to be done every frame, yes
            if (isPaused == true) {
                AllowMovement(false);
                InGameMenu.isCurrentlyNavigationMenu = true;
                InGameMenu.hasBeenInMenu = true;
                climbingDelay.SetValue(Cache.inGameMenu, 0f);
                PlayerVelocity.Pause();
            }

            // Same with freeing the cursor
            if (isCursorFree == true) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

#endregion

    }
}
