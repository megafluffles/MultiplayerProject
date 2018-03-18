﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerProject.Source
{
    public interface IMessageable
    {
        MessageableComponent ComponentType { get; set; }

        List<ServerConnection> ComponentClients { get; set; }

        void RecieveClientMessage(ServerConnection client, MessageType messageType, byte[] packetBytes);
        void RemoveClient(ServerConnection client);
        void Update(GameTime gameTime);
    }
}
