using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTCommandPalette {
    public class CommandManager {
        // PRAGMA MARK - Constructors
        public void AddLoader(ICommandLoader loader) {
            this._objectLoaders.Add(loader);

            // Add all objects from loaded into _loadedObjects
            ICommand[] objects = loader.Load();
            this._loadedObjects.AddRange(objects);
        }

        // PRAGMA MARK - Public Interface
        public ICommand[] ObjectsSortedByMatch(string input) {
            string inputLowercase = input.ToLower();

            List<ICommand> objectsCopy = new List<ICommand>(this._loadedObjects);

            Dictionary<string, double> cachedDistances = new Dictionary<string, double>();
            foreach (ICommand obj in objectsCopy) {
                string displayTitle = obj.DisplayTitle;
                string displayTitleLowercase = displayTitle.ToLower();

                float editDistance = ComparisonUtil.EditDistance(displayTitleLowercase, inputLowercase);

                string longestCommonSubstring = ComparisonUtil.LongestCommonSubstring(displayTitleLowercase, inputLowercase);
                float substringLength = longestCommonSubstring.Length;
                float substringIndex = displayTitleLowercase.IndexOf(longestCommonSubstring);

                double distance = 0;
                distance += 0.05f * editDistance;
                distance += 2.0f * -substringLength;
                distance += substringIndex;

                cachedDistances[displayTitle] = distance;
            }

            objectsCopy.Sort(delegate(ICommand objA, ICommand objB) {
                double distanceA = cachedDistances[objA.DisplayTitle];
                double distanceB = cachedDistances[objB.DisplayTitle];
                return distanceA.CompareTo(distanceB);
                });

                return objectsCopy.ToArray();
            }


            // PRAGMA MARK - Internal
            private List<ICommand> _loadedObjects = new List<ICommand>();
            private List<ICommandLoader> _objectLoaders = new List<ICommandLoader>();
        }
    }
