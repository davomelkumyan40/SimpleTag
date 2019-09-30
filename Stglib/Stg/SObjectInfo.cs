using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stglib.Stg
{
    [Serializable]
    public class SObjectInfo : SObject
    {
        public SObjectInfo()
        {
            Fields = new Dictionary<string, KeyValuePair<Type, object>>();
            Value = new List<object>();
        }

        protected SObjectInfo(object value)
        {
            this.Value.Add(value);
        }

        public List<object> Value { get; set; }

        public override string Name { get; set; }

        public override Dictionary<string, KeyValuePair<Type, object>> Fields { get; set; }


        public void AddMember(Type type, object value)
        {
            Value.Add(CreateInstance(type, value));
        }

        public override object Cast(Type type)
        {
            if (Fields.Count != 1 && type.BaseType == typeof(ValueType))
                throw new InvalidCastException($"Unable cast to {type.Name}");
            if (type.BaseType == typeof(Array))
            {
                object obj = Activator.CreateInstance(type, new object[] { 1 });
                obj = Fields.Values.FirstOrDefault().Value;
                return obj;
            }
            else if (type.BaseType == typeof(ValueType))
                return CreateInstance(type, Convert.ChangeType(Fields.Values.FirstOrDefault().Value, type));
            else if (type == typeof(string))
                return CreateInstance(type, Fields.Values.FirstOrDefault().Value);
            else // here is coming only if class is not from mscorelib
            {
                if (this.Name == type.Name)
                {
                    var instance = Activator.CreateInstance(type);
                    OnCast(instance);
                    return instance;
                }
                else
                    throw new InvalidCastException($"Unable cast to {type.Name}");
            }
        }

        public override T Cast<T>() => (T)Cast(typeof(T));

        private void OnCast(object instance)
        {
            SObject objTemp = this.DeepClone();
            Type t = instance.GetType();
            FieldInfo[] fieldInfos = t.GetFields();
            PropertyInfo[] propertyInfos = t.GetProperties();

            if (objTemp != null)
            {
                while (objTemp.Fields.Count != 0)
                {
                    FieldInfo field = fieldInfos.FirstOrDefault(f => objTemp.Fields.ContainsKey(f.Name));
                    PropertyInfo prop = propertyInfos.FirstOrDefault(f => objTemp.Fields.Keys.FirstOrDefault() == f.Name);
                    if (field == null && prop == null)
                        return;
                    if (field != null)
                    {
                        field.SetValue(instance, objTemp.Fields[field.Name].Value);
                        objTemp.Fields.Remove(field.Name);
                    }
                    if (prop != null)
                    {
                        prop.SetValue(instance, objTemp.Fields[prop.Name].Value);
                        objTemp.Fields.Remove(prop.Name);
                    }
                }
            }
        }

        private object CreateInstance(Type type, object value)
        {
            object t = null;
            if (type == typeof(string))
                t = Activator.CreateInstance(type, new object[] { value.ToString().ToCharArray() });
            else if (type.IsArray)
            {
                t = Activator.CreateInstance(type, new object[] { 1 });
                t = value;
            }
            else
            {
                t = Activator.CreateInstance(type, true);
                t = value;
            }
            return t;
        }
    }
}
