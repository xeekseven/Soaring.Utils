using System.Collections.Generic;
using System.Threading.Tasks;
using Soaring.AdminWeb.Models.DatabaseModel;
using Soaring.AdminWeb.Wrappers;
using MongoDB.Driver;

namespace Soaring.AdminWeb.Services {
    public class XomlTaskService {
        private MongoWrapper _mongo;
        private string _collectionName = "TaskInfo";
        public XomlTaskService (MongoWrapper mongo) {
            this._mongo = mongo;
        }
        public async Task<bool> InsertOne (XomlTaskModel bson) {
            await this._mongo.InsertOne<XomlTaskModel> (this._collectionName, bson);
            return true;
        }
        public async Task<IList<XomlTaskModel>> GetAll<XomlTaskModel> () {
            return await this._mongo.GetAll<XomlTaskModel> (this._collectionName);
        }
        public async Task<IList<XomlTaskModel>> QueryByTaskId<XomlTaskModel> (string taskId) {
            return await this._mongo.Query<XomlTaskModel> (this._collectionName, "XomlTaskId", taskId);
        }
        public async Task<bool> Delete(string taskId){
            return await this._mongo.Delete<XomlTaskModel>(this._collectionName,"XomlTaskId",taskId);
        }
    }
}