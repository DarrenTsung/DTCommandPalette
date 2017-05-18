using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DTCommandPalette {
	public class InProgressCommand {
		public InProgressCommand(ICommandWithArguments commandWithArguments, Action onFinishedExecutingCommand) {
			commandWithArguments_ = commandWithArguments;
			onFinishedExecutingCommand_ = onFinishedExecutingCommand;
			arguments_ = new object[commandWithArguments.Parameters.Length];

			LoopGrabArgument();
		}


		// PRAGMA MARK - Internal
		private ICommandWithArguments commandWithArguments_;

		private Action onFinishedExecutingCommand_;
		private object[] arguments_;

		private void LoopGrabArgument() {
			for (int i = 0; i < commandWithArguments_.Parameters.Length; i++) {
				if (arguments_[i] != null) {
					continue;
				}

				int index = i;
				ParameterInfo parameterInfo = commandWithArguments_.Parameters[index];

				Type parameterType = parameterInfo.ParameterType;
				TypeConverter converter = TypeDescriptor.GetConverter(parameterType);
				if (!converter.CanConvertFrom(typeof(string))) {
					Debug.LogWarning("Cannot convert string to type: " + parameterType.Name + " from command: " + commandWithArguments_.DisplayTitle);
					arguments_[i] = new object();
					continue;
				}

				CommandPaletteArgumentWindow.Show(string.Format("{0} ({1})", parameterInfo.Name, parameterType.Name), cancelCallback: () => {
					EditorApplication.delayCall += () => {
						LoopGrabArgument();
					};
				}, argumentCallback: (string input) => {
					EditorApplication.delayCall += () => {
						try {
							arguments_[index] = converter.ConvertFrom(input);
						} catch (Exception) {
							Debug.LogWarning(string.Format("Could not convert input: {0} into type: {1}! Please try again.", input, parameterType));
						}
						LoopGrabArgument();
					};
				});
				return;
			}

			// arguments are grabbed, execute command
			if (commandWithArguments_ != null) {
				commandWithArguments_.Execute(arguments_);
				commandWithArguments_ = null;
			}

			if (onFinishedExecutingCommand_ != null) {
				onFinishedExecutingCommand_.Invoke();
				onFinishedExecutingCommand_ = null;
			}
		}
	}
}