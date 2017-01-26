using System;
using System.IO;
using System.Linq;
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


        // PRAGMA MARK - Public Interface
        public bool IsStatic {
            get { return _method.IsStatic; }
        }

        public Type ClassType {
            get { return _classType; }
        }

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
            args = args ?? new object[0];

            var defaultArgs = _method.GetParameters().Skip(args == null ? 0 : args.Length).Select(a => a.IsOptional ? a.DefaultValue : null);
            object[] allArgs = args.Concat(defaultArgs).ToArray();

            if (IsStatic) {
                _method.Invoke(null, allArgs);
            } else if (typeof(UnityEngine.Component).IsAssignableFrom(_classType)) {
                var activeGameObject = Selection.activeGameObject;
                if (activeGameObject == null) {
                    Debug.LogWarning("MethodCommand: cannot run method without selected game object!");
                    return;
                }

                UnityEngine.Component classTypeComponent = activeGameObject.GetComponent(_classType);
                if (classTypeComponent == null) {
                    Debug.LogWarning("MethodCommand: failed to grab component of type: " + _classType.Name + " :from selected game object!");
                    return;
                }

                _method.Invoke(classTypeComponent, allArgs);
                SceneView.RepaintAll();
                EditorUtility.SetDirty(classTypeComponent);
            } else {
                Debug.LogWarning("MethodCommand: instance method not assignable to UnityEngine.Component has no way to be run!");
            }
        }
    }
}