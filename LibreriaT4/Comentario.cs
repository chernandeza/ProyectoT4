using System;
using System.ComponentModel;

namespace LibreriaT4
{
    [Serializable]
    public class Comentario
    {
        private int _id;
        [DisplayName("Identificador")]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private String _cedula;
        [DisplayName("Cedula")]
        public String Cedula
        {
            get { return _cedula; }
            set { _cedula = value; }
        }

        private String _comentario;
        [DisplayName("Comentario")]
        public String Texto
        {
            get { return _comentario; }
            set { _comentario = value; }
        }
    }
}
