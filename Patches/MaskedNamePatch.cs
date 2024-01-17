using GameNetcodeStuff;
using MaskedEnemyRework.External_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using BepInEx.Logging;
using System.ComponentModel;

namespace MaskedEnemyRework.Patches
{
    internal class MaskedNamePatch
    {
        public static void UpdateNameBillboard(MaskedPlayerEnemy masked)
        {
            if (masked.gameObject.TryGetComponent(out MaskedNameBillboard maskedBillboard))
            {
                Canvas usernameCanvas = maskedBillboard.usernameCanvas;
                CanvasGroup canvasAlpha = GetCanvasGroup(usernameCanvas);
                Transform maskedUsernameTransform = usernameCanvas.transform;
                TextMeshProUGUI usernameBillboardText = maskedBillboard.usernameText;
                if (usernameCanvas && maskedUsernameTransform && canvasAlpha)
                {
                    if (!usernameBillboardText.gameObject.activeInHierarchy || !usernameBillboardText.canvas)
                    {
                        usernameBillboardText.transform.SetParent(maskedUsernameTransform, false);
                    }
                    if (canvasAlpha.alpha >= 0f && GameNetworkManager.Instance.localPlayerController != null)
                    {
                        canvasAlpha.alpha -= Time.deltaTime;
                        Vector3 position2 = default;
                        position2.Set(masked.transform.position.x, masked.transform.position.y + 2.64f, masked.transform.position.z);
                        maskedUsernameTransform.SetPositionAndRotation(position2, maskedUsernameTransform.rotation);
                        maskedUsernameTransform.LookAt(GameNetworkManager.Instance.localPlayerController.localVisorTargetPoint);
                    }
                    else if (usernameCanvas.gameObject.activeSelf)
                    {
                        usernameCanvas.gameObject.SetActive(false);
                    }
                }
            }
            else if (masked.mimickingPlayer != null)
            {
                SetNameBillboard(masked);
            }
        }

        public static void ToggleName(MaskedPlayerEnemy masked, bool TurnOn) 
        {
            if(masked == null) return;
            if (masked.gameObject.TryGetComponent(out MaskedNameBillboard nameCanvas))
            {
                nameCanvas.usernameCanvas.gameObject.SetActive(TurnOn);

                if(TurnOn)
                {
                    GetCanvasGroup(nameCanvas.usernameCanvas).alpha = 1f;
                } 
            }
        }

        public static CanvasGroup GetCanvasGroup(Canvas canvas)
        {
            CanvasGroup cg = null;
            if (canvas != null)
            {
                cg = canvas.gameObject.GetComponent<CanvasGroup>();
                if(cg == null)
                {
                    cg = canvas.gameObject.AddComponent<CanvasGroup>();              
                }
            }

            return cg;
        }

        public static void SetNameBillboard(MaskedPlayerEnemy masked)
        {
            if (masked.mimickingPlayer == null)
            {
                return;
            }
            PlayerControllerB mimickingPlayer = masked.mimickingPlayer;
            if (!masked.gameObject.TryGetComponent<MaskedNameBillboard>(out var maskedNameBillboard))
            {
                maskedNameBillboard = masked.gameObject.AddComponent<MaskedNameBillboard>();
            }
            maskedNameBillboard.usernameCanvas = mimickingPlayer.usernameCanvas;
            // maskedNameBillboard.usernameCanvas.gameObject.AddComponent<CanvasGroup>();
            GetCanvasGroup(maskedNameBillboard.usernameCanvas);        
            GameObject gameObject = new("usernameText");
            gameObject.transform.SetParent(maskedNameBillboard.usernameCanvas.transform, worldPositionStays: false);
            maskedNameBillboard.usernameText = gameObject.AddComponent<TextMeshProUGUI>();
            maskedNameBillboard.usernameText.transform.SetParent(maskedNameBillboard.usernameCanvas.transform, false);
            maskedNameBillboard.usernameText.text = mimickingPlayer.usernameBillboardText.text;
            maskedNameBillboard.usernameText.font = mimickingPlayer.usernameBillboardText.font;
            maskedNameBillboard.usernameText.color = mimickingPlayer.usernameBillboardText.color;
            maskedNameBillboard.usernameText.fontSize = mimickingPlayer.usernameBillboardText.fontSize;
            maskedNameBillboard.usernameText.enableAutoSizing = mimickingPlayer.usernameBillboardText.enableAutoSizing;
            maskedNameBillboard.usernameText.extraPadding = mimickingPlayer.usernameBillboardText.extraPadding;
            maskedNameBillboard.usernameText.alignment = mimickingPlayer.usernameBillboardText.alignment;
            maskedNameBillboard.usernameText.textStyle = mimickingPlayer.usernameBillboardText.textStyle;
            maskedNameBillboard.usernameText.textPreprocessor = mimickingPlayer.usernameBillboardText.textPreprocessor;
            maskedNameBillboard.usernameText.characterSpacing = mimickingPlayer.usernameBillboardText.characterSpacing;
            maskedNameBillboard.usernameText.wordSpacing = mimickingPlayer.usernameBillboardText.wordSpacing;
            maskedNameBillboard.usernameText.lineSpacing = mimickingPlayer.usernameBillboardText.lineSpacing;
            maskedNameBillboard.usernameText.margin = mimickingPlayer.usernameBillboardText.margin;
            maskedNameBillboard.usernameText.overflowMode = mimickingPlayer.usernameBillboardText.overflowMode;
            maskedNameBillboard.usernameText.fontSharedMaterial = mimickingPlayer.usernameBillboardText.fontSharedMaterial;
            maskedNameBillboard.usernameText.fontSizeMin = mimickingPlayer.usernameBillboardText.fontSizeMin;
            maskedNameBillboard.usernameText.fontSizeMax = mimickingPlayer.usernameBillboardText.fontSizeMax;
            maskedNameBillboard.usernameText.enableKerning = mimickingPlayer.usernameBillboardText.enableKerning;
            RectTransform playerTransform = mimickingPlayer.usernameBillboardText.GetComponent<RectTransform>();
            RectTransform maskedTransform = maskedNameBillboard.usernameText.GetComponent<RectTransform>();
            maskedTransform.localPosition = playerTransform.localPosition;
            maskedTransform.sizeDelta = playerTransform.sizeDelta;
            maskedTransform.anchorMin = playerTransform.anchorMin;
            maskedTransform.anchorMax = playerTransform.anchorMax;
            maskedTransform.pivot = playerTransform.pivot;
            maskedTransform.anchoredPosition = playerTransform.anchoredPosition;
            maskedTransform.localScale = playerTransform.localScale;
            maskedTransform.offsetMin = playerTransform.offsetMin;
            maskedTransform.offsetMax = playerTransform.offsetMax;
            maskedTransform.right = playerTransform.right;
            maskedNameBillboard.usernameText.gameObject.SetActive(true);
            maskedNameBillboard.usernameText.enabled = true;
            maskedNameBillboard.usernameCanvas.enabled = true;
        }
    }
}
