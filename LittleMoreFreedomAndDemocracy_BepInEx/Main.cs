using BepInEx;
using HarmonyLib;
using static ConstantNamespace.ConstantClass;

namespace LittleMoreFreedomAndDemocracy_BepInEx
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public static Harmony harmony = new Harmony(pluginName);
        private bool _initialized = false;

        public void Update()
        {
            if (global::Config.gameLoaded && !_initialized)
            {
                harmony.PatchAll(typeof(Patches));

                _initialized = true;
            }
        }
    }
}
