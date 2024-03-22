using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetcodeXR
{
    [CreateAssetMenu(menuName = "NetcodeXR/Create Network Object Pool List")]
    public class NetworkObjectPoolList : ScriptableObject
    {
        [SerializeField]
        private List<PoolConfigObject> m_PooledPrefabList;

        public IReadOnlyList<PoolConfigObject> ReadOnlyPooledPrefabList 
        {
            get
            {
                if(m_PooledPrefabList != null)
                {
                    return m_PooledPrefabList.AsReadOnly();
                }
            
                return null;
            }
        }
    }
}
