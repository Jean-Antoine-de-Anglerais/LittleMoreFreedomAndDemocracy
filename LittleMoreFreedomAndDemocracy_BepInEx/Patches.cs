using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace LittleMoreFreedomAndDemocracy_BepInEx
{
    public static class Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(WorkshopMaps), nameof(WorkshopMaps.uploadMap))]
        public static IEnumerable<CodeInstruction> uploadMap_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);

            Label label = generator.DefineLabel();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_2 && codes[i + 3].opcode == OpCodes.Ldloc_0)
                {
                    Console.WriteLine("FOUND TRANSITION");

                    codes[i + 2] = new CodeInstruction(OpCodes.Brtrue_S, label);
                }

                if (codes[i].opcode == OpCodes.Ldloc_1 && codes[i - 1].opcode == OpCodes.Ret && codes[i + 2].opcode == OpCodes.Dup)
                {
                    Console.WriteLine("FOUND LABEL");

                    codes[i].labels.Add(label);
                }

                else
                {
                    Console.WriteLine("UNFOUNDED");
                }
            }

            return codes.AsEnumerable();
        }


        /*public static IEnumerable<CodeInstruction> UploadMapTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);

            int indexStart = codes.FindIndex(instr => instr.opcode == OpCodes.Ldfld && ((FieldInfo)instr.operand).Name == "width") - 1;
            int indexStop = codes.FindIndex(instr => instr.opcode == OpCodes.Ldstr && (string)instr.operand == "Not a square world!") + 5;
            int indexLabel = codes.FindIndex(instr => instr.opcode == OpCodes.Stloc_2) + 2;

            if (indexStart == -1)
            {
                Console.WriteLine("uploadMap_Transpiler: indexStart not found");
                return codes.AsEnumerable();
            }

            if (indexStop == -1)
            {
                Console.WriteLine("uploadMap_Transpiler: indexStop not found");
                return codes.AsEnumerable();
            }

            if (indexLabel == -1)
            {
                Console.WriteLine("uploadMap_Transpiler: indexLabel not found");
                return codes.AsEnumerable();
            }

            Label label = generator.DefineLabel();
            codes[indexStop + 1].labels.Add(label);

            codes.Insert(indexLabel + 1, new CodeInstruction(OpCodes.Brtrue_S, label));
            codes.RemoveAt(indexLabel);

            codes.RemoveRange(indexStart, indexStop);

            foreach (var item in codes)
            {
                Console.WriteLine(item?.opcode.Name + "      " + item?.operand?.ToString());
            }

            return codes.AsEnumerable();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopMaps), nameof(WorkshopMaps.uploadMap))]
        public static bool uploadMap_Prefix(ref Promise __result)
        {
            Promise promise = new Promise();
            WorkshopMaps.uploadProgress = 0f;
            WorkshopMapData workshopMapData = WorkshopMapData.currentMapToWorkshop();
            SaveManager.currentWorkshopMapData = workshopMapData;
            MapMetaData meta_data_map = workshopMapData.meta_data_map;
            if (SaveManager.currentWorkshopMapData == null)
            {
                promise.Reject(new Exception("Missing world data"));
                __result = promise;
                return false;
            }
            MapMetaData meta_data_map2 = workshopMapData.meta_data_map;
            string name = meta_data_map2.mapStats.name;
            string description = meta_data_map2.mapStats.description;
            if (string.IsNullOrWhiteSpace(name))
            {
                promise.Reject(new Exception("Give your world a name!"));
                __result = promise;
                return false;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                promise.Reject(new Exception("Give your world a description!"));
                __result = promise;
                return false;
            }
            string main_path = workshopMapData.main_path;
            string preview_image_path = workshopMapData.preview_image_path;
            Editor editor = Editor.NewCommunityFile.WithTag("World");
            if (!string.IsNullOrWhiteSpace(name))
            {
                editor = editor.WithTitle(name);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                editor = editor.WithDescription(description);
            }
            if (!string.IsNullOrWhiteSpace(preview_image_path))
            {
                editor = editor.WithPreviewFile(preview_image_path);
            }
            if (!string.IsNullOrWhiteSpace(main_path))
            {
                editor = editor.WithContent(main_path);
            }
            editor = editor.WithFriendsOnlyVisibility();
            WorkshopMaps.uploadProgressTracker = new WorkshopUploadProgress();
            editor.SubmitAsync(WorkshopMaps.uploadProgressTracker).ContinueWith(delegate (Task<PublishResult> taskResult)
            {
                if (taskResult.Status != TaskStatus.RanToCompletion)
                {
                    promise.Reject(taskResult.Exception.GetBaseException());
                    return;
                }
                PublishResult result = taskResult.Result;
                if (!result.Success)
                {
                    Debug.LogError("Error when uploading Workshop world");
                }
                if (result.NeedsWorkshopAgreement)
                {
                    Debug.Log("w: Needs Workshop Agreement");
                    WorkshopUploadingWorldWindow.needsWorkshopAgreement = true;
                    WorkshopOpenSteamWorkshop.fileID = result.FileId.ToString();
                }
                if (result.Result != Result.OK)
                {
                    Debug.LogError(result.Result);
                    promise.Reject(new Exception("Something went wrong: " + result.Result.ToString()));
                    return;
                }
                WorkshopMaps.uploaded_file_id = result.FileId;
                World.world.gameStats.data.workshopUploads++;
                WorkshopAchievements.checkAchievements();
                promise.Resolve();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            __result = promise;
            return false;
        }*/
    }
}
