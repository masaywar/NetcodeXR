using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeXR
{
    public class MessageHandlerManager : NetworkBehaviour
    {
        private static MessageHandlerManager m_Instance = null;
        public static MessageHandlerManager Instance => m_Instance;

        private Dictionary<string, MessageHandler> m_MessageHandlers = new Dictionary<string, MessageHandler>();
        public IReadOnlyDictionary<string, MessageHandler> MessageHandlers { get => m_MessageHandlers; }

#pragma warning disable 0414
        private CustomMessagingManager m_CachedCustomMessagingManager = null;
#pragma warning restore 0414

        public void SubscribeMessageHandler(MessageHandler handler)
        {
            if (m_MessageHandlers.ContainsKey(handler.MessageName))
            {
                Debug.LogError("Already exist message handler : " + handler.MessageName);
                return;
            }

            m_MessageHandlers.Add(handler.MessageName, handler);
        }

        public void UnsubscribeMessageHandler(MessageHandler handler)
        {
            if (!m_MessageHandlers.ContainsKey(handler.MessageName))
            {
                Debug.LogError("Not exist message handler : " + handler.MessageName);
                return;
            }

            m_MessageHandlers.Remove(handler.MessageName);
        }

        public IEnumerator SendMessageCoroutine(string messageName, MessageArgs dataToSend)
        {
            yield return StartCoroutine(WaitForSubscriptionAndSend(messageName, dataToSend));
        }

        public void SendMessageAsync(string messageName, MessageArgs dataToSend)
        {
            StartCoroutine(WaitForSubscriptionAndSend(messageName, dataToSend));
        }

        #region Methods for Reserved(named) messages

        #endregion

        private IEnumerator WaitForSubscriptionAndSend(string messageName, MessageArgs dataToSend)
        {
            float time = 0;
            var wait = new WaitForSecondsRealtime(0.02f);

            while (!m_MessageHandlers.ContainsKey(messageName))
            {
                time += 0.02f;
                if (time >= 5)
                {
                    Debug.LogError("Time out : " + messageName);
                    yield break;
                }

                yield return wait;
            }

            var messageHandler = m_MessageHandlers[messageName];
            messageHandler.SendMessage(dataToSend);
        }

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }

            else
            {
                Destroy(gameObject);
            }
        }

    }
}
