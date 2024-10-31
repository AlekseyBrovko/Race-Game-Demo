using UnityEngine;

namespace Saver
{
    public class SavedData
    {
        static int _version = 1;

        public static int Version
        {
            get
            {
                return _version;
            }
        }

        public bool Sounds = true;
        public bool Music = true;
        public bool Vibration = true;
        public bool Tutorial = true;

        public float SoundsVolumeValue = 1;
        public float MusicVolumeValue = 1;
    }
}
