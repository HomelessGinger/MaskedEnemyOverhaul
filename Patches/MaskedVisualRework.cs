using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using HarmonyLib.Tools;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace MaskedEnemyRework.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedVisualRework
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ReformVisuals(ref MaskedPlayerEnemy __instance)
        {
            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);

            PlayerControllerB[] playerObjects = StartOfRound.Instance.allPlayerScripts; // this spawns max amount of players, not total players (not dynamic)
            int playerCount = StartOfRound.Instance.ClientPlayerList.Count;
            if (playerCount == 0)
            {
                playerCount = UnityEngine.Random.Range(0,2);
                logger.LogInfo("Player count was zero");
            }


            // this chooses the player to mimic
            
            var randomPlayerIndex = StartOfRound.Instance.randomMapSeed % playerCount;

            logger.LogInfo("player count" + playerCount);
            logger.LogInfo("player index" + randomPlayerIndex);
            randomPlayerIndex = Mathf.Clamp(randomPlayerIndex, 0, playerCount);

            var playerToMimic = playerObjects[randomPlayerIndex];
            __instance.mimickingPlayer = playerToMimic;
            // replace player models with ones found on active Clients in level

            __instance.SetSuit(playerToMimic.currentSuitID);

            // remove mask
            if (Plugin.RemoveMasks)
            {
                __instance.gameObject.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/HeadMaskComedy").gameObject.SetActive(false);
                __instance.gameObject.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/HeadMaskTragedy").gameObject.SetActive(false);
            }
        }
    }
}
