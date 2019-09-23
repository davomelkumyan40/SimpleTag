using System;
using System.Collections.Generic;
using System.Linq;

namespace Stglib.Stg
{
    [Serializable]
    public abstract class SObject
    {
        public abstract string Name { get; set; }

        public abstract Dictionary<string, KeyValuePair<Type, object>> Fields { get; set; }

        public abstract T Cast<T>();

        public abstract object Cast(Type type);

        public override string ToString()
        {
            return $"Object : {Name} = {Fields[Name].Value}";
        }
    }
}
