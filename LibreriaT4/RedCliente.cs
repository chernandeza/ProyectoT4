using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;


namespace LibreriaT4
{
    /// <summary>
    /// Esta clase administra la conexión de cada uno de los clientes hacia el servidor.
    /// En cada interfaz GUI de un cliente, debe instanciarse un objeto de esta clase.
    /// </summary>
    public class RedCliente
    {
        public String Server { get { return "localhost"; } }  // Address of server. In this case - local IP address.
        public int Port { get { return 10830; } }
        //private Task tcpTask;                           //Subproceso para transmitir/recibir
        private bool _isConnected;                      //Indica si el cliente está o no conectado
        private TcpClient commChannel;                  // Canal de comunicación hacia el servidor
        private NetworkStream netStream;                // Stream del canal de respuesta para enviar datos hacia el servidor.
        private BinaryReader netDataReader;             // Utilizado para leer datos del canal de comunicación
        private BinaryWriter netDataWriter;             // Utilizado para escribir datos en el canal de comunicación
        private EvtLogWriter evtLW;

        // Events
        public event EventHandler Connected; //Evento lanzado al conectarse al servidor
        public event EventHandler ComentarioEnviado; //Evento lanzado al enviar un mensaje
        public event EventHandler Disconnected; //Evento lanzado al desconectarse del servidor
        public event EventHandler ServerError; //Evento lanzado al obtener un mensaje de error desde el servidor
        public event EventHandler SinComentarios; //Evento lanzado al no obtener mensajes de respuesta por tipo de mensaje
        public event EventHandlerComentarios ComentariosRecibidos; //Evento lanzado al recibir lista de comentarios
        public event EventHandlerCedulas ListaCedulas; //Evento lanzado al recibir lista de comentarios

        /*Estos métodos validan que la suscripción a los eventos no esté vacía. Si está vacía, no lanza el evento de forma innecesaria*/
        virtual protected void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        virtual protected void OnServerError()
        {
            if (ServerError != null)
                ServerError(this, EventArgs.Empty);
        }

        virtual protected void OnConnected()
        {
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        virtual protected void OnSinComentarios()
        {
            if (SinComentarios != null)
                SinComentarios(this, EventArgs.Empty);
        }

        virtual protected void OnComentarioEnviado()
        {
            if (ComentarioEnviado != null)
                ComentarioEnviado(this, EventArgs.Empty);
        }

        virtual protected void OnComentariosRecibidos(ComentarioEventos ce)
        {
            if (ComentariosRecibidos != null)
                ComentariosRecibidos(this, ce);
        }

        virtual protected void OnListaCedulas(CedulaEventos ce)
        {
            if (ListaCedulas != null)
                ListaCedulas(this, ce);
        }

        public RedCliente()
        {
            evtLW = new EvtLogWriter("RedCliente", "Application");
            _isConnected = false;
            //Conectar();
        }
        
        public void Disconnect()
        {
            if (_isConnected)
                this.CloseConn();
        }

        private void CloseConn() // Close connection.
        {
            netDataReader.Close();
            netDataWriter.Close();
            netStream.Close();
            commChannel.Close();
            OnDisconnected();
            _isConnected = false;
        }

        public void Conectar()
        {
            try
            {
                if (!_isConnected)
                {
                    _isConnected = true;
                    //tcpTask = new Task(() => ConnectionSetup());
                    //tcpTask.Start();
                    EstablecerConexion();
                }
            }
            catch (Exception)
            {
                evtLW.writeError("Error al establecer conexión con el servidor");
                //Console.WriteLine("Error al establecer conexión con el servidor");
                OnServerError();
            }
        }

        private void EstablecerConexion()
        {
            try
            {
                commChannel = new TcpClient(Server, Port);  //Connect to server
                netStream = commChannel.GetStream();
                netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
            }
            catch (Exception)
            {
                evtLW.writeError("Error al establecer conexion");
                //Console.WriteLine("NetClient Manager: Error connecting to Server");
                OnServerError();
            }

            //Logica de conexion
            try
            {
                //Lo primero que se hace es esperar por un mensaje de control CM_HELLO del servidor
                ControlMessage hello = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());

                if (hello == ControlMessage.CM_Hola)
                {
                    //Recibimos un Hello, procedemos a responder con un hello.
                    netDataWriter.Write((Byte)ControlMessage.CM_Hola);
                    netDataWriter.Flush();

                    //Esperamos el ACK del servidor
                    ControlMessage srvAns = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());
                    if (srvAns == ControlMessage.CM_Entendido)
                    {
                        //El servidor nos respondió el ACK. Podemos iniciar a enviar mensajes.
                        OnConnected(); //Disparamos el evento de conexión exitosa.
                    }
                    else
                    {
                        Disconnect();
                        //Console.WriteLine("Error al establecer conexión con el servidor");
                        evtLW.writeError("Error al establecer conexión con el servidor");
                    }
                }
            }
            catch (Exception)
            {
                evtLW.writeError("Error al establecer conexión con el servidor");
                OnServerError();
            }
        }
        
