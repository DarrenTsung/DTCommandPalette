using System;
using System.Reflection;

namespace DTCommandPalette {
	public interface ICommandWithArguments : ICommand {
		ParameterInfo[] Parameters {
			get;
		}

		void Execute(object[] args);
	}
}