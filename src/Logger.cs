using System;

namespace UILib {
    /**
     * <summary>
     * A class for helping with creating
     * more consistent and informational logging.
     * </summary>
     */
    internal class Logger {
        private Type type;

        internal Logger(Type type) {
            this.type = type;
        }

        internal virtual void LogDebug(string message) {
            Plugin.LogDebug($"[{type}] {message}");
        }

        internal virtual void LogInfo(string message) {
            Plugin.LogInfo($"[{type}] {message}");
        }

        internal virtual void LogError(string message) {
            Plugin.LogError($"[{type}] {message}");
        }
    }
}
