using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public abstract class GameObjectCommand : ICommand {
        // PRAGMA MARK - ICommand
        public string DisplayTitle {
            get {
                return _obj.name;
            }
        }

        public string DisplayDetailText {
            get {
                return _obj.FullName();
            }
        }

        public abstract Texture2D DisplayIcon {
            get;
        }

        public bool IsValid() {
            return _obj != null;
        }

        public abstract void Execute();


        // PRAGMA MARK - Constructors
        public GameObjectCommand(GameObject obj) {
            _obj = obj;
        }


        // PRAGMA MARK - Internal
        protected GameObject _obj;
    }
}