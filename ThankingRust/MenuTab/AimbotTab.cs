using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public static class AimbotTab
    {
        public static void Tab()
        {
            Prefab.MenuArea(new Rect(0, 0, 466, 436), "AIMBOT", () =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(230));
                Prefab.Toggle("Aimbot Enabled", ref Aimbot.bAimbotEnabled);
                Prefab.Toggle("No Recoil", ref Aimbot.bNoRecoil);
                Prefab.Toggle("No Sway", ref Aimbot.bNoSway);
                Prefab.Toggle("Force Automatic", ref Aimbot.bForceAutomatic);
                Prefab.Toggle("Predict Drop", ref Aimbot.bPredictDrop);
                Prefab.Toggle("Predict Movement", ref Aimbot.bPredictVelocity);
                Prefab.Toggle("Aim at Head", ref Aimbot.bAimAtHead);
                GUILayout.Space(3);
                //if (AimbotOptions.Smooth)
                //{
                //    GUILayout.Label("Aim Speed: " + AimbotOptions.AimSpeed, Prefab._TextStyle);
                //    AimbotOptions.AimSpeed = (int)Prefab.Slider(1, AimbotOptions.MaxSpeed, AimbotOptions.AimSpeed, 200);
                //}

                GUILayout.Label("FOV: " + Aimbot.fFOV, Prefab._TextStyle);
                Aimbot.fFOV = (int)Prefab.Slider(1, 300, Aimbot.fFOV, 200);
                //GUILayout.Label("Distance: " + AimbotOptions.Distance, Prefab._TextStyle);
                //AimbotOptions.Distance = (int)Prefab.Slider(50, 1000, AimbotOptions.Distance, 200);
                //GUIContent[] TargetMode = {
                //    new GUIContent("Distance"),
                //    new GUIContent("FOV")
                //};

                //if (Prefab.List(200, "_TargetMode", new GUIContent("Target Mode: " + TargetMode[DropDown.Get("_TargetMode").ListIndex].text), TargetMode))
                //{
                //    AimbotOptions.TargetMode = (TargetMode)DropDown.Get("_TargetMode").ListIndex;
                //}
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            });
        }
    }
}
