namespace enigma
{
    /// <summary>
    /// Defines Options for the Protocoll
    /// </summary>
    public interface IProtocoll
    {
        /// <summary>
        /// Set to false to disable the use of the bandwidth-reducing delta protocol.
        /// </summary>
        bool Delta { get; set; }
    }
}