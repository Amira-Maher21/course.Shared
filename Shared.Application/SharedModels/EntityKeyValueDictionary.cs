namespace  Shared.Application.SharedModels
{
    public class EntityKeyValueDictionary : List<KeyValuePair<string, object>>
    {
        public string[] Keys
        {
            get { return this.Select(p => p.Key).ToArray(); }
        }

        public object[] Values
        {
            get { return this.Select(p => p.Value).ToArray(); }
        }

    }
}
