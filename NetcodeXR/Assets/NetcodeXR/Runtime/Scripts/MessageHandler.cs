using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace NetcodeXR
{
    public abstract class MessageHandler : NetworkBehaviour
    {
        public abstract string MessageName {get;}

        public abstract void OnReceivedMessage(ulong clientId, FastBufferReader reader);
        public abstract void SendMessage(MessageArgs dataToSend);

        public override void OnNetworkSpawn()
        {
            MessageHandlerManager.Instance?.SubscribeMessageHandler(this);
            NetworkManager?.CustomMessagingManager?.RegisterNamedMessageHandler(MessageName, OnReceivedMessage);
        }

        public override void OnNetworkDespawn()
        {
            MessageHandlerManager.Instance?.UnsubscribeMessageHandler(this);
            NetworkManager?.CustomMessagingManager?.UnregisterNamedMessageHandler(MessageName);
        }
    }

    public struct MessageArgs : INetworkSerializable
    {
        public ulong SenderObjectId;
        public ulong SenderPlayerId;
        public FixedString128Bytes Content; 

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref SenderObjectId);
            serializer.SerializeValue(ref SenderPlayerId);
            serializer.SerializeValue(ref Content);
        }

        public override string ToString()
        {
            return "Sender Id : " + SenderObjectId + " ClientId : " + SenderPlayerId + " Content : " + Content;
        }
    }   
}
