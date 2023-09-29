using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.server.Classes
{
    public class SenderInfo
    {
        public KeyValuePair<string, StreamWriter> UserInfo { get; set; }
        public KeyValuePair<StreamWriter, string> ConnectionInfo { get; set; }

        public string GetUserName() 
        { 
            if (UserInfo.Equals(default(KeyValuePair<string, StreamWriter>)))
            {
                return "";
            }
            return UserInfo.Key;
        }  
    }
}
