using System;

namespace Soaring.AdminWeb.Models.DatabaseModel {
    public class TaskDataModel : MongoModel {
        public string TaskId { get; set; }
        public string TaskData { get; set; }
    }
}