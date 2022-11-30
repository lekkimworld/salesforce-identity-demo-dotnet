using System.Text.Json;

namespace SalesforceManagementLibrary {
    namespace Models {
        public class User : BaseModelObject {
            public User(JsonElement elem) : base(elem) {}
        }
    }
}
