﻿
using BepInEx.Configuration;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter : KontrolSystemConfig {
        internal ConfigEntry<string> stdLibFolder;

        internal ConfigAdapter(ConfigFile config) {
            stdLibFolder = config.Bind("Paths", "stdLibPath", "",
                "Folder of the standard library");
        }
        
        public string StdLibPath => stdLibFolder.Value;
        
        public string To2Path => "";

        public static ConfigAdapter Instance;

        internal static void Init(ConfigFile config) {
            Instance = new ConfigAdapter(config);
        }
    }
}
