namespace FiiiPay.Foundation.Entities
{
    public class MasterSetting
    {
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets a int value for the Id column.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a string value for the Name column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a string value for the Type column.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a string value for the Value column.
        /// </summary>
        public string Value { get; set; }

    }
}
