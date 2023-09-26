using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arriel.chat.infrastructure.Delegates
{
    public class StatusChangedEventArgs : EventArgs
    {        
        public string EventMessage { get; set; }        

        public StatusChangedEventArgs(string eventMsg)
        {
            EventMessage = eventMsg;
        }
    }
}
