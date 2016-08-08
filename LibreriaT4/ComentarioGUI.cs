using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaT4
{
    public class ComentarioGUI : EventArgs
    {
        private Comentario _comentario;

        public Comentario Coment
        {
            get { return _comentario; }
            set { _comentario = value; }
        }
    }
    public delegate void EventHandlerComentarioGUI(object sender, ComentarioGUI e);
}
