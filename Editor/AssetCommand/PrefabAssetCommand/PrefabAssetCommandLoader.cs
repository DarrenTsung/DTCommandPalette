using System;
using System.Collections.Generic;
using UnityEditor;

namespace DTCommandPalette {
    public class PrefabAssetCommandLoader : ICommandLoader {
        // PRAGMA MARK - ICommandLoader
        public ICommand[] Load() {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");

            List<ICommand> objects = new List<ICommand>();
            foreach (string guid in guids) {
                objects.Add(new PrefabAssetCommand(guid));
            }
            return objects.ToArray();
        }
    }
}