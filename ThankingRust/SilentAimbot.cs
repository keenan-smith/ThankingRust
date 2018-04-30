using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class SilentAimbot
    {
        public static List<Projectile> createdProjectiles = new List<Projectile>();
        public static List<Projectile> aimbotProjectiles = new List<Projectile>();

        public static void Shoot(BasePlayer ownerPlayer, BaseProjectile baseProjectile)
        {
            if (HasAttackCooldown(baseProjectile))
            {
                return;
            }

            if (baseProjectile.primaryMagazine.contents <= 0)
            {
                baseProjectile.DryFire();
                return;
            }


            baseProjectile.primaryMagazine.contents--;
            //baseProjectile.OnSignal(BaseEntity.Signal.Attack, string.Empty);
            LaunchProjectile(ownerPlayer, baseProjectile);
            UpdateAmmoDisplay(baseProjectile);
            //ShotFired(baseProjectile);
            baseProjectile.BeginCycle();
        }

        public static void ShotFired (BaseProjectile baseProjectile)
        {
            baseProjectile.numShotsFired++;
            baseProjectile.SetFieldValue("lastShotTime", UnityEngine.Time.time);
        }

        public static bool HasAttackCooldown(BaseProjectile baseProjectile)
        {
            //Debug.LogError((float)baseProjectile.GetFieldValue("nextAttackTime"));
            return (Time.time < (float)baseProjectile.GetFieldValue("nextAttackTime"));
        }

        public static void UpdateAmmoDisplay(BaseProjectile baseProjectile)
        {
            typeof(BaseProjectile).GetMethod("UpdateAmmoDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(baseProjectile, null);
        }

        public static void LaunchProjectile(BasePlayer ownerPlayer, BaseProjectile baseProjectile)
        {
            ItemModProjectile component = baseProjectile.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
            if (!component)
            {
                Debug.LogError("NO ITEMMODPROJECTILE FOR AMMO: " + baseProjectile.primaryMagazine.ammoType.displayName.english);
                return;
            }
            LaunchProjectileClientside(ownerPlayer, baseProjectile, baseProjectile.primaryMagazine.ammoType, component.numProjectiles, 0/*baseProjectile.GetAimCone()*/);
        }

        public static void Launch(Projectile projectile)
        {
            typeof(Projectile).GetMethod("Launch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(projectile, null);
        }

        public static Projectile CreateProjectile(BaseProjectile baseProjectile, string prefabPath, Vector3 pos, Vector3 forward, Vector3 velocity)
        {
            GameObject gameObject = GameManager.client.CreatePrefab(prefabPath, pos, Quaternion.LookRotation(forward), false);
            if (gameObject == null)
            {
                return null;
            }
            if (ConVar.Pool.projectiles)
            {
                gameObject.EnablePooling(StringPool.Get(prefabPath));
            }
            gameObject.AwakeFromInstantiate();
            Projectile component = gameObject.GetComponent<Projectile>();
            component.InitializeVelocity(velocity);
            component.modifier = baseProjectile.GetProjectileModifier();
            return component;
        }

        public static void LaunchProjectileClientside(BasePlayer ownerPlayer, BaseProjectile baseProjectile, ItemDefinition ammo, int projectileCount, float projSpreadaimCone)
        {

            ItemModProjectile component = ammo.GetComponent<ItemModProjectile>();
            if (component == null)
            {
                Debug.Log("Ammo doesn't have a Projectile module!");
                return;
            }
            createdProjectiles.Clear();
            float num = ProjectileWeaponMod.Average(baseProjectile, (ProjectileWeaponMod x) => x.projectileVelocity, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
            float num2 = ProjectileWeaponMod.Sum(baseProjectile, (ProjectileWeaponMod x) => x.projectileVelocity, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
            using (ProjectileShoot projectileShoot = Facepunch.Pool.Get<ProjectileShoot>())
            {
                projectileShoot.projectiles = new List<ProjectileShoot.Projectile>();
                projectileShoot.ammoType = ammo.itemid;
                for (int i = 0; i < projectileCount; i++)
                {
                    Vector3 position = ownerPlayer.eyes.position;
                    Vector3 vector = ownerPlayer.eyes.BodyForward();
                    if (projSpreadaimCone > 0f || component.projectileSpread > 0f)
                    {
                        Quaternion rotation = ownerPlayer.eyes.rotation;
                        float num3 = baseProjectile.aimconeCurve.Evaluate(UnityEngine.Random.Range(0f, 1f));
                        float num4 = (projectileCount <= 1) ? component.GetSpreadScalar() : component.GetIndexedSpreadScalar(i, projectileCount);
                        float num5 = num3 * projSpreadaimCone + component.projectileSpread * num4;
                        vector = AimConeUtil.GetModifiedAimConeDirection(num5, rotation * Vector3.forward, projectileCount <= 1);
                        if (ConVar.Global.developer > 0)
                        {
                            UnityEngine.DDraw.Arrow(position, position + vector * 3f, 0.1f, Color.white, 20f);
                        }
                    }
                    Vector3 vector2 = vector * (component.GetRandomVelocity() * baseProjectile.projectileVelocityScale * num + num2);
                    int seed = ownerPlayer.NewProjectileSeed();
                    int projectileID = ownerPlayer.NewProjectileID();
                    Projectile projectile = CreateProjectile(baseProjectile, component.projectileObject.resourcePath, position, vector, vector2);
                    if (projectile != null)
                    {
                        projectile.mod = component;
                        projectile.seed = seed;
                        projectile.owner = ownerPlayer;
                        projectile.sourceWeaponPrefab = GameManager.client.FindPrefab(baseProjectile).GetComponent<AttackEntity>();
                        projectile.sourceProjectilePrefab = component.projectileObject.Get().GetComponent<Projectile>();
                        projectile.projectileID = projectileID;
                        projectile.invisible = baseProjectile.IsSilenced();
                        createdProjectiles.Add(projectile);
                        aimbotProjectiles.Add(projectile);
                    }
                    ProjectileShoot.Projectile projectile2 = new ProjectileShoot.Projectile();
                    projectile2.projectileID = projectileID;
                    projectile2.startPos = position;
                    projectile2.startVel = vector2;
                    projectile2.seed = seed;
                    projectileShoot.projectiles.Add(projectile2);
                }
                baseProjectile.ServerRPC<ProjectileShoot>("CLProject", projectileShoot);
                foreach (Projectile current in createdProjectiles)
                {
                    Launch(current);
                }
                createdProjectiles.Clear();
            }

        }

        public static void DoSilentAimbot()
        {
            if (LocalPlayer.Entity)
            {
                if (LocalPlayer.Entity.GetHeldEntity())
                {
                    BasePlayer ownerPlayer = LocalPlayer.Entity;
                    HeldEntity heldEntity = ownerPlayer.GetHeldEntity();

                    if (heldEntity is BaseProjectile)
                    {
                        BaseProjectile baseProjectile = heldEntity as BaseProjectile;

                        Shoot(ownerPlayer, baseProjectile);
                    }
                }
            }
        }
    }
}
