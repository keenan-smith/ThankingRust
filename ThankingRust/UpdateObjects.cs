using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class UpdateObjects : MonoBehaviour
    {
        public static List<StorageContainer> storageContainerArray = new List<StorageContainer>();
        public static List<ResourceEntity> baseResourceArray = new List<ResourceEntity>();
        public static List<CollectibleEntity> collectibleArray = new List<CollectibleEntity>();
        public static List<BaseNpc> baseNPCArray = new List<BaseNpc>();
        public static List<BuildingPrivlidge> cupboardArray = new List<BuildingPrivlidge>();
        public static List<Door> doorArray = new List<Door>();
        public static List<LootableCorpse> corpseArray = new List<LootableCorpse>();
        public static List<BaseHelicopter> heliArray = new List<BaseHelicopter>();
        public static List<WorldItem> worldItemArray = new List<WorldItem>();
        public static List<BaseProjectile> baseProjectileArray = new List<BaseProjectile>();
        public static List<AttackEntity> attackEntityArray = new List<AttackEntity>();
        public static List<PlayerWalkMovement> playerWalkMovement = new List<PlayerWalkMovement>();

        private void Start()
        {
            base.StartCoroutine(this.updateObjects());
        }

        private IEnumerator updateObjects()
        {
            while (true)
            {
                baseProjectileArray.Clear();

                foreach (BaseNetworkable NetworkableObject in BaseNetworkable.clientEntities)
                {
                    if (NetworkableObject is BaseProjectile)
                    {
                        baseProjectileArray.Add((BaseProjectile)NetworkableObject);
                    }

                    //if (NetworkableObject is ResourceEntity)
                    //{
                    //    baseResourceArray.Add((ResourceEntity)NetworkableObject);
                    //}
                }

                yield return new WaitForSeconds(1);
            }
        }

        //private IEnumerator updateObjects()
        //{
        //    while (true)
        //    {
        //        //if (Config.ESP.shouldDrawStorage)
        //        //{
        //        //    try
        //        //    {
        //        //        storageContainerArray = FindObjectsOfType<StorageContainer>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}


        //        //if (Config.Misc.ClimbHack)
        //        //{
        //        //    try
        //        //    {
        //        //        playerWalkMovement = FindObjectOfType<PlayerWalkMovement>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //try
        //        //{
        //        //    if (Config.Misc.FastGathering)
        //        //    {
        //        //        attackEntityArray = FindObjectsOfType<AttackEntity>();
        //        //        Misc.FastGathering();
        //        //    }
        //        //    else
        //        //    {
        //        //        Misc.FastGathering();
        //        //    }
        //        //}
        //        //catch
        //        //{
        //        //}
        //        //yield return new WaitForSeconds(1f);

        //        if (Aimbot.bNoRecoil || Aimbot.bNoSway || Aimbot.bForceAutomatic)
        //        {
        //            try
        //            {
        //                baseProjectileArray = FindObjectsOfType<BaseProjectile>();
        //                Aimbot.NoRecoil();
        //            }
        //            catch
        //            {
        //            }
        //            yield return new WaitForSeconds(1f);
        //        }

        //        //if (Config.ESP.shouldDrawResources)
        //        //{
        //        //    try
        //        //    {
        //        //        baseResourceArray = FindObjectsOfType<ResourceEntity>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawAnimals)
        //        //{
        //        //    try
        //        //    {
        //        //        baseNPCArray = FindObjectsOfType<BaseNpc>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawCollectible)
        //        //{
        //        //    try
        //        //    {
        //        //        collectibleArray = FindObjectsOfType<CollectibleEntity>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawCupboards)
        //        //{
        //        //    try
        //        //    {
        //        //        cupboardArray = FindObjectsOfType<BuildingPrivlidge>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawDoors)
        //        //{
        //        //    try
        //        //    {
        //        //        doorArray = FindObjectsOfType<Door>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawCorpses)
        //        //{
        //        //    try
        //        //    {
        //        //        corpseArray = FindObjectsOfType<LootableCorpse>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawHeli)
        //        //{
        //        //    try
        //        //    {
        //        //        heliArray = FindObjectsOfType<BaseHelicopter>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}

        //        //if (Config.ESP.shouldDrawWorldItems)
        //        //{
        //        //    try
        //        //    {
        //        //        worldItemArray = FindObjectsOfType<WorldItem>();
        //        //    }
        //        //    catch
        //        //    {
        //        //    }
        //        //    yield return new WaitForSeconds(1f);
        //        //}
        //        yield return new WaitForSeconds(1f);
        //    }
        //    yield break;
        //}
    }
}
