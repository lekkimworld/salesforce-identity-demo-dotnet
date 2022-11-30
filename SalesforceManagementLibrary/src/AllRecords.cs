using System.Text.Json;
using System.Collections;

namespace SalesforceManagementLibrary {
    namespace Models {
        public class AllRecords<T> : IEnumerable<T>  where T: BaseModelObject {
            public static AllRecords<T> FromJson(JsonDocument json)
            {
                return new AllRecords<T>(json);
            }
            public static AllRecords<T> FromJson(string json)
            {
                return FromJson(JsonDocument.Parse(json));
            }

            // declarations
            public string? nextRecordsUrl { get; }
            public int Count { get; }
            private IEnumerable<T> records;

            AllRecords(JsonDocument doc) {
                this.Count = doc.RootElement.GetProperty("totalSize").GetInt32();
                try {
                    this.nextRecordsUrl = doc.RootElement.GetProperty("nextRecordsUrl").GetString();
                } catch (Exception) {}
                this.records = doc.RootElement.GetProperty("records").EnumerateArray().Select((elem) => {
                    return (T)Activator.CreateInstance(typeof(T), new object[]{elem})!;
                })!;
            }

            public async Task<AllRecords<T>?> GetNextRecords(SalesforceAPIClient client) {
                if (null == this.nextRecordsUrl) return null;
                var doc = await client.GetAsync($"{client.InstanceUrl}{this.nextRecordsUrl}");
                return AllRecords<T>.FromJson(doc);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.records.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
