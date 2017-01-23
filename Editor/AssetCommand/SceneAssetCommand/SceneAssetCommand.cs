using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DTCommandPalette {
    public class SceneAssetCommand : AssetCommand {
        private static Texture2D _sceneDisplayIcon;
        private static Texture2D SceneDisplayIcon {
            get {
                if (_sceneDisplayIcon == null) {
                    _sceneDisplayIcon = AssetDatabase.LoadAssetAtPath(CommandPaletteWindow.ScriptDirectory + "/Icons/SceneIcon.png", typeof(Texture2D)) as Texture2D;
                }
                return _sceneDisplayIcon ?? new Texture2D(0, 0);
            }
        }

        // PRAGMA MARK - ICommand
        public override Texture2D DisplayIcon {
            get {
                return SceneAssetCommand.SceneDisplayIcon;
            }
        }

        public override void Execute() {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                EditorSceneManager.OpenScene(this._path);
            }
        }


        // PRAGMA MARK - Constructors
        public SceneAssetCommand(string guid) : base(guid) {
            if (!PathUtil.IsScene(_path)) {
                throw new ArgumentException("SceneAssetCommand loaded with guid that's not a scene!");
            }
        }
    }
}