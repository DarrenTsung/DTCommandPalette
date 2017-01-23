using System;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public class PrefabAssetCommand : AssetCommand {
        public static Action<string> OnPrefabGUIDExecuted;

        private static Texture2D _prefabDisplayIcon;
        private static Texture2D PrefabDisplayIcon {
            get {
                if (_prefabDisplayIcon == null) {
                    _prefabDisplayIcon = AssetDatabase.LoadAssetAtPath(CommandPaletteWindow.ScriptDirectory + "/Icons/PrefabIcon.png", typeof(Texture2D)) as Texture2D;
                }
                return _prefabDisplayIcon ?? new Texture2D(0, 0);
            }
        }

        // PRAGMA MARK - ICommand
        public override Texture2D DisplayIcon {
            get {
                return PrefabAssetCommand.PrefabDisplayIcon;
            }
        }

        public override void Execute() {
            if (PrefabAssetCommand.OnPrefabGUIDExecuted == null) {
                Debug.LogError("No action set-up to handle Prefabs Asset Commands!");
            } else {
                PrefabAssetCommand.OnPrefabGUIDExecuted.Invoke(this._guid);
            }
        }


        // PRAGMA MARK - Constructors
        public PrefabAssetCommand(string guid) : base(guid) {
            if (!PathUtil.IsPrefab(this._path)) {
                throw new ArgumentException("PrefabAssetCommand loaded with guid that's not a prefab!");
            }
        }
    }
}