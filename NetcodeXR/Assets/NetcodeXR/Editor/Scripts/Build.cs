using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

namespace NetcodeXR.NetcodeXREditor
{
    public class Build
    {
        [MenuItem("NetcodeXR/Export Package")]
        public static void ExportPackage()
        {
            AssetDatabase.ExportPackage("Assets/NetcodeXR", "NetcodeXR.unitypackage", ExportPackageOptions.Recurse);
        }

        public static void Prepare()
        {
            SymbolDefineHelper.RemoveDefineSymbol(NamedBuildTarget.Standalone, ProjectConfiguration.INSTALLED_ROOT_MOTION);
            SymbolDefineHelper.RemoveDefineSymbol(NamedBuildTarget.Android, ProjectConfiguration.INSTALLED_ROOT_MOTION);
        }
    }
}
