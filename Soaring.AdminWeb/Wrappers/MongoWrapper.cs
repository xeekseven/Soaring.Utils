using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Soaring.AdminWeb.Wrappers{
    public class MongoWrapper
    {
        private IMongoDatabase _database = null;
        private string _connectionString = "mongodb://127.0.0.1:27017";
        public MongoWrapper(string connection,string dbName)
        {
            this._connectionString = connection;
            var client = new MongoClient(this._connectionString);
            this._database = client.GetDatabase(dbName);
        }
        public async Task<bool> InsertOne<T>(string collectName,T bson)
        {
            var collect = this._database.GetCollection<T>(collectName);
            await collect.InsertOneAsync(bson);
            return true;
        }
        public async Task<IList<T>> GetAll<T>(string collectName){
            var collect = this._database.GetCollection<T>(collectName);
            var list =await collect.Find(new BsonDocument()).ToListAsync();
            return list;
        }
        public async Task<IList<T>> Query<T>(string collectName,string key,object value){
            var collect = this._database.GetCollection<T>(collectName);
            var filter = Builders<T>.Filter.Eq(key, value);
            var list = await collect.Find(filter).ToListAsync();
            return list;
        }
        public async Task<bool> Delete<T>(string collectName,string key,object value){
            var collect = this._database.GetCollection<T>(collectName);
            var filter = Builders<T>.Filter.Eq(key, value);
            await collect.DeleteOneAsync(filter);
            return true;
        }
        // public async Task<IList<BsonDocument>> QueryLike(string collectName,string key,object value){
        //     var collect = this._database.GetCollection<BsonDocument>(collectName);
        //     var query = new QueryBuilder<BsonDocument>();
        // }
    }
}