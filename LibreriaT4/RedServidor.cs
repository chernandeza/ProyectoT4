using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace LibreriaT4
{
    public class RedServidor
    {
        private IPAddress ip = IPAddress.Parse("127.0.0.1");    //IP de "localhost"
        private int port = 10830;       //Puerto para escuchar por conexiones
        public bool running;     //Indica si el servidor está escuchando por conexiones o no
        private TcpListener listener;     //TCPListener para escuchar por conexiones
        public static ConectorBD db;            // Enlace a la base de datos

        public event EventHandler ClientConnected; // Evento se dispara cuando se conecta un cliente
        public event EventHandlerComentarioGUI ComentarioRecibido; //Evento se dispara cuando se recibe un mensaje
        public event EventHandler ClientDisconnected; //Evento se dispara cuando se desconecta un cliente.

        private EvtLogWriter evtW = new EvtLogWriter("ServidorT4", "Application");

        /*Estos métodos validan que la suscripción a los eventos no esté vacía. Si está vacía, no lanza el evento de forma innecesaria*/
        virtual protected void OnClientDisconnected()
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, EventArgs.Empty);
        }

        virtual protected void OnClientConnected()
        {
            if (ClientConnected != null)
                ClientConnected(this, EventArgs.Empty);
        }

        virtual protected void OnComentarioRecibido(ComentarioGUI e)
        {
            if (ComentarioRecibido != null)
                ComentarioRecibido(this, e);
        }
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public RedServidor()
        {
            this.running = false;
            db = new ConectorBD();
        }

        public void Initialize()
        {
            listener = new TcpListener(ip, port);
            listener.Start();
            this.running = true;
            Listen();
        }

        void Listen()
        {
            while (running)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();  // Acepta una conexión entrante
                //Crear una nueva tarea para cada uno de los clientes.
                new Task(() => ConnectionSetup(tcpClient)).Start();
            }
        }

        private void ConnectionSetup(TcpClient tcpC)
        {
            TcpClient commChannel;                   // Canal de comunicación hacia el servidor
            NetworkStream netStream;                 // Stream del canal de respuesta para enviar datos hacia el servidor.
            BinaryReader netDataReader;              // Utilizado para leer datos del canal de comunicación
            BinaryWriter netDataWriter;              // Utilizado para escribir datos en el canal de comunicación
            commChannel = tcpC;

            try
            {
                netStream = commChannel.GetStream(); //Obtenemos el canal de comunicación
                netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);

                //Enviamos un "HELLO" al cliente y esperamos respuesta
                netDataWriter.Write((Byte)(ControlMessage.CM_Hola));
                netDataWriter.Flush();

                //Esperamos un "HELLO" proveniente del cliente
                ControlMessage clientMsg = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());
                if (clientMsg == ControlMessage.CM_Hola)
                {
                    // Se recibió un mensaje de "hello". El cliente responde adecuadamente. Se inicia la interacción con el cliente.
                    // Enviamos un mensaje de Acknowledge y luego iniciamos la interacción.
                    netDataWriter.Write((Byte)(ControlMessage.CM_Entendido));
                    netDataWriter.Flush();
                    OnClientConnected();
                    InteractWithClient(ref commChannel, ref netDataReader, ref netDataWriter);  // Listen to client in loop.
                }
                else
                {
                    throw new Exception("Server Unresponsive");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ConnectionSetup" + Environment.NewLine + ex.Message);
            }
            finally
            {
                OnClientDisconnected();
                //CloseConn();
            }
        }

        private void InteractWithClient(ref TcpClient commChannel, ref BinaryReader netDataReader, ref BinaryWriter netDataWriter)  // Receive all incoming packets.
        {
            try
            {
                while (commChannel.Client.Connected)  // While we are connected.
                {
                    ControlMessage ctrlMsg = (ControlMessage)Enum.Parse(typeof(ControlMessage), netDataReader.ReadByte().ToString());
                    /*La instrucción anterior parsea el contenido de un mensaje de control que el cliente envía 
                     * previo a colocar cualquier mensaje en el medio de red. 
                     * La interacción es básicamente siempre así: 
                     *      1. El cliente envía un mensaje de control.
                     *      2. Si el mensaje de control es de tipo enviar datos, el cliente envía:
                     *          2.1 El tamaño del mensaje.
                     *          2.2 Los datos del mensaje serializados.
                     *      3. El servidor parsea el mensaje de control y el mensaje.
                     *      4. El servidor envía una confirmación al cliente indicando que se recibió el mensaje correctamente.
                     *      5. El servidor genera un evento que contiene el mensaje y que puede ser utilizado por la GUI para presentarlo al usuario.
                     */

                    switch (ctrlMsg)
                    {
                        case ControlMessage.CM_Comentario:
                            /*
                             * El cliente va a almacenar un comentario. Debemos deserializar el contenido para luego pasarlo a la GUI.
                             */
                            /*Utilizar este codigo como base para almacenar comentarios*/
                                Comentario newCom = new Comentario();
                                // Leemos el tamaño del mensaje
                                int sizeOfMsg = netDataReader.ReadInt32();
                                newCom = (Comentario)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfMsg));
                                lock (db)
                                {
                                    if (db.GuardarComentario(newCom))
                                    {
                                        netDataWriter.Write((Byte)ControlMessage.CM_OK);
                                        netDataWriter.Flush(); //Mensaje almacenado en la BD con éxito
                                        ComentarioGUI infoMsg = new ComentarioGUI();
                                        infoMsg.Coment = newCom;
                                        OnComentarioRecibido(infoMsg); //Se dispara el evento y agregamos la información del mensaje recibido
                                        // Esto para que la GUI pueda tener acceso al mensaje.
                                    }
                                    else
                                    {
                                        netDataWriter.Write((Byte)ControlMessage.CM_Error);
                                        netDataWriter.Flush(); //Problemas almacenando mensaje
                                    }
                                }
                            break;
                        case ControlMessage.CM_ObtenerComentarios:
                            int sizeOfCed = netDataReader.ReadInt32();
                            String cedula = (String)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfCed));
                            List<Comentario> comentariosPersona = db.CargarComentariosPersona(cedula);

                            if (comentariosPersona == null)
                            {
                                netDataWriter.Write((Byte)ControlMessage.CM_SinComentarios);
                                netDataWriter.Flush(); //No hay mensajes
                                evtW.writeInfo("No hay comentarios para la cedula " + cedula + " en la BD");
                            }
                            else
                            {
                                if (comentariosPersona.Count > 0)
                                {
                                    netDataWriter.Write((Byte)ControlMessage.CM_OK);
                                    netDataWriter.Flush(); //Hay comentarios
                                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(comentariosPersona).Length);
                                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(comentariosPersona));
                                    netDataWriter.Flush();
                                    evtW.writeInfo("Hay comentarios para la cedula " + cedula + " en la BD y se enviaron al cliente");
                                }
                                else
                                {
                                    netDataWriter.Write((Byte)ControlMessage.CM_Error);
                                    netDataWriter.Flush(); //Mensaje de error al cliente
                                    evtW.writeError("Hay errores en la BD");
                                }
                            }
                            /*
                             * Si existiesen más interacciones entre el cliente y el servidor, se deberían agregar más 
                             * cases a este switch según mensajes de control existan y que se envíen desde el cliente.
                             */
                            break;
                        case ControlMessage.CM_ObtenerCedulas:
                            List<String> listaCed = db.CargarCedulasPersonas();
                            if (listaCed == null)
                            {
                                netDataWriter.Write((Byte)ControlMessage.CM_Nada);
                                netDataWriter.Flush(); //No hay mensajes 
                                evtW.writeInfo("No hay cedulas en la BD");
                            }
                            else
                            {
                                if (listaCed.Count > 0)
                                {
                                    netDataWriter.Write((Byte)ControlMessage.CM_OK);
                                    netDataWriter.Flush(); //Hay mensajes
                                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(listaCed).Length);
                                    netDataWriter.Write(ObjSerializer.ObjectToByteArray(listaCed));
                                    netDataWriter.Flush();
                                    evtW.writeInfo("Hay cedulas en la BD y se enviaron al cliente");
                                }
                                else
                                {
                                    netDataWriter.Write((Byte)ControlMessage.CM_Error);
                                    netDataWriter.Flush(); //Mensaje de error al cliente
                                    evtW.writeError("Hay errores en la BD");
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
            catch (ModifiedException mE)
            {
                evtW.writeError("Error en InteractWithClient = " + mE.CustomMessage);
            }
            catch (Exception E)
            {
                evtW.writeError("Error en InteractWithClient = " + E.Message);
                netDataWriter.Write((Byte)ControlMessage.CM_Error);
                netDataWriter.Flush();
            }
        }
        
        public void End()
        {
            evtW.writeInfo("Servidor Finalizado");
            listener.Stop();
        }
    }
}
