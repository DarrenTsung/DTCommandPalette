using System;

namespace DTCommandPalette {
    public interface ICommandWithArguments : ICommand {
        void Execute(object[] args = null);
    }
}