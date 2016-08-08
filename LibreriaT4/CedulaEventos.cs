using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaT4
{
    public class CedulaEventos : EventArgs
    {
        private List<String> _cedulas;

        public List<String> Cedulas
        {
            get { return _cedulas; }
            set { _cedulas = value; }
        }
    }
    public delegate void EventHandlerCedulas(object sender, CedulaEventos e);
}
