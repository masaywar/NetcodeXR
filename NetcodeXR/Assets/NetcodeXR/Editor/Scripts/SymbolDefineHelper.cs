using UnityEngine;
using UnityEditor.Build;
using UnityEditor;
using NetcodeXR.Utility;

namespace NetcodeXR.NetcodeXREditor
{
    public static class SymbolDefineHelper
    {
        static readonly string PREFIX = "[NetcodeXR SymbolDefineHelper] : ";

        public static void AddDefineSymbol(NamedBuildTarget namedBuildTarget, string symbol)
        {
            if(IsDefinedSymbol(namedBuildTarget, symbol)) 
            {
                NetcodeXRLog.Log(PREFIX +$"{symbol} is already in {namedBuildTarget.TargetName}");

                return;
            }

            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols + ";" + symbol);
        }

        public static void RemoveDefineSymbol(NamedBuildTarget namedBuildTarget, string symbol)
        {
            if(!IsDefinedSymbol(namedBuildTarget, symbol))
            {
                return;
            }

            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            var newSymbols = symbols.Replace(symbol + ";", "").Replace(symbol, "");
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols);
        }

        public static bool IsDefinedSymbol(NamedBuildTarget namedBuildTarget, string symbol)
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            return symbols.Contains(symbol);
        }
    }
}
