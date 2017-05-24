using System;
using System.Collections.Generic;
using UnityEditor;

namespace DTCommandPalette {
	public class PrefabAssetCommandLoader : ICommandLoader {
		private static readonly ICommand[] kEmptyArray = new ICommand[0];

		// PRAGMA MARK - ICommandLoader
		public ICommand[] Load() {
			if (PrefabAssetCommand.OnPrefabGUIDExecuted == null) {
				return kEmptyArray;
			}

			string[] guids = AssetDatabase.FindAssets("t:Prefab");

			List<ICommand> objects = new List<ICommand>();
			foreach (string guid in guids) {
				objects.Add(new PrefabAssetCommand(guid));
			}
			return objects.ToArray();
		}
	}
}