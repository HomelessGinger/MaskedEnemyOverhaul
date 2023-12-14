using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaskedEnemyRework.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class GetMaskedPrefabForLaterUse
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void SavesPrefabForLaterUse(ref SelectableLevel[] ___moonsCatalogueList)
        {
            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            foreach(var moon in ___moonsCatalogueList) // finds enemy type to add to 
            {
                foreach(SpawnableEnemyWithRarity enemy in moon.Enemies)
                {
                    if(enemy.enemyType.enemyName == "Masked")
                    {
                        logger.LogInfo("Found Masked!");
                        Plugin.maskedPrefab = enemy;
                    } else if(enemy.enemyType.enemyName == "Flowerman")
                    {
                        Plugin.flowerPrefab = enemy;
                        logger.LogInfo("Found Flowerman!");

                    }
                }
            }
        }
    }
}
