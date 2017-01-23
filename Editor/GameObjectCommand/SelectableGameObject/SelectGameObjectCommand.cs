using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public class SelectGameObjectCommand : GameObjectCommand {
        private static Texture2D _selectableGameObjectDisplayIcon;
        private static Texture2D SelectGameObjectCommandDisplayIcon {
            get {
                if (_selectableGameObjectDisplayIcon == null) {
                    _selectableGameObjectDisplayIcon = AssetDatabase.LoadAssetAtPath(CommandPaletteWindow.ScriptDirectory + "/Icons/GameObjectIcon.png", typeof(Texture2D)) as Texture2D;
                }
                return _selectableGameObjectDisplayIcon ?? new Texture2D(0, 0);
            }
        }

        // PRAGMA MARK - ICommand
        public override Texture2D DisplayIcon {
            get {
                return SelectGameObjectCommand.SelectGameObjectCommandDisplayIcon;
            }
        }

        public override void Execute() {
            Selection.activeGameObject = _obj;
        }


        // PRAGMA MARK - Constructors
        public SelectGameObjectCommand(GameObject obj) : base(obj) {
        }
    }
}