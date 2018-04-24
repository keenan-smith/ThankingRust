using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class Aimbot : MonoBehaviour
    {
        public static bool bNoRecoil = false;
        public static bool bNoSway = false;
        public static bool bAimbotEnabled = false;
        public static bool bAimAtHead = false;
        public static bool bPredictVelocity = true;
        public static bool bPredictDrop = true;
        public static bool bForceAutomatic = false;

        public static bool ShouldAim = false;
        
        public static KeyCode kAimKey = KeyCode.F;
        public static float fFOV = 200f;

        public static List<float> velocities = new List<float>();
        public static List<string> projectiles = new List<string>();

        public static BasePlayer aimPlayer;

        public static Vector3 playerVelocity = Vector3.zero;
        
        public static void NoRecoil()
        {
            if (bNoRecoil || bNoSway || bForceAutomatic)
            {
                foreach (BaseProjectile projectile in UpdateObjects.baseProjectileArray)
                {
                    if (projectile != null && projectile.recoil != null)
                    {
                        velocities.Clear();
                        projectiles.Clear();
                        projectiles.Add(projectile.ShortPrefabName);
                        ItemModProjectile component = projectile.primaryMagazine.ammoType
                            .GetComponent<ItemModProjectile>();
                        float velocity = component.GetAverageVelocity() * projectile.projectileVelocityScale;
                        velocities.Add(velocity);
                        if (bNoRecoil)
                        {
                            projectile.recoil.recoilYawMax = 0f;
                            projectile.recoil.recoilYawMin = 0f;
                            projectile.recoil.recoilPitchMax = 0f;
                            projectile.recoil.recoilPitchMin = 0f;
                        }
                        if (bNoSway)
                        {
                            projectile.aimSway = 0f;
                            projectile.aimSwaySpeed = 0f;
                        }
                        if (bForceAutomatic)
                            projectile.automatic = true;
                    }
                }
            }
        }

        public static void DoAimbot()
        {
            Vector3 vector = GetEnemyVector();
            if (vector != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(vector, LocalPlayer.Entity.eyes.transform.up);
                LocalPlayer.Entity.input.SetViewVars(BaseMountable.ConvertVector(rotation.eulerAngles));
            }
        }

        public static Vector3 GetEnemyVector()
        {
            Vector3 result = Vector3.zero;
            Vector2 middleScreen = new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2));
            float maxDistance = 4999f;
            foreach (BasePlayer player in BasePlayer.VisiblePlayerList)
            {
                if (((player != null) && (player.health > 0f)) && (!player.IsSleeping() && !player.IsLocalPlayer()))
                {
                    Vector3 enemyPosition;
                    if (bAimAtHead)
                        enemyPosition = GetBonePosition(player.GetModel(), "head");
                    else
                        enemyPosition = GetBonePosition(player.GetModel(), "penis");

                    if (enemyPosition != Vector3.zero)
                    {
                        bool flag = IsVisible(enemyPosition);

                        if (flag)
                        {
                            Vector3 vector4 = MainCamera.mainCamera.WorldToScreenPoint(enemyPosition);
                            Vector2 enemyOnScreen = new Vector2(vector4.x, Screen.height - vector4.y);                            
                            float fov = Mathf.Abs(Vector2.Distance(middleScreen, enemyOnScreen));
                            if ((fov <= fFOV) && (fov <= maxDistance))
                            {
                                if (vector4.z > 0) 
                                {
                                    result = enemyPosition;
                                    aimPlayer = player;
                                    
                                    if (bAimAtHead)
                                        enemyPosition = GetBonePosition(aimPlayer.GetModel(), "head");
                                    else
                                        enemyPosition = GetBonePosition(aimPlayer.GetModel(), "penis");
                                    
                                    if (bPredictVelocity)
                                       return playerVelocity;

                                    return enemyPosition;
                                }
                            maxDistance = fov;
                            }
                        }
                    }
                }
            }

            return result; 
        }


        public void FixedUpdate()
        {
            if (LocalPlayer.Entity == null || aimPlayer == null)
                return;
                
            Vector3 enemyPosition;
            if (bAimAtHead)
                enemyPosition = GetBonePosition(aimPlayer.GetModel(), "head");
            else
                enemyPosition = GetBonePosition(aimPlayer.GetModel(), "penis");
            
            Item activeItem = LocalPlayer.Entity.Belt.GetActiveItem();
            if (activeItem != null && (activeItem.info.shortname.Contains("bow") ||
                                       activeItem.info.shortname.Contains("smg.") ||
                                       activeItem.info.shortname.Contains("pistol.") ||
                                       activeItem.info.shortname.Contains("lmg.") ||
                                       activeItem.info.shortname.Contains("spear.") ||
                                       activeItem.info.shortname.Contains("rifle")))
            {
                if (bPredictDrop || bPredictVelocity)
                {
                    float bulletSpeed = 250f;
                    switch (activeItem.info.shortname)
                    {
                        case "rifle.bolt":
                            bulletSpeed = 656.25f;
                            break;
                        case "rifle.ak":
                            bulletSpeed = 375f;
                            break;
                        case "rifle.lr300":
                            bulletSpeed = 375f;
                            break;
                        case "rifle.semiauto":
                            bulletSpeed = 375f;
                            break;
                        case "smg.mp5":
                            bulletSpeed = 180f;
                            break;
                        case "smg.thompson":
                            bulletSpeed = 300f;
                            break;
                        case "smg.2":
                            bulletSpeed = 240f;
                            break;
                        case "pistol.m92":
                            bulletSpeed = 300f;
                            break;
                        case "pistol.semiauto":
                            bulletSpeed = 300f;
                            break;
                        case "pistol.python":
                            bulletSpeed = 300f;
                            break;
                        case "pistol.nailgun":
                            bulletSpeed = 50f;
                            break;
                        case "bow.hunting":
                            bulletSpeed = 50f;
                            break;
                        case "crossbow":
                            bulletSpeed = 75f;
                            break;
                        case "spear.wooden":
                            bulletSpeed = 25f;
                            break;
                        case "rock":
                            bulletSpeed = 50f;
                            break;
                    }

                    if (bPredictVelocity)
                    {

                        int numSolutions = 0;

                        Vector3 projPos = LocalPlayer.Entity.eyes.position;

                        Vector3 vel = (Vector3) aimPlayer.playerModel.GetFieldValue("velocity");
                        Vector3[] solutions = new Vector3[2];

                        if (vel.sqrMagnitude > 0)
                            numSolutions = fts.solve_ballistic_arc(projPos, bulletSpeed, enemyPosition, vel, 9.81f,
                                out solutions[0], out solutions[1]);
                        else
                            numSolutions = fts.solve_ballistic_arc(projPos, bulletSpeed, enemyPosition, 9.81f,
                                out solutions[0], out solutions[1]);

                        if (numSolutions > 0)
                            playerVelocity = solutions[0];
                    }
                }
            }
        }


        public static bool IsVisible(Vector3 vector3_0)
        {
            Vector3 vector = MainCamera.mainCamera.transform.position - vector3_0;
            float magnitude = vector.magnitude;
            if (magnitude < Mathf.Epsilon)
                return true;
            Vector3 direction = (Vector3)(vector / magnitude);
            Vector3 vector3 = (Vector3)(direction * Mathf.Min(magnitude, 0.01f));
            return LocalPlayer.Entity.IsVisible(new Ray(vector3_0 + vector3, direction), magnitude);
        }

        public static Vector3 GetBonePosition(Model playerModel, string boneName)
        {
            if (playerModel == null)
                return Vector3.zero;
            
            return playerModel.FindBone(boneName).position;
        }

        private void OnGUI()
        {
            foreach (BaseNetworkable NetworkableObject in BaseNetworkable.clientEntities)
            {
                if (NetworkableObject is OreResourceEntity)
                {
                    ResourceEntity resource = NetworkableObject as OreResourceEntity;

                    if (resource != null)
                    {
                        Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(resource.transform.position);
                        if (vector.z > 0f)
                        {
                            int distance = (int)Vector3.Distance(LocalPlayer.Entity.transform.position,
                                resource.transform.position);
                            if (distance <= 5000)
                            {
                                vector.x += 3f;
                                vector.y = Screen.height - (vector.y + 1f);
                                Renderer.DrawString(new Vector2(vector.x, vector.y),
                                    string.Format("{0} [{1}m]",
                                        resource.ShortPrefabName
                                            .Replace(".prefab", "")
                                            .Replace("_deployed", "")
                                        , distance),
                                    Color.green, true, 12, true);
                            }
                        }
                    }
                }
            }
            
            if (bAimbotEnabled && LocalPlayer.Entity != null && Input.GetKey(kAimKey))
                DoAimbot();
        }
    }
}
