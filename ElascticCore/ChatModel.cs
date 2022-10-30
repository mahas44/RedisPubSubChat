using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElascticCore
{
    public class ChatModel : BaseModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
    }
}
