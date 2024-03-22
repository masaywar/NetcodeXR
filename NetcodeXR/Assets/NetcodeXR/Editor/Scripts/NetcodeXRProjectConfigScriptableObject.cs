using UnityEngine;

namespace NetcodeXR.NetcodeXREditor
{
    public abstract class NetcodeXRProjectConfigScriptableObject : ScriptableObject
    {
        public abstract void CreateScene(bool isServer);
      
    }
}
