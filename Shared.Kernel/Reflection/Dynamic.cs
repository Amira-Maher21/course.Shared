namespace Shared.Kernel.Reflection
{
    public class Dynamic
    {
        private Dictionary<string, DynamicProperty> _properties;

        private Dynamic(object obj)
        {
            _properties = new Dictionary<string, DynamicProperty>();

            var objectType = obj.GetType();
            var originalPropertiesList = objectType.GetProperties();


            foreach (var p in originalPropertiesList)
            {
                DynamicProperty property = new DynamicProperty
                {
                    PropertyName = p.Name,
                    PropertyType = p.GetType(),
                    Value = p.GetValue(p)
                };
                _properties.Add(property.PropertyName, property);
            }

        }

        public DynamicProperty this[string key]
        {
            get
            {
                if (_properties.ContainsKey(key))
                    return _properties[key];
                throw new Exception($"Property {key} ");
            }
        }

        public static Dynamic AsDynamic(object obj)
        {
            return new Dynamic(obj);
        }
    }

    public class DynamicProperty
    {
        public string? PropertyName { get; set; }

        public Type? PropertyType { get; set; }

        public object? Value { get; set; }
    }
}
