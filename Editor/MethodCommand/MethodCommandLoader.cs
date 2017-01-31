using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    [InitializeOnLoad]
    public class MethodCommandLoader : ICommandLoader {
        // PRAGMA MARK - Static
        private static readonly HashSet<string> k_CheckedAssemblyNames = new HashSet<string>() {
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp-Editor",
            "Assembly-CSharp-Editor-firstpass",
            "Assembly-UnityScript",
            "Assembly-UnityScript-firstpass",
            "Assembly-UnityScript-Editor",
            "Assembly-UnityScript-Editor-firstpass",
        };

        private static List<ICommand> _staticCommands = new List<ICommand>();
        private static Dictionary<Type, List<ICommand>> _instanceMethodCommandMap = new Dictionary<Type, List<ICommand>>();

        static MethodCommandLoader() {
            EditorApplication.delayCall += () => {
                var thread = new Thread(LoadMethodCommands);
                thread.IsBackground = true;
                thread.Start();
            };
        }

        private static void LoadMethodCommands() {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                string assemblyName = a.GetName().Name;
                if (!k_CheckedAssemblyNames.Contains(assemblyName)) {
                    continue;
                }

                foreach (Type t in a.GetTypes()) {
                    var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (MethodInfo method in methods) {
                        MethodCommandAttribute attr = method.GetCustomAttributes(true).OfType<MethodCommandAttribute>().FirstOrDefault();
                        if (attr == null) {
                            continue;
                        }

                        MethodCommand command = new MethodCommand(new MethodCommandConfig {
                            methodInfo = method,
                            classType = t,
                            methodDisplayName = attr.methodDisplayName
                        });

                        if (command.IsStatic) {
                            _staticCommands.Add(command);
                        } else {
                            _instanceMethodCommandMap.GetAndCreateIfNotFound(t).Add(command);
                        }
                    }
                }
            }
        }


        // PRAGMA MARK - ICommandLoader
        public ICommand[] Load() {
            var commands = new List<ICommand>(_staticCommands);

            if (Selection.activeGameObject != null) {
                var typeHashSet = new HashSet<Type>();

                var componentsOnActive = Selection.activeGameObject.GetComponents(typeof(UnityEngine.Component));
                foreach (var component in componentsOnActive) {
                    if (component == null) {
                        continue;
                    }

                    var componentType = component.GetType();
                    if (typeHashSet.Contains(componentType)) {
                        continue;
                    }

                    typeHashSet.Add(componentType);

                    var methodCommands = _instanceMethodCommandMap.GetValueOrDefault(componentType);
                    if (methodCommands == null) {
                        continue;
                    }

                    commands.AddRange(methodCommands);
                }
            }

            return commands.ToArray();
        }
    }
}