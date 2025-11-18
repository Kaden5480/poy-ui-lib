using System;

namespace UILib {
    [Flags]
    public enum ScrollType {
        None       = 0,
        Vertical   = 1 << 1,
        Horizontal = 1 << 2,
    }
}
