namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// Common error codes returned by IO functions.  Any return value less than zero is considered an error.
    /// </summary>
    public enum ErrorCodes :int
    {
        /// <summary>
        /// No such file or directory. The specified file or directory does not exist or cannot be found. This message can occur whenever a specified file does not exist or a component of a path does not specify an existing directory.
        /// </summary>
        ENOENT = -2,
        /// <summary>
        /// I/O error
        /// </summary>
        EIO = -5,
        /// <summary>
        /// Not enough memory
        /// </summary>
        ENOMEM = -12,
        /// <summary>
        /// File too large
        /// </summary>
        EFBIG = -27,
        /// <summary>
        /// The operation is invalid given the current state of the device.
        /// </summary>
        EINVAL = -22,
        /// <summary>
        /// The IO operation timed out.
        /// </summary>
        ETIMEDOUT = -116,
        /// <summary>
        /// The IO operation was successfully cancelled.
        /// </summary>
        EINTR = -4,
        /// <summary>
        /// An attempt was made to read/write to an endpoint that allready has a pending IO operation.
        /// </summary>
        EBUSY = -16,
        /// <summary>
        /// Bad address.
        /// </summary>
        EFAULT = -14,
        /// <summary>
        /// Attempted access on a disposed object.
        /// </summary>
        ENODEV = -19,
        /// <summary>
        /// An internal exception was generated. Use the <see cref="UsbGlobals.OnUsbError"/> event for more information.
        /// </summary>
        EEXCEPTION = -1073741826,
        /// <summary>
        /// A read thread was destructively aborted.
        /// </summary>
        ETHREADABORT = -1073741827,
    }
}