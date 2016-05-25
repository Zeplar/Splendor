using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Splendor
{

    public static class SynchronousSocketListener
    {
        private static Socket listener;
        private static Socket handler;


        public static Move parseRequest()
        {
            JsonTextReader request = Listen();
            request.Read();
            request.Read();
            switch (request.Value.ToString())
            {
                case "GET":
                handler.Send(Board.current.Encode());
                return null;

                case "TAKE":
                    Gem g = Gem.FromJson(request);
                    if (g.magnitude == 2)
                    {
                        return new Move.TAKE2(g);
                    }
                    else return new Move.TAKE3(g);

                case "BUY":
                    request.ReadAsInt32();
                    return new Move.BUY(Card.allCardsByID[(int)request.Value]);

                case "RESERVE":
                    request.ReadAsInt32();
                    return new Move.RESERVE(Card.allCardsByID[(int)request.Value]);

                default:
                    throw new FormatException("Bad client request: " + request.Value);
            }
            
        }

        public static List<Player> parseStartGame()
        {
            string players = RawListen();
            var playerEnum = JsonConvert.DeserializeObject<List<int>>(players);
            Console.WriteLine(playerEnum.String());
            var ret = new List<Player>();
            foreach (int i in playerEnum)
            {
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        ret.Add(new Greedy(Heuristic.dictionary["allEval2"]));
                        break;
                    case 2:
                        ret.Add(new RandomSearch(Heuristic.dictionary["allEval2"], 10, 5000));
                        break;
                    case 3:
                        ret.Add(new Human());
                        break;
                    default:
                        throw new FormatException("Bad player added");
                }
            }
            return ret;
        }

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPAddress ipAddress = IPAddress.Loopback;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                handler = listener.Accept();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new FormatException();
            }

        }

        public static void StopListening()
        {
            handler.Close();
            listener.Close();
        }

        static string RawListen()
        {
            byte[] bytes = new Byte[1024];

            try {
                int recvd = handler.Receive(bytes);
                return System.Text.Encoding.ASCII.GetString(bytes);
            } catch { throw new FormatException(); }
        }

        static JsonTextReader Listen()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];
            try {
                int recvd = handler.Receive(bytes);
            } catch { throw new FormatException(); }
            return new JsonTextReader(new System.IO.StringReader(System.Text.Encoding.ASCII.GetString(bytes)));
        }

        public static void Reply(bool success)
        {
            try {
                if (success) handler.Send(System.Text.Encoding.ASCII.GetBytes("success"));
                else handler.Send(System.Text.Encoding.ASCII.GetBytes("failure"));
            } catch { throw new FormatException(); }
        }

        public static void EndGame()
        { try {
                bool tie, stalemate;
                string winner = GameController.getMaxPlayer(out tie, out stalemate).ToString();

                JsonTextWriter writer = new JsonTextWriter(new System.IO.StringWriter());
                writer.WriteStartObject();
                writer.WriteValue("Game Over");
                writer.WriteValue("Winner");
                writer.WriteValue(winner);
                writer.WriteValue("Tied");
                writer.WriteValue(tie || stalemate);
                writer.WriteEndObject();
                string json = writer.ToString();
                handler.Send(System.Text.Encoding.ASCII.GetBytes(json));

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch { throw new FormatException(); }

        }

    }
}