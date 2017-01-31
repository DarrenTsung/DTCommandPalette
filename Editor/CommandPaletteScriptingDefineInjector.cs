using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    [InitializeOnLoad]
    public static class CommandPaletteScriptingDefineInjector {
        public static readonly string kScriptingDefineSymbol = "DT_COMMAND_PALETTE";

        static CommandPaletteScriptingDefineInjector() {
            PlayerSettingsUtil.AddScriptingDefineSymbolIfNotFound(BuildTargetGroup.iOS, kScriptingDefineSymbol);
        }
    }
}