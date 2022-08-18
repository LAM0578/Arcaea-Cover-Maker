using System.IO;
using System.Collections;
using System.Globalization;
using UnityEngine;
using Newtonsoft.Json;
using Maker.Project;

namespace Maker.Util
{
    public static class CoverMakerUntl
    {
        public static Color GetAvgColor(this Texture2D tex, float alpha)
        {
            var pixelCount = tex.width * tex.height;
            float r = 0, g = 0, b = 0;
            foreach (var p in tex.GetPixels())
            {
                if (p == new Color(0, 0, 0)) continue;
                r += p.r;
                g += p.g;
                b += p.b;
            }
            // Debug.Log($"{r}, {g}, {b}, {pixelCount}");
            r /= pixelCount;
            g /= pixelCount;
            b /= pixelCount;
            // Debug.Log($"result: {r}, {g}, {b}");
            return new Color(r, g, b, alpha);
        }

        public static Texture2D GetReadableTexture(this Texture2D tex)
        {
            // https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear
            );
            Graphics.Blit(tex, renderTexture);
            RenderTexture previous = RenderTexture.active;
            Texture2D result = new Texture2D(tex.width, tex.height);
            result.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            result.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
            return result;
        }

        public static Vector2 JsonVector2ToVector2(this vector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Color HexToColor(this string hex, float alpha = 1)
        {
            string Hex = hex;
            if (hex.Contains("#"))
            {
                string[] Splits = hex.Split('#');
                Hex = Splits[1];
            }

            byte hr = byte.Parse(Hex.Substring(0, 2), NumberStyles.HexNumber);
            byte hg = byte.Parse(Hex.Substring(2, 2), NumberStyles.HexNumber);
            byte hb = byte.Parse(Hex.Substring(4, 2), NumberStyles.HexNumber);

            float r = hr / 255f;
            float g = hg / 255f;
            float b = hb / 255f;

            return new Color(r, g, b, alpha);
        }

        public static string JsonFormatting(this string @base, int indentation = 4)
        {
            JsonSerializer s = new JsonSerializer();
            TextReader t = new StringReader(@base);
            JsonTextReader r = new JsonTextReader(t);
            object target = s.Deserialize(r);
            if (target != null)
            {
                StringWriter stringWriter = new StringWriter();
                JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = indentation,
                    IndentChar = ' '
                };
                s.Serialize(jsonTextWriter, target);
                return stringWriter.ToString();
            }
            return @base;
        }

        public static bool OutOfRange(this ICollection collection, int index)
        {
            return index < 0 || index >= collection.Count;
        }

        public static void OpenFolder(string path)
        {

        }
    }
}
