using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public class MethodCommandConfig {
        public MethodInfo methodInfo;
        public Type classType;
        public string methodDisplayName;
    }

    public class MethodCommand : ICommandWithArguments {
        private static Texture2D _methodDisplayIcon;
        private static Texture2D MethodDisplayIcon {
            get {
                if (_methodDisplayIcon == null) {
                    _methodDisplayIcon = AssetDatabase.LoadAssetAtPath(CommandPaletteWindow.ScriptDirectory + "/Icons/MethodIcon.png", typeof(Texture2D)) as Texture2D;
                }
                return _methodDisplayIcon ?? new Texture2D(0, 0);
            }
        }

        // PRAGMA MARK - ICommand
        public string DisplayTitle {
            get {
                return _methodDisplayName;
            }
        }

        public string DisplayDetailText {
            get {
                if (_method.IsStatic) {
                    return _classType.Name + "::" + _methodDisplayName;
                } else {
                    return _classType.Name + "->" + _methodDisplayName;
                }
            }
        }

        public Texture2D DisplayIcon {
            get {
                return MethodCommand.MethodDisplayIcon;
            }
        }

        public bool IsValid() {
            return true;
        }

        public void Execute(object[] args) {
            this.ExecuteInteral(args);
        }

        public void Execute() {
            this.ExecuteInteral();
        }


        // PRAGMA MARK - Constructors
        public MethodCommand(MethodCommandConfig config) {
            _method = config.methodInfo;
            _methodDisplayName = config.methodDisplayName ?? _method.Name;
            _classType = config.classType;
        }


        // PRAGMA MARK - Internal
        protected MethodInfo _method;
        protected Type _classType;
        protected string _methodDisplayName;

        private void ExecuteInteral(object[] args = null) {
            if (_method.IsStatic) {
                _method.Invoke(null, args ?? new object[0]);
            } else {
                UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(_classType);
                if (objects.Length > 0) {
                    _method.Invoke(objects[0], args ?? new object[0]);
                } else {
                    Debug.LogWarning("MethodCommand: instance method couldn't find UnityEngine.Object instance matching type");
                }
            }
        }
    }
}