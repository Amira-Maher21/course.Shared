namespace Shared.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SecurableColumnAttribute : Attribute
    {
        public SecurableColumnAttribute(string accessControlColumnName)
        {
            AccessControlColumnName = accessControlColumnName;
        }

        public string AccessControlColumnName { get; }
    }
}
