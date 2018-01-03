using System.Collections.Generic;
using Aimtec.SDK.Menu.Config;

namespace Adept_AIO.BOT.Utilities
{
    using System.Linq;

    public static class KeyManager
    {
        private static List<GlobalKey> activeKeys;

        public static void Load()
        {
            activeKeys = new List<GlobalKey>
            {
                GlobalKeys.ComboKey,
                GlobalKeys.WaveClearKey,
                GlobalKeys.MixedKey,
                GlobalKeys.LastHitKey
            };
        }

        public static void ChangeKey(GlobalKey newKey)
        {
            ClearKeys();
            newKey.KeyBindItem.Value = true;
        }

        public static void ClearKeys()
        {
            foreach (var key in activeKeys.Where(x => x.KeyBindItem.Enabled))
            {
                key.KeyBindItem.Value = false;
            }
        }
    }
}