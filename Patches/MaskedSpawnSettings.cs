using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaskedEnemyRework.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class MaskedSpawnSettings
    {

        [HarmonyPatch("BeginEnemySpawning")]
        [HarmonyPrefix]
        static void UpdateSpawnRates(ref SelectableLevel ___currentLevel)
        {
           

            if (Plugin.UseVanillaSpawns)
                return;

            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            logger.LogInfo("Starting Round Manager");
            SpawnableEnemyWithRarity maskedEnemy = Plugin.maskedPrefab;
            SpawnableEnemyWithRarity flowerman = Plugin.flowerPrefab;
            

            ___currentLevel.maxEnemyPowerCount += maskedEnemy.enemyType.MaxCount;
            ___currentLevel.maxDaytimeEnemyPowerCount += maskedEnemy.enemyType.MaxCount;
            ___currentLevel.maxOutsideEnemyPowerCount += maskedEnemy.enemyType.MaxCount;

            List<SpawnableEnemyWithRarity> currentLevelEnemiesCopy = ___currentLevel.Enemies;
   

            for (int i = 0; i < ___currentLevel.Enemies.Count; i++)
            {
                var enemy = ___currentLevel.Enemies[i];
                if (enemy.enemyType.enemyName == "Masked")
                    currentLevelEnemiesCopy.Remove(enemy);
                if (enemy.enemyType.enemyName == "Flowerman")
                    flowerman = enemy;
            }
            ___currentLevel.Enemies = currentLevelEnemiesCopy;
            

            // might spawn too frequently, we will see.
            maskedEnemy.enemyType.PowerLevel = 1;
            maskedEnemy.enemyType.probabilityCurve = flowerman.enemyType.probabilityCurve;
            if (Plugin.UseSpawnRarity)
            {
                maskedEnemy.rarity = Plugin.SpawnRarity;
            }
            else
            {
                maskedEnemy.rarity = flowerman.rarity;
            }
            maskedEnemy.enemyType.MaxCount = Plugin.MaxSpawnCount;
            maskedEnemy.enemyType.isOutsideEnemy = Plugin.CanSpawnOutside;
            var firstTwoDigitsOfMapSeed = 0;
            if (StartOfRound.Instance.randomMapSeed.ToString().Length < 3)
            {
                firstTwoDigitsOfMapSeed = 0;
            } else
            {
                firstTwoDigitsOfMapSeed = int.Parse(StartOfRound.Instance.randomMapSeed.ToString().Substring(1, 2));
            }
            logger.LogInfo("Map seed " +  firstTwoDigitsOfMapSeed + " random chance " + Plugin.RandomChanceZombieApocalypse);

            firstTwoDigitsOfMapSeed = Mathf.Clamp(firstTwoDigitsOfMapSeed, 0, 100);
            if(Plugin.ZombieApocalypseMode || (firstTwoDigitsOfMapSeed <= Plugin.RandomChanceZombieApocalypse && Plugin.RandomChanceZombieApocalypse >= 0))
            {
                logger.LogInfo("ZOMBIE APOCALYPSE");
                ___currentLevel.enemySpawnChanceThroughoutDay = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                {
                    new Keyframe(0f, Plugin.InsideEnemySpawnCurve),
                    new Keyframe(0.5f, Plugin.MiddayInsideEnemySpawnCurve)
                });
                ___currentLevel.daytimeEnemySpawnChanceThroughDay = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                {
                    new Keyframe(0f, 7f),
                    new Keyframe(0.5f, 7f)
                });
                ___currentLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
                {
                    new Keyframe(0f, Plugin.StartOutsideEnemySpawnCurve),
                    new Keyframe(20f,Plugin.MidOutsideEnemySpawnCurve),
                    new Keyframe(21f, Plugin.EndOutsideEnemySpawnCurve)
                });
                ___currentLevel.DaytimeEnemies.Clear();
                ___currentLevel.OutsideEnemies.Clear();

                maskedEnemy.rarity = 1000000000;
                foreach (var enemy in ___currentLevel.Enemies)
                {
                    enemy.rarity = 0;
                }
                if (Plugin.CanSpawnOutside)
                {
                    maskedEnemy.enemyType.isOutsideEnemy = true;
                    ___currentLevel.OutsideEnemies.Add(maskedEnemy);
                    ___currentLevel.DaytimeEnemies.Add(maskedEnemy);
                }
            } else
            {
                logger.LogInfo("no zombies :(");
            }
            ___currentLevel.Enemies.Add(maskedEnemy);
        }
    }
}
