using HarmonyLib;
using RSG;
using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine;

namespace LittleMoreFreedomAndDemocracy_NativeModloader
{
    public static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopMaps), "uploadMap")]
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
        }
    }
}
