using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    public class Renderer
    {
        private static Color color_0;
        private static GUIStyle guistyle_0 = new GUIStyle(GUI.skin.label);
        private static Texture2D texture2D_0 = new Texture2D(1, 1);

        public static void BoxRect(Rect rect, Color color)
        {
            if (color != color_0)
            {
                texture2D_0.SetPixel(0, 0, color);
                texture2D_0.Apply();
                color_0 = color;
            }
            GUI.DrawTexture(rect, texture2D_0);
        }

        public static void DrawBox(Vector2 pos, Vector2 size, float thick, Color color, bool ducked = false)
        {
            if (ducked)
            {
                size.y = size.y * 0.611f;
                pos.y = pos.y - 1.5f;
            }

            BoxRect(new Rect(pos.x, pos.y, size.x, thick), color);
            BoxRect(new Rect(pos.x, pos.y, thick, size.y), color);
            BoxRect(new Rect(pos.x + size.x, pos.y, thick, size.y), color);
            BoxRect(new Rect(pos.x, pos.y + size.y, size.x + thick, thick), color);
        }

        public static void DrawHealth(Vector2 pos, float health, bool center = false)
        {
            if (center)
            {
                pos -= new Vector2(26f, 0f);
            }
            pos -= new Vector2(0f, 8f);
            BoxRect(new Rect(pos.x, pos.y, 52f, 5f), Color.black);
            pos += new Vector2(1f, 1f);
            Color green = Color.green;
            if (health <= 60f)
            {
                green = Color.yellow;
            }
            if (health <= 25f)
            {
                green = Color.red;
            }
            BoxRect(new Rect(pos.x, pos.y, 0.5f * health, 3f), green);
        }

        public static void DrawString(Vector2 pos, string text, Color color, bool center = true, int size = 12,
            bool outline = false, int lines = 1)
        {
            guistyle_0.fontSize = size;
            guistyle_0.normal.textColor = color;
            GUIContent content = new GUIContent(text);
            if (center)
            {
                pos.x -= guistyle_0.CalcSize(content).x / 2f;
            }
            Rect rect = new Rect(pos.x, pos.y, 300f, lines * 25f);
            if (!outline)
            {
                GUI.Label(rect, content, guistyle_0);
            }
            else
            {
                DrawOutlinedString(rect, text, color);
            }
        }

        public static void DrawWatermark(string text, Color color, int size = 12)
        {
            guistyle_0.fontSize = size;
            guistyle_0.normal.textColor = color;
            GUIContent content = new GUIContent(text);
            GUI.Label(new Rect(1, 1, 300f, 25f), content, guistyle_0);
        }

        public static void DrawWatermark2(string text, Color color, int size = 12)
        {
            guistyle_0.fontSize = size;
            guistyle_0.normal.textColor = color;
            GUIContent content = new GUIContent(text);
            GUI.Label(new Rect(250, 250, 300f, 25f), content, guistyle_0);
        }

        public static void DrawOutlinedString(Rect rect, string text, Color color)
        {
            GUIStyle backupStyle = guistyle_0;
            guistyle_0.normal.textColor = Color.black;
            rect.x--;
            GUI.Label(rect, text, guistyle_0);
            rect.x += 2;
            GUI.Label(rect, text, guistyle_0);
            rect.x--;
            rect.y--;
            GUI.Label(rect, text, guistyle_0);
            rect.y += 2;
            GUI.Label(rect, text, guistyle_0);
            rect.y--;
            guistyle_0.normal.textColor = color;
            GUI.Label(rect, text, guistyle_0);
            guistyle_0 = backupStyle;
        }
    }
}
