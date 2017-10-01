using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette.ScriptingDefines {
	public static class ScriptingDefinesManager {
		public static BuildTargetGroup CurrentTargetGroup {
			get { return EditorUserBuildSettings.selectedBuildTargetGroup; }
		}

		public static string[] GetCurrentDefines() {
			return GetDefinesFor(CurrentTargetGroup);
		}

		public static string[] GetDefinesFor(BuildTargetGroup targetGroup) {
			string scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			return scriptingDefinesString.Split(';');
		}

		public static bool RemoveDefine(string symbol) {
			return RemoveDefine(CurrentTargetGroup, symbol);
		}

		public static bool RemoveDefine(BuildTargetGroup targetGroup, string symbol) {
			string[] scriptingDefines = GetDefinesFor(targetGroup);
			if (!scriptingDefines.Contains(symbol)) {
				return false;
			}

			scriptingDefines = scriptingDefines.Where(s => s != symbol).ToArray();
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", scriptingDefines));
			return true;
		}

		public static bool AddDefineIfNotFound(string symbol) {
			return AddDefineIfNotFound(CurrentTargetGroup, symbol);
		}

		public static bool AddDefineIfNotFound(BuildTargetGroup targetGroup, string symbol) {
			string[] scriptingDefines = GetDefinesFor(targetGroup);
			if (scriptingDefines.Contains(symbol)) {
				return false;
			}

			scriptingDefines = scriptingDefines.ConcatSingle(symbol).ToArray();
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", scriptingDefines));
			return true;
		}
	}
}
