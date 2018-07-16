namespace enigma
{
    public interface IProtocoll
    {
        /// <summary>
        /// Set to false to disable the use of the bandwidth-reducing delta protocol.
        /// </summary>
        bool Delta { get; set; }
    }
}