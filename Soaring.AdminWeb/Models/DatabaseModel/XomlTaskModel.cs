using System;

namespace Soaring.AdminWeb.Models.DatabaseModel {
    public class XomlTaskModel : MongoModel {
        public string XomlTaskName { get; set; }
        public string XomlTaskId { get; set; }
        public string XomlTaskPath { get; set; }
        public DateTime CreateTime { get; set; }
    }
}