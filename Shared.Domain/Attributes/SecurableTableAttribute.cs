namespace  Shared.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SecurableTableAttribute : Attribute
    {
        public SecurableTableAttribute(Type accessControlTableName)
        {
            AccessControlTableName = accessControlTableName;
        }
        public Type AccessControlTableName { get; }
    }
}
