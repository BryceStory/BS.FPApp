namespace FiiiPay.Framework
{
    /// <summary>
    /// Class Martin.Framework.Identity
    /// </summary>
    public sealed class Identity
    {
        private readonly IdWorker _idWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Identity"/> class.
        /// </summary>
        /// <param name="workId">The work identifier.</param>
        public Identity(long workId)
        {
            _idWorker = new IdWorker(workId);
        }

        /// <summary>
        /// Strings the identifier.
        /// </summary>
        /// <returns></returns>
        public string StringId()
        {
            return _idWorker.Id().ToString();
        }

        /// <summary>
        /// Longs the identifier.
        /// </summary>
        /// <returns></returns>
        public long LongId()
        {
            return _idWorker.Id();
        }
    }
}
