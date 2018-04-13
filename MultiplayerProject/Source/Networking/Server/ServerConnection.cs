﻿using MultiplayerProject.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiplayerProject
{
    // protoc -I=. --csharp_out=./ ./networkpackages.proto
    public class ServerConnection
    {
        public Socket ClientSocket;
        public string ID;
        public string Name;

        public NetworkStream Stream;
        public BinaryReader Reader;
        public BinaryWriter Writer;

        private Thread _thread;

        private List<IMessageable> _messageableComponents;

        public ServerConnection(Socket socket)
        {
            ClientSocket = socket;
            Stream = new NetworkStream(ClientSocket, true);
            Reader = new BinaryReader(Stream, Encoding.UTF8);
            Writer = new BinaryWriter(Stream, Encoding.UTF8);

            ID = Guid.NewGuid().ToString();
            Name = "UNNAMED PLAYER";
            _messageableComponents = new List<IMessageable>();
        }

        public void SetPlayerName(string name)
        {
            Name = name;
        }

        public void AddServerComponent(IMessageable component)
        {
            _messageableComponents.Add(component);
        }

        public void RemoveServerComponent(IMessageable component)
        {
            _messageableComponents.Remove(component);
        }

        public void StartListeningForMessages()
        {
            _thread = new Thread(new ThreadStart(ProcessClientMessage));
            _thread.Start();
        }

        public void StopAll()
        {
            _thread.Abort();
        }

        private void CleanUp()
        {
            ClientSocket.Close();        
            for (int i = 0; i < _messageableComponents.Count; i++)
            {
                _messageableComponents[i].RemoveClient(this);
            }
            _messageableComponents.Clear();
        }

        public void SendPacketToClient(BasePacket packet, MessageType type)
        {
            byte[] bytes = packet.PackMessage(type);
            string stringToSend = Convert.ToBase64String(bytes);

            try
            {
                var testBytes = Convert.FromBase64String(stringToSend);
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED TO CONVERT TO BASE64 STRING: " + e.Message);
            }


            Writer.Write(stringToSend);
            Writer.Flush();
        }

        private void ProcessClientMessage()
        {
            try
            {
                while (true)
                {
                    string message;
                    while ((message = Reader.ReadString()) != null)
                    {
                        byte[] bytes = Convert.FromBase64String(message);
                        using (var stream = new MemoryStream(bytes))
                        {
                            while (stream.HasValidPackage(out int messageSize))
                            {
                                MessageType type = stream.UnPackMessage(messageSize, out byte[] buffer);
                                for (int i = 0; i < _messageableComponents.Count; i++)
                                {
                                    _messageableComponents[i].RecieveClientMessage(this, type, buffer);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e.Message);
            }
            finally
            {
                CleanUp();
            }
        }
    }
}
