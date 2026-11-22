namespace WebApi.Security
{

    // Specifies that this attribute can be applied to classes or methods, only once per target, and is inherited by derived classes
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class PermissionAttribute : Attribute
    {
        // Initializes the attribute with the required permission code (to pass it)
        public PermissionAttribute(string code)
        {
            Code = code;
        }

        // The permission code required to access the target resource (e.g., "UsersView")
        public string Code { get; }
    }
}
