using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThankingRust
{
    public class Loading
    {
        public static GameObject HookObject;

        public static void Hook()
        {
            HookObject = new GameObject();
            HookObject.AddComponent<Main>();
            HookObject.AddComponent<PlayerESP>();
            HookObject.AddComponent<UpdateObjects>();
            HookObject.AddComponent<Aimbot>();
            UnityEngine.Object.DontDestroyOnLoad(HookObject);
        }

        public static void Unload()
        {
            UnityEngine.Object.DestroyImmediate(HookObject);
        }
    }
}
