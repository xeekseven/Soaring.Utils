using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Soaring.AdminWeb.Models
{
    public class XomlTaskViewModel
    {
        public string TaskName { get; set; }
        public List<IFormFile> TaskFiles { get; set; }
    }
}