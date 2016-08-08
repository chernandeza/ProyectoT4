using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaT4
{
    public class ModifiedException : Exception
    {
        private String _customMsg;

        public String CustomMessage
        {
            get { return _customMsg; }
            set { _customMsg = value; }
        }

        public ModifiedException(String msg)
        {
            this.CustomMessage = msg + " " + base.Message;
        }
    }
}
