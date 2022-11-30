using System.Text.Json;

namespace SalesforceManagementLibrary {
    namespace Models {
        public class Account : BaseModelObject {
            public Account() : base() {

            }
            public Account(JsonElement elem) : base(elem) {}
        }
    }
}
