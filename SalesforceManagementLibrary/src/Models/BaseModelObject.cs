using System.Text.Json;
using System.Dynamic;

namespace SalesforceManagementLibrary {
    namespace Models {
        public abstract class BaseModelObject : System.Dynamic.DynamicObject {
            // declarations
            protected JsonElement elem;
            private Dictionary<string,object>? values;
            protected bool Dirty {
                get => null != this.values;
            }
            public string Id { get => (string)this.getValue("Id")!; }
            public string Name { 
                get => (string)this.getValue("Name")!; 
                set => this.SetValue("Name", value);
            }
            internal Dictionary<string,object>? Values { get => this.values; }

            protected BaseModelObject() {}
            protected BaseModelObject(JsonElement elem)
            {
                this.elem = elem;
            }

            private object? getValue(string name) {
                try
                {
                    var prop = this.elem.GetProperty(name);
                    switch (prop.ValueKind)
                    {
                        case JsonValueKind.Null:
                            return null;
                        case JsonValueKind.Number:
                            return prop.GetInt32();
                        case JsonValueKind.String:
                            return prop.GetString();
                        case JsonValueKind.False:
                            return false;
                        case JsonValueKind.True:
                            return true;
                        default:
                            return null;
                    }
                } catch (Exception) {}
                return null;
            }

            private void SetValue(string name, object? v) {
                if (null == this.values) this.values = new Dictionary<string, object>();
                if (null == v)
                {
                    this.values.Remove(name.ToLower());
                }
                else
                {
                    this.values[name.ToLower()] = v;
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object? result)
            {
                string name = binder.Name;
                result = this.getValue(name);
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object? value)
            {
                this.SetValue(binder.Name, value);
                 return true;
            }
        }
    }
}
