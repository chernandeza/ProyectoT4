using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaT4
{
    public class ComentarioEventos : EventArgs
    {
        private List<Comentario> _comentarios;

        public List<Comentario> Comentarios
        {
            get { return _comentarios; }
            set { _comentarios = value; }
        }
    }
    public delegate void EventHandlerComentarios(object sender, ComentarioEventos e);
}
