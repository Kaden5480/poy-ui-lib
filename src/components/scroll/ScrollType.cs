using System;

namespace UILib.Components {
    /**
     * <summary>
     * Scroll types which can be applied to ScrollBars.
     * </summary>
     */
    [Flags]
    public enum ScrollType {
        None        = 1 << 0,
        Vertical    = 1 << 1,
        Horizontal  = 1 << 2,
    }
}
