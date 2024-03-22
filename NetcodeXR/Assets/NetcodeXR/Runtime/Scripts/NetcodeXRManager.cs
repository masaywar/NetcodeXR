using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace NetcodeXR
{
    using Utility;
    using Unity.Netcode;

    [DefaultExecutionOrder(-20)]
    public class NetcodeXRManager : NetworkBehaviour
    {
        private static NetcodeXRManager m_Instance = null;
        public static NetcodeXRManager Instance => m_Instance;
       

        public NetworkAvatar defaultAvatar = null;        
        public NetworkPlayer defaultPlayer = null;

        private NetworkManager m_NetworkManager = null;

        [SerializeField]
        private bool m_SpawnAvatarAtStart = true;
        public bool SpawnAvatarAtStart => m_SpawnAvatarAtStart;
        
        [SerializeField]
        private bool m_SpawnPlayerObjectAtStart = true;
        public bool SpawnPlayerObjectAtStart => m_SpawnPlayerObjectAtStart;

        [SerializeField, ReadOnly(true)]
        private bool m_Visibility;
        public bool Visibility => m_Visibility;

        [SerializeField]
        private float m_VisibilityDistance;
        public float VisibilityDistance => m_VisibilityDistance;

        private Dictionary<ulong, NetworkPlayer> m_PlayerSpawnedObjects = new Dictionary<ulong, NetworkPlayer>();
        public Dictionary<ulong, NetworkPlayer> PlayerSpawnedObjects => m_PlayerSpawnedObjects;

        public delegate void ChangeOnForceGrab(bool isForceGrab);
        public event ChangeOnForceGrab forceGrabChanged;

        private VisibilityManager m_VisibilityManager;
        public VisibilityManager VisibilityManager => m_VisibilityManager; 

        [SerializeField]
        private bool m_ForceGrab = true;
        public bool ForceGrab
        {
            get => m_ForceGrab;
            set
            {
                m_ForceGrab = value;
                forceGrabChanged.Invoke(m_ForceGrab);
            }
        }

        private void Awake()
        {
            if(m_Instance == null)
            {
                m_Instance = this;
            }

            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            if(NetworkManager.Singleton == null)
            {
                if(m_NetworkManager == null)
                    m_NetworkManager = FindObjectOfType<NetworkManager>();
                
                m_NetworkManager.SetSingleton();
            }

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            NetworkManager.Singleton.OnClientStopped += OnClientStopped;
            NetworkManager.Singleton.OnConnectionEvent += OnNetworkConnect;
        }

        private void OnDisable()
        {
            // NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            // NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
            // NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
            // NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
            // NetworkManager.Singleton.OnConnectionEvent -= OnNetworkConnect;
        }

        private void OnValidate()
        {
            if(m_NetworkManager == null)
                m_NetworkManager = FindObjectOfType<NetworkManager>();
            
            if(m_NetworkManager == null)
            {
                return;
            }

            if(m_NetworkManager.NetworkConfig.PlayerPrefab != null)
            {
                m_NetworkManager.NetworkConfig.PlayerPrefab = null;
            }
        }

        private void OnServerStarted()
        {
            if(m_NetworkManager.IsServer && m_Visibility && !m_NetworkManager.ServerIsHost)
            {
                m_VisibilityManager = new DefaultVisibilityManager(m_NetworkManager, this);
            }

            else
            {
                m_Visibility = false;
            }
        }

        private void OnServerStopped(bool value)
        {
            /**
            !TODO
            **/
        }

        private void OnNetworkConnect(NetworkManager networkManager, ConnectionEventData data)
        {
            switch(data.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    NetcodeXRLog.Log($"Client {data.ClientId} Connected!");
                    OnClientConnected(data.ClientId);
                    break;

                case ConnectionEvent.ClientDisconnected:
                    NetcodeXRLog.Log($"Client {data.ClientId} Disconnected!");
                    OnClientDisconnected(data.ClientId);
                    break;

                case ConnectionEvent.PeerConnected:
                    NetcodeXRLog.Log($"Peer {data.ClientId} Connected!");
                    OnPeerConnected(data.ClientId);
                    break;

                case ConnectionEvent.PeerDisconnected:
                    NetcodeXRLog.Log($"Peer {data.ClientId} Disconnected!");
                    OnPeerDisconnected(data.ClientId);
                    break;
                default: break;
            }
        }
      
        private void OnClientConnected(ulong clientId)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                if(clientId == NetworkManager.ServerClientId && !NetworkManager.Singleton.IsClient)
                {
                    return;
                }

                /**
                !TODO !FIXME
                Scene Manager 만들어야 할 듯 
                Spawn Position 및 그런거 만들어서 Spawn position/rotation 설정해줘야 할 듯 하다.

                예초에 Local Player를 Server Start에 맟춰 Spawn 포지션으로 위치로 옮겨야 댐

                지금은 제로 포지션으로 하겠다. 
                **/

                var pooledPlayer = NetworkObjectPool.Singleton.GetNetworkObject(defaultPlayer.gameObject, Vector3.zero, Quaternion.identity);
                pooledPlayer.SpawnAsPlayerObject(clientId);
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            /**
            !TODO
            **/ 
        }

        private void OnPeerConnected(ulong clientId)
        {
            /**
            !TODO
            **/ 
        }

        private void OnPeerDisconnected(ulong clientId)
        {
            /**
            !TODO
            **/ 
        }

        private void OnClientStarted()
        {
            /**
            !TODO
            **/ 
        }

        private void OnClientStopped(bool value)
        {
            /**
            !TODO
            **/ 
        }

        public NetworkPlayer GetNetworkPlayerById(ulong ownerId)
        {
            if(m_PlayerSpawnedObjects.TryGetValue(ownerId, out var returnValue))
            {
                return returnValue;
            }

            return null;
        }
    }

    public class DefaultVisibilityManager : VisibilityManager
    {
        public DefaultVisibilityManager(NetworkManager networkManager, NetcodeXRManager netcodeXRManager) : base(networkManager, netcodeXRManager)
        {
            m_NetworkManager = networkManager;
            m_NetcodeXRManager = netcodeXRManager;
            m_Distance = m_NetcodeXRManager.VisibilityDistance;
        }

        public override void AddNetworkObject(NetworkVisibility networkObject)
        {
            m_SpawnedObjectList.Add(networkObject);
        }

        public override void RemoveNetworkObject(NetworkVisibility networkObject)
        {
            m_SpawnedObjectList.Remove(networkObject);
        }

        protected override bool CheckVisibility(NetworkClient networkClient, NetworkVisibility checkedObject)
        {
            var playerObjectTf = networkClient.PlayerObject.transform;
            
            return Vector3.Distance(playerObjectTf.position, checkedObject.transform.position) <= m_Distance;
        }

        protected override void Update()
        {
            if(!NetworkManager.Singleton.IsServer) return;

            foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if(client.ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    continue;
                }

                foreach(var spawnedObject in m_SpawnedObjectList)
                {
                    bool isVisibile = CheckVisibility(client, spawnedObject);
                    bool shouldVisible = spawnedObject.IsNetworkVisibleTo(client.ClientId);

                    if (shouldVisible!=isVisibile)
                    {
                        if(isVisibile)
                        {
                            spawnedObject.NetworkShow(client.ClientId);    
                        }
                        else
                        {
                            spawnedObject.NetworkHide(client.ClientId);
                        }
                    }                    
                }
            }
        }
    
    }

    public abstract class VisibilityManager
    {
        protected float m_Distance;
        public float Distance
        {
            get => m_Distance;
            set => m_Distance = value;
        }

        protected NetworkManager m_NetworkManager;
        protected NetcodeXRManager m_NetcodeXRManager;

        public VisibilityManager(NetworkManager networkManager, NetcodeXRManager netcodeXRManager)
        {
            m_NetworkManager = networkManager;
            m_NetcodeXRManager = netcodeXRManager;

            m_NetworkManager.NetworkTickSystem.Tick += Update;
        } 

        protected HashSet<NetworkVisibility> m_SpawnedObjectList = new HashSet<NetworkVisibility>();


        protected abstract bool CheckVisibility(NetworkClient networkClient, NetworkVisibility checkedObject);

        protected abstract void Update();
        
        public abstract void AddNetworkObject(NetworkVisibility networkObject);
        public abstract void RemoveNetworkObject(NetworkVisibility networkObject);
       
    }
}
