 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class PlayerESP : MonoBehaviour
    {
        public static bool bDrawPlayers = false;
        public static bool bDrawHP = true;
        public static bool bDrawEquipItem = true;
        public static bool bDrawSleepers = true;
        public static bool bEnableChams = false;
        public static bool bFlatChams = false;
        public static int iESPDrawDistance = 5000;
        public static Color cSleepingColor = new Color(0f, 0f, 1f);
        public static Color cDeadColor = new Color(0f, 0.588f, 0.588f);

        private void OnGUI()
        {
            Draw();
        }

        public static void Draw()
        {
            if (bDrawPlayers)
            {
                foreach (BasePlayer player in BasePlayer.VisiblePlayerList)
                {
                    //Debug.LogError("1: " + player.displayName);
                    if ((player != null) && !player.IsLocalPlayer() && !player.IsAdmin)
                    {
                        //Debug.LogError("2: " + player.displayName);
                        Vector3 position = player.transform.position;
                        Vector3 vector3 = MainCamera.mainCamera.WorldToScreenPoint(position);
                        if (vector3.z > 0f)
                        {
                            //Debug.LogError("3: " + player.displayName);
                            int distance = (int)Vector3.Distance(LocalPlayer.Entity.transform.position, position);
                            if (distance <= iESPDrawDistance)
                            {
                                //Debug.LogError("4: " + player.displayName);
                                Color color;
                                if (!player.IsSleeping() && player.IsAlive() && !player.HasPlayerFlag(BasePlayer.PlayerFlags.IsAdmin))
                                {
                                    //Debug.LogError("5: " + player.displayName);
                                    Vector3 vector2 =
                                        MainCamera.mainCamera.WorldToScreenPoint(position + new Vector3(0f, 1.7f, 0f));
                                    float y = Mathf.Abs((float)(vector3.y - vector2.y));
                                    float x = y / 2f;
                                    color = Color.red;
                                    if (bDrawHP)
                                    {
                                        Renderer.DrawHealth(new Vector2(vector2.x, Screen.height - vector2.y),
                                            player.health, true);
                                    }
                                    Renderer.DrawBox(new Vector2(vector2.x - (x / 2f), Screen.height - vector2.y),
                                                      new Vector2(x, y), 1f, color, player.IsDucked());
                                    if (bDrawEquipItem)
                                    {
                                        Item activeItem = player.Belt.GetActiveItem();
                                        HeldEntity heldEntity = player.GetHeldEntity();

                                        if (activeItem != null)
                                        {
                                            if (heldEntity != null && heldEntity.GetItem() != null && heldEntity
                                                    .GetItem()
                                                    .info.shortname.Equals(activeItem.info.shortname))
                                            {
                                                Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                                    string.Format("{0}\n[{1}m]\n{2}HP\n[{3}]", player.displayName,
                                                                  distance, (int)player.health, activeItem.info.displayName.english),
                                                    color, true, 12, true, 4);
                                            }
                                            else
                                                Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                                    string.Format("{0}\n[{1}m]\n{2}HP\n{3}", player.displayName,
                                                        distance,
                                                        (int)player.health, activeItem.info.displayName.english), color, true,
                                                    12,
                                                    true, 4);
                                        }
                                        else
                                        {
                                            Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                                string.Format("{0}\n[{1}m]\n{2}HP", player.displayName, distance,
                                                    (int)player.health), color, true, 12, true, 3);
                                        }
                                    }
                                    else
                                    {
                                        Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                            string.Format("{0}\n[{1}m]\n{2}HP", player.displayName, distance,
                                                (int)player.health), color, true, 12, true, 3);
                                    }
                                }
                                else if (bDrawSleepers && player.health > 0f)
                                {
                                    color = cSleepingColor;
                                    Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                        string.Format("{0}\n[{1}m]\n{2}HP", player.displayName, distance,
                                            (int)player.health), color, true, 12, true, 3);
                                }
                                else if (player.IsDead())
                                {
                                    color = cDeadColor;
                                    Renderer.DrawString(new Vector2(vector3.x, Screen.height - vector3.y),
                                        string.Format("{0}\n[{1}m]\nDEAD", player.displayName, distance), color, true,
                                        12, true, 3);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
