namespace CloudyWing.MoneyTrack.Infrastructure.Filters {
    /// <summary>
    /// Specifies the property to be validated for a method.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ValidationPropertyAttribute"/> class with the specified property name.
    /// </remarks>
    /// <param name="propertyName">The name of the property to be validated.</param>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ValidationPropertyAttribute(string propertyName) : Attribute {

        /// <summary>
        /// Gets or sets the name of the property to be validated.
        /// </summary>
        public string PropertyName { get; set; } = propertyName;
    }
}
