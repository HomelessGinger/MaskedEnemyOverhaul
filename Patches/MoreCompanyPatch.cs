using BepInEx.Bootstrap;
using HarmonyLib;
using MoreCompany.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaskedEnemyRework.Patches
{
    internal class MoreCompanyPatch
    {
        public static void ApplyCosmetics(MaskedPlayerEnemy masked)
        {
            
            Transform cosmeticRoot = masked.transform.Find("ScavengerModel").Find("metarig");
            CosmeticApplication cosmeticApplication = cosmeticRoot.GetComponent<CosmeticApplication>();
            if (cosmeticApplication)
            {
                cosmeticApplication.ClearCosmetics();
                GameObject.Destroy(cosmeticApplication);
            }


            if (!MoreCompany.MainClass.showCosmetics)
                return;


            List<string> playerCosmetics = MoreCompany.MainClass.playerIdsAndCosmetics[(int)masked.mimickingPlayer.playerClientId];
            cosmeticApplication = cosmeticRoot.gameObject.AddComponent<CosmeticApplication>();
            foreach (var cosmetic in playerCosmetics)
            {
                cosmeticApplication.ApplyCosmetic(cosmetic, true);
            }

            foreach (var cosmetic in cosmeticApplication.spawnedCosmetics)
            {
                cosmetic.transform.localScale *= CosmeticRegistry.COSMETIC_PLAYER_SCALE_MULT;
            }
        }

    }
}
