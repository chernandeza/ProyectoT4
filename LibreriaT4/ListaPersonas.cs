using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaT4
{
    public class ListaPersonas
    {
        private Dictionary<String, Persona> _listaP;

        public Dictionary<String, Persona> ListaP
        {
            get { return _listaP; }
            set { _listaP = value; }
        }

        public ListaPersonas()
        {
            this._listaP = new Dictionary<String, Persona>();
        }

        public bool AgregarPersonaALista(Persona P)
        {
            try
            {
                ListaP.Add(P.Cedula, P);
                return true;
            }
            catch (Exception)
            {
                //Escribir en bitacora el error
                return false;
            }
        }
    }
}
