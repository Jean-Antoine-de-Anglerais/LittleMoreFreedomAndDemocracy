using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace LittleMoreFreedomAndDemocracy_NativeModloader
{
    internal class Main : MonoBehaviour
    {
        public static Harmony harmony = new Harmony(MethodBase.GetCurrentMethod().DeclaringType.Namespace);
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
