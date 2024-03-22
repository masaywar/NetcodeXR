
using UnityEditor;
using System.Reflection;
using System;
using UnityEditor.Build;
using UnityEngine;
using NetcodeXR.Utility;

namespace NetcodeXR.NetcodeXREditor
{
    public class ProjectConfiguration
    {
        public static readonly string INSTALLED_ROOT_MOTION = "INSTALLED_ROOT_MOTION";
        static readonly string REQUIRED_ASSEMBLIES = 
            @"RootMotion,"+
            "Unity.Netcode.Runtime,"+
            "Unity.XR.Interaction.Toolkit";

        [MenuItem("NetcodeXR/Validate Project Setting")]
        public static void Validate()        
        {
            var assemblies = GetRequiredAssemblies();

            if(assemblies == null)
            {
                NetcodeXRLog.LogError("Assemblies string Split Error!");
                return; 
            }

            bool isAssembliesInstalled = true;

            foreach(var s in assemblies)
            {
                string polishedString = s.Trim();
                bool isInstalled = CheckLibraryInstallation(polishedString);
                isAssembliesInstalled &= isInstalled;

                if(!isAssembliesInstalled)
                {
                    NetcodeXRLog.LogError($"{polishedString} package is not installed! please install to run NetcodeXR Properly");
                }

                else
                {
                    NetcodeXRLog.Log($"{polishedString} is installed!");
                }
            }

            if(isAssembliesInstalled)
            {
                AddDefineSymbol(INSTALLED_ROOT_MOTION);
            }

            else
            {
                RemoveDefineSymbol(INSTALLED_ROOT_MOTION);
            }
        }

        [MenuItem("NetcodeXR/Create Default Server Scene")]
        public static void CreateDefaultServerScene()
        {
            var configObject = ScriptableObject.CreateInstance<DefaultNetcodeXRSetting>(); 
            
            configObject.CreateScene(true);
        } 

        [MenuItem("NetcodeXR/Create Default Client Scene")]
        public static void CreateDefaultClientScene()
        {
            var configObject = ScriptableObject.CreateInstance<DefaultNetcodeXRSetting>(); 
            
            configObject.CreateScene(false);
        } 

        [MenuItem("GameObject/NetcodeXR/XROrigin")]
        public static void InstantiateXROrigin()
        {
            
        }

        private static string[] GetRequiredAssemblies()
        {
            var assemblyNames = REQUIRED_ASSEMBLIES.Split(",", StringSplitOptions.RemoveEmptyEntries);
        
            return assemblyNames;
        }        

        private static bool CheckLibraryInstallation(string targetAssembly)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(var assembly in assemblies)
            {
                var name = assembly.GetName();

                if(name.Name == targetAssembly)
                {
                    return true;
                }                
            }    

            return false;
        }

        private static void AddDefineSymbol(string symbol)
        {
            SymbolDefineHelper.AddDefineSymbol(NamedBuildTarget.Standalone, symbol);
            SymbolDefineHelper.AddDefineSymbol(NamedBuildTarget.Android, symbol);
        }

        private static void RemoveDefineSymbol(string symbol)
        {
            SymbolDefineHelper.RemoveDefineSymbol(NamedBuildTarget.Android, symbol);
            SymbolDefineHelper.RemoveDefineSymbol(NamedBuildTarget.Standalone, symbol);
        }


    }
}
