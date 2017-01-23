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
        private static ICommand[] methodObjects = new ICommand[0];

        static MethodCommandLoader() {
            var thread = new Thread(LoadMethodCommands);
            thread.Start();
        }

        private static void LoadMethodCommands() {
            List<ICommand> objects = new List<ICommand>();

            List<Assembly> assemblies = new List<Assembly>();
            // Editor Assembly
            assemblies.Add(Assembly.GetAssembly(typeof(MethodCommandLoader)));
            // Runtime Assembly
            assemblies.Add(Assembly.GetAssembly(typeof(MethodCommandAttribute)));

            foreach (Assembly a in assemblies) {
                foreach (Type t in a.GetTypes()) {
                    var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (MethodInfo method in methods) {
                        MethodCommandAttribute attr = method.GetCustomAttributes(true).OfType<MethodCommandAttribute>().FirstOrDefault();
                        if (attr == null) {
                            continue;
                        }

                        MethodCommand openable = new MethodCommand(new MethodCommandConfig {
                            methodInfo = method,
                            classType = t,
                            methodDisplayName = attr.methodDisplayName
                        });
                        objects.Add(openable);
                    }
                }
            }

            MethodCommandLoader.methodObjects = objects.ToArray();
        }


        // PRAGMA MARK - ICommandLoader
        public ICommand[] Load() {
            return MethodCommandLoader.methodObjects;
        }
    }
}