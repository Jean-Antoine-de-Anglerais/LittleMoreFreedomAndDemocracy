using HarmonyLib;
using NCMS;
using System.Reflection;
using UnityEngine;

namespace LittleMoreFreedomAndDemocracy_NCMS
{
    [ModEntry]
    class Main : MonoBehaviour
    {
        public static Harmony harmony = new Harmony(MethodBase.GetCurrentMethod().DeclaringType.Namespace);

        void Awake()
        {
            harmony.PatchAll(typeof(Patches));
        }
    }
}
