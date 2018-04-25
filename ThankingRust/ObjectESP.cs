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
        public static int iDrawDistanceLoot = 150;
        public static Color cResourceColor = new Color(1f, 1f, 0f);

        private void OnGUI()
        {
            try
            {
                if (bDrawResources)
                {
                    foreach (BaseNetworkable NetworkableObject in BaseNetworkable.clientEntities)
                    {
                        if (NetworkableObject is ResourceEntity)
                        {
                            ResourceEntity resource = NetworkableObject as ResourceEntity;

                            if (resource != null)
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
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
