using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class ObjectESP : MonoBehaviour
    {
        public static bool bDrawResources = false;
        public static bool bDrawCollectibles = false;
        public static int iDrawDistanceLoot = 150;
        public static Color cResourceColor = new Color(1f, 1f, 0f);
        public static Color cCollectibleColor = new Color(0.515625f, 0.46875f, 0.91015625f);

        private void OnGUI()
        {
            try
            {
                if (bDrawResources || bDrawCollectibles)
                {
                    foreach (BaseNetworkable NetworkableObject in BaseNetworkable.clientEntities)
                    {
                        if (bDrawResources)
                        {
                            if (NetworkableObject is ResourceEntity)
                            {
                                ResourceEntity resource = NetworkableObject as ResourceEntity;

                                if (resource != null)
                                {
                                    if (resource.ShortPrefabName.Contains("ore"))
                                    {
                                        Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(resource.transform.position);
                                        if (vector.z > 0f)
                                        {
                                            int distance = (int)Vector3.Distance(LocalPlayer.Entity.transform.position,
                                                resource.transform.position);
                                            if (distance <= iDrawDistanceLoot)
                                            {
                                                vector.x += 3f;
                                                vector.y = Screen.height - (vector.y + 1f);
                                                Renderer.DrawString(new Vector2(vector.x, vector.y),
                                                    string.Format("{0} [{1}m]",
                                                        resource.ShortPrefabName.Replace(".prefab", "")
                                                            .Replace("_deployed", ""), distance),
                                                    cResourceColor, true, 12, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (bDrawCollectibles)
                        {
                            if (NetworkableObject is CollectibleEntity)
                            {
                                CollectibleEntity collectible = NetworkableObject as CollectibleEntity;

                                if (collectible != null)
                                {
                                    Vector3 vector = MainCamera.mainCamera.WorldToScreenPoint(collectible.transform.position);
                                    if (vector.z > 0f)
                                    {
                                        int distance = (int)Vector3.Distance(LocalPlayer.Entity.transform.position,
                                            collectible.transform.position);
                                        if (distance <= iDrawDistanceLoot)
                                        {
                                            vector.x += 3f;
                                            vector.y = Screen.height - (vector.y + 1f);
                                            Renderer.DrawString(new Vector2(vector.x, vector.y),
                                                //string.Format("{0} [{1}m]",
                                                //	collectible.ShortPrefabName.Replace(".prefab", "")
                                                //		.Replace("_deployed", ""), distance),
                                                string.Format("{0} [{1}m]", collectible.itemName.english, distance),
                                                cCollectibleColor, true, 12, true);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
