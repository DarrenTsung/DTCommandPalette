using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public abstract class AssetCommand : ICommand {
        // PRAGMA MARK - ICommand
        public string DisplayTitle {
            get {
                return _assetFileName;
            }
        }

        public string DisplayDetailText {
            get {
                return _path;
            }
        }

        public abstract Texture2D DisplayIcon {
            get;
        }

        public bool IsValid() {
            return true;
        }

        public abstract void Execute();


        // PRAGMA MARK - Constructors
        public AssetCommand(string guid) {
            _guid = guid;
            _path = AssetDatabase.GUIDToAssetPath(_guid);
            _assetFileName = Path.GetFileName(_path);
        }


        // PRAGMA MARK - Internal
        protected string _guid;
        protected string _assetFileName;
        protected string _path;
    }
}