using System;

namespace UILib {
    /**
     * <summary>
     * A class for helping with creating
     * more consistent and informational logging.
     * </summary>
     */
    internal class Logger {
        private string name;

        /**
         * <summary>
         * Initializes a Logger.
         * </summary>
         * <param name="type">The type this logger is for</param>
         */
        internal Logger(Type type) {
            name = $"{type}";
        }

        /**
         * <summary>
         * Initializes a Logger with a custom name.
         * </summary>
         * <param name="name">The name to assign to this logger</param>
         */
        internal Logger(string name) {
            this.name = name;
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal void LogDebug(string message) {
            Plugin.LogDebug($"[{name}] {message}");
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal void LogInfo(string message) {
            Plugin.LogInfo($"[{name}] {message}");
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal void LogError(string message) {
            Plugin.LogError($"[{name}] {message}");
        }
    }
}