        //Metodo para enviar un comentario al servidor para almacenar en BD
        public void EnviarComentarioAServidor(Comentario C)
        {
            try
            {
                if (_isConnected)
                {
                    netDataWriter.Write((Byte)(ControlMessage.CM_Comentario));
                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(C).Length);
                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(C));
                    netDataWriter.Flush();

                    ControlMessage respuestaServidor = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());

                    if (respuestaServidor == ControlMessage.CM_OK)
                    {
                        //MessageEventArgs mEa = new MessageEventArgs(clientMsg, DateTime.Now);
                        OnComentarioEnviado(); //Lanzamos el evento de envío exitoso
                    }
                    if (respuestaServidor == ControlMessage.CM_Error)
                    {
                        OnServerError(); //Lanzamos el evento de error en el servidor
                    }
                }
            }
            catch
            {
                evtLW.writeError("Error enviando comentario al servidor");
                //Console.WriteLine("Error en envío de mensajes");
            }
        }

        public void ObtenerComentariosPersona(String cedulaP)
        {
            try
            {
                if (_isConnected)
                {
                    netDataWriter.Write((Byte)(ControlMessage.CM_ObtenerComentarios));
                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(cedulaP).Length);
                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(cedulaP));
                    netDataWriter.Flush();

                    ControlMessage respuestaServidor = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());

                    switch (respuestaServidor)
                    {
                        case ControlMessage.CM_OK:
                            //evtLW.writeWarning("CM_OK");
                            List<Comentario> comentariosPersona;
                            int tamanoComentarios = netDataReader.ReadInt32();
                            evtLW.writeWarning("TAMANO COMENTARIOS = " + tamanoComentarios);
                            comentariosPersona = (List<Comentario>)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(tamanoComentarios));
                            /*String mensaje = "Lista de comentarios = ";
                            foreach (Comentario s in comentariosPersona)
                            {
                                mensaje += Environment.NewLine + s.Texto;
                            }
                            evtLW.writeWarning(mensaje);*/
                            ComentarioEventos ce = new ComentarioEventos();
                            ce.Comentarios = comentariosPersona;
                            OnComentariosRecibidos(ce);
                            break;
                        case ControlMessage.CM_Error:
                            OnServerError();
                            break;
                        case ControlMessage.CM_SinComentarios:
                            OnSinComentarios();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                evtLW.writeError("Error al solicitar comentarios de persona");
                throw;
            }
        }

        public void ObtenerCedulas()
        {
            try
            {
                if (_isConnected)
                {
                    netDataWriter.Write((Byte)(ControlMessage.CM_ObtenerCedulas));
                    netDataWriter.Flush();

                    ControlMessage respuestaServidor = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());

                    switch (respuestaServidor)
                    {
                        case ControlMessage.CM_OK:
                            List<String> cedulasPersonas;
                            int tamanoCedulas = netDataReader.ReadInt32();
                            cedulasPersonas = (List<String>)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(tamanoCedulas));
                            /*String mensaje = "Lista de cedulas = ";
                            foreach (String s in cedulasPersonas)
                            {
                                mensaje += Environment.NewLine + s;
                            }
                            evtLW.writeWarning(mensaje);*/
                            CedulaEventos ce = new CedulaEventos();
                            ce.Cedulas = cedulasPersonas;
                            OnListaCedulas(ce);
                            break;
                        case ControlMessage.CM_Error:
                            OnServerError();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                evtLW.writeError("Error al solicitar cedulas");
                throw;
            }

        }
    
    }
}
