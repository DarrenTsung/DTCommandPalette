using UnityEngine;

namespace DTCommandPalette {
    public interface ICommand {
        string DisplayTitle {
            get;
        }

        string DisplayDetailText {
            get;
        }

        Texture2D DisplayIcon {
            get;
        }

        bool IsValid();

        void Execute();
    }
}
