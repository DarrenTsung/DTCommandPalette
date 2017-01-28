using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
    public class CommandPaletteWindow : EditorWindow {
        // PRAGMA MARK - Constants
        private const string kTextFieldControlName = "CommandPaletteWindowTextField";

        private const int kMaxRowsDisplayed = 8;
        private const float kWindowWidth = 400.0f;
        private const float kWindowHeight = 30.0f;

        private const float kRowHeight = 30.0f;
        private const float kRowTitleHeight = 20.0f;
        private const float kRowSubtitleHeightPadding = -5.0f;
        private const float kRowSubtitleHeight = 15.0f;

        private const int kSubtitleMaxSoftLength = 35;
        private const int kSubtitleMaxTitleAdditiveLength = 15;

        private const float kIconEdgeSize = 17.0f;
        private const float kIconPadding = 7.0f;

        private const int kFontSize = 20;

        public static string _scriptDirectory = null;
        public static string ScriptDirectory {
            get {
                if (CommandPaletteWindow._scriptDirectory == null) {
                    CommandPaletteWindow window = ScriptableObject.CreateInstance<CommandPaletteWindow>();
                    MonoScript script = MonoScript.FromScriptableObject(window);
                    CommandPaletteWindow._scriptDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
                    ScriptableObject.DestroyImmediate(window);
                }
                return CommandPaletteWindow._scriptDirectory;
            }
        }


        // PRAGMA MARK - Public Interface
        [MenuItem("Window/Open.. %t")]
        public static void ShowObjectWindow() {
            CommandPaletteWindow.InitializeWindow("Open.. ");

            CommandPaletteWindow._commandManager = new CommandManager();
            if (!Application.isPlaying) {
                CommandPaletteWindow._commandManager.AddLoader(new PrefabAssetCommandLoader());
                CommandPaletteWindow._commandManager.AddLoader(new SceneAssetCommandLoader());
            }
            CommandPaletteWindow._commandManager.AddLoader(new SelectGameObjectCommandLoader());
            CommandPaletteWindow.ReloadObjects();
        }

        [MenuItem("Window/Open Command Palette.. %#m")]
        public static void ShowCommandPaletteWindow() {
            CommandPaletteWindow.InitializeWindow("Command Palette.. ");

            CommandPaletteWindow._commandManager = new CommandManager();
            CommandPaletteWindow._commandManager.AddLoader(new MethodCommandLoader());
            CommandPaletteWindow.ReloadObjects();
        }

        public static void InitializeWindow(string title) {
            EditorWindow window = EditorWindow.GetWindow(typeof(CommandPaletteWindow), utility: true, title: title, focus: true);
            window.position = new Rect(0.0f, 0.0f, CommandPaletteWindow.kWindowWidth, CommandPaletteWindow.kWindowHeight);
            window.CenterInMainEditorWindow();

            CommandPaletteWindow._selectedIndex = 0;
            CommandPaletteWindow._focusTrigger = true;
            CommandPaletteWindow._isClosing = false;
        }

        // PRAGMA MARK - Internal
        protected static string _input = "";
        protected static bool _focusTrigger = false;
        protected static bool _isClosing = false;
        protected static int _selectedIndex = 0;
        protected static ICommand[] _objects = new ICommand[0];
        protected static CommandManager _commandManager = null;
        protected static Color _selectedBackgroundColor = ColorUtil.HexStringToColor("#4076d3").WithAlpha(0.4f);

        private static string _parsedSearchInput = "";
        private static string[] _parsedArguments = null;

        protected void OnGUI() {
            Event e = Event.current;
            switch (e.type) {
                case EventType.KeyDown:
                this.HandleKeyDownEvent(e);
                break;
                default:
                break;
            }

            if (CommandPaletteWindow._objects.Length > 0) {
                CommandPaletteWindow._selectedIndex = MathUtil.Wrap(CommandPaletteWindow._selectedIndex, 0, Mathf.Min(CommandPaletteWindow._objects.Length, CommandPaletteWindow.kMaxRowsDisplayed));
            } else {
                CommandPaletteWindow._selectedIndex = 0;
            }

            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontSize = CommandPaletteWindow.kFontSize;

            GUI.SetNextControlName(CommandPaletteWindow.kTextFieldControlName);
            string updatedInput = EditorGUI.TextField(new Rect(0.0f, 0.0f, CommandPaletteWindow.kWindowWidth, CommandPaletteWindow.kWindowHeight), CommandPaletteWindow._input, textFieldStyle);
            if (updatedInput != CommandPaletteWindow._input) {
                CommandPaletteWindow._input = updatedInput;
                this.HandleInputUpdated();
            }

            int displayedAssetCount = Mathf.Min(CommandPaletteWindow._objects.Length, CommandPaletteWindow.kMaxRowsDisplayed);
            this.DrawDropDown(displayedAssetCount);

            this.position = new Rect(this.position.x, this.position.y, this.position.width, CommandPaletteWindow.kWindowHeight + displayedAssetCount * CommandPaletteWindow.kRowHeight);

            if (CommandPaletteWindow._focusTrigger) {
                CommandPaletteWindow._focusTrigger = false;
                EditorGUI.FocusTextInControl(CommandPaletteWindow.kTextFieldControlName);
            }
        }

        private void HandleInputUpdated() {
            ReparseInput();
            CommandPaletteWindow._selectedIndex = 0;
            CommandPaletteWindow.ReloadObjects();
        }

        private static void ReloadObjects() {
            if (CommandPaletteWindow._commandManager == null) {
                return;
            }

            CommandPaletteWindow._objects = CommandPaletteWindow._commandManager.ObjectsSortedByMatch(CommandPaletteWindow._parsedSearchInput);
        }

        private static void ReparseInput() {
            if (CommandPaletteWindow._input == null || CommandPaletteWindow._input.Length <= 0) {
                CommandPaletteWindow._parsedSearchInput = "";
                CommandPaletteWindow._parsedArguments = null;
                return;
            }

            string[] parameters = CommandPaletteWindow._input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            CommandPaletteWindow._parsedSearchInput = parameters[0];

            if (parameters.Length == 1) {
                CommandPaletteWindow._parsedArguments = null;
            } else {
                CommandPaletteWindow._parsedArguments = parameters.Skip(1).ToArray();
            }
        }

        private void HandleKeyDownEvent(Event e) {
            switch (e.keyCode) {
                case KeyCode.Escape:
                    this.CloseIfNotClosing();
                    break;
                case KeyCode.Return:
                    ICommand obj = CommandPaletteWindow._objects[CommandPaletteWindow._selectedIndex];

                    var parsedArguments = CommandPaletteWindow._parsedArguments;
                    EditorApplication.delayCall += () => {
                        if (parsedArguments != null) {
                            ICommandWithArguments castedObj;
                            try {
                                castedObj = (ICommandWithArguments)obj;
                                castedObj.Execute(parsedArguments);
                            } catch (InvalidCastException) {
                                Debug.LogWarning("Attempted to pass arguments to CommandObject, but object does not allow arguments!");
                                obj.Execute();
                            }
                        } else {
                            obj.Execute();
                        }
                    };

                    this.CloseIfNotClosing();
                    break;
                case KeyCode.DownArrow:
                    CommandPaletteWindow._selectedIndex++;
                    e.Use();
                    break;
                case KeyCode.UpArrow:
                    CommandPaletteWindow._selectedIndex--;
                    e.Use();
                    break;
                default:
                    break;
            }
        }

        private void DrawDropDown(int displayedAssetCount) {
            HashSet<char> inputSet = new HashSet<char>();
            foreach (char c in _input.ToLower()) {
                inputSet.Add(c);
            }

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.richText = true;

            GUIStyle subtitleStyle = new GUIStyle(GUI.skin.label);
            subtitleStyle.fontSize = 9;

            int currentIndex = 0;
            for (int i = 0; i < displayedAssetCount; i++) {
                ICommand obj = CommandPaletteWindow._objects[i];

                if (!obj.IsValid()) {
                    continue;
                }

                float topY = CommandPaletteWindow.kWindowHeight + CommandPaletteWindow.kRowHeight * currentIndex;

                if (currentIndex == CommandPaletteWindow._selectedIndex) {
                    EditorGUI.DrawRect(new Rect(0.0f, topY, CommandPaletteWindow.kWindowWidth, CommandPaletteWindow.kRowHeight), CommandPaletteWindow._selectedBackgroundColor);
                }

                string title = obj.DisplayTitle;
                string subtitle = obj.DisplayDetailText;

                int subtitleMaxLength = Math.Min(CommandPaletteWindow.kSubtitleMaxSoftLength + title.Length, CommandPaletteWindow.kSubtitleMaxSoftLength + CommandPaletteWindow.kSubtitleMaxTitleAdditiveLength);
                if (subtitle.Length > subtitleMaxLength + 2) {
                    subtitle = ".." + subtitle.Substring(subtitle.Length - subtitleMaxLength);
                }

                string colorHex = EditorGUIUtility.isProSkin ? "#8e8e8e" : "#383838";

                StringBuilder titleStringBuilder = new StringBuilder();
                bool startedConsecutive = false;
                bool finishedConsecutive = false;
                foreach (char c in title) {
                    if (!finishedConsecutive && inputSet.Contains(char.ToLower(c))) {
                        startedConsecutive = true;
                        titleStringBuilder.Append(c);
                    } else {
                        if (startedConsecutive) {
                            finishedConsecutive = true;
                        }
                        titleStringBuilder.Append("<color=" + colorHex + ">" + c + "</color>");
                    }
                }
                title = titleStringBuilder.ToString();

                EditorGUI.LabelField(new Rect(0.0f, topY, CommandPaletteWindow.kWindowWidth, CommandPaletteWindow.kRowTitleHeight), title, titleStyle);
                EditorGUI.LabelField(new Rect(0.0f, topY + CommandPaletteWindow.kRowTitleHeight + CommandPaletteWindow.kRowSubtitleHeightPadding, CommandPaletteWindow.kWindowWidth, CommandPaletteWindow.kRowSubtitleHeight), subtitle, subtitleStyle);

                GUIStyle textureStyle = new GUIStyle();
                textureStyle.normal.background = obj.DisplayIcon;
                EditorGUI.LabelField(new Rect(CommandPaletteWindow.kWindowWidth - CommandPaletteWindow.kIconEdgeSize - CommandPaletteWindow.kIconPadding, topY + CommandPaletteWindow.kIconPadding, CommandPaletteWindow.kIconEdgeSize, CommandPaletteWindow.kIconEdgeSize), GUIContent.none, textureStyle);

                // NOTE (darren): we only increment currentIndex if we draw the object
                // because it is used for positioning the UI
                currentIndex++;
            }
        }

        private void OnFocus() {
            CommandPaletteWindow._focusTrigger = true;
        }

        private void OnLostFocus() {
            this.CloseIfNotClosing();
        }

        protected void CloseIfNotClosing() {
            if (!CommandPaletteWindow._isClosing) {
                CommandPaletteWindow._isClosing = true;
                this.Close();
            }
        }
    }
}