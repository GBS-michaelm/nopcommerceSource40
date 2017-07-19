namespace Nop.Plugin.Payments.GBS
{
    /// <summary>
    /// Represents GBS payment processor transaction mode
    /// </summary>
    public enum TransactMode
    {
        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 1,

        /// <summary>
        /// Authorize and capture
        /// </summary>
        AuthorizeAndCapture = 2
    }
}
