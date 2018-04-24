using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace ThankingRust
{
    public class Main : MonoBehaviour
    {
        public static Main Instance;
        
        public static bool InMenu = false;
        public static bool Unloading = true;

        public static Rect MainRect = new Rect(50f, 100f, 300f, 500f);

        private void Start()
        {
            Instance = this;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                InMenu = !InMenu;
            }
        }

        private void OnGUI()
        {
            if (InMenu)
            {
                MainRect = GUI.Window(0, MainRect, new GUI.WindowFunction(DrawMenu), "Thanking");
            }
        }

        public static void DrawMenu(int id)
        {
            GUILayout.BeginVertical();
            //GUILayout.Label("Player ESP");
            PlayerESP.bDrawPlayers = GUILayout.Toggle(PlayerESP.bDrawPlayers, "Player ESP");
            PlayerESP.bDrawSleepers = GUILayout.Toggle(PlayerESP.bDrawSleepers, "Sleeper ESP");
            PlayerESP.bDrawHP = GUILayout.Toggle(PlayerESP.bDrawHP, "Health ESP");
            PlayerESP.bDrawEquipItem = GUILayout.Toggle(PlayerESP.bDrawEquipItem, "Equipped Items");

            GUILayout.Label(string.Format("ESP Distance: {0}", PlayerESP.iESPDrawDistance));
            PlayerESP.iESPDrawDistance = (int)GUILayout.HorizontalSlider(PlayerESP.iESPDrawDistance, 0f, 5000f);

            Aimbot.bAimbotEnabled = GUILayout.Toggle(Aimbot.bAimbotEnabled, "Aimbot");
            Aimbot.bNoSway = GUILayout.Toggle(Aimbot.bNoSway, "No Sway");
            Aimbot.bNoRecoil = GUILayout.Toggle(Aimbot.bNoRecoil, "No Recoil");
            Aimbot.bForceAutomatic = GUILayout.Toggle(Aimbot.bForceAutomatic, "Force Automatic");
            Aimbot.bPredictDrop = GUILayout.Toggle(Aimbot.bPredictDrop, "Predict Drop");
            Aimbot.bPredictVelocity = GUILayout.Toggle(Aimbot.bPredictVelocity, "Predict Velocity");
            Aimbot.bAimAtHead = GUILayout.Toggle(Aimbot.bAimAtHead, "Aim at Head");

            GUILayout.Label(string.Format("Aimbot FOV: {0}", Aimbot.fFOV));
            Aimbot.fFOV = (int)GUILayout.HorizontalSlider(Aimbot.fFOV, 0f, 500f);
            
            if (GUILayout.Button("Unload"))
                Loading.Unload();
            
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
