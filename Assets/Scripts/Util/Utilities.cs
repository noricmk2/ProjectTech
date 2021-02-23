using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UFUtil
{
    #region Func
    public static class Func
    {
        #region Extension Method
        public static void SetActive_Check(this GameObject gameObj, bool bActive)
        {
            if (gameObj.activeSelf != bActive)
            {
                gameObj.SetActive(bActive);
            }
        }

        public static void Init(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            //transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static List<T> ToList<T>(this T[] array)
        {
            List<T> eventList = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                eventList.Add(array[i]);
            }
            return eventList;
        }

        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
                transform.GetChild(i).SetLayerRecursively(layer);
        }

        public static List<T> ShallowCopy<T>(List<T> source)
        {
            List<T> destination = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                destination.Add(source[i]);
            }

            return destination;
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list == null)
                return default(T);

            if (list.Count == 0)
                return default(T);

            System.Random random = new System.Random();
            int index = random.Next(0, list.Count); // [0, list.Count)
            return list[index];
        }

        public static T GetRandom<T>(this List<T> list, int start, int end)
        {
            if (list == null)
                return default(T);

            if (list.Count == 0)
                return default(T);

            if (start < 0)
                start = 0;

            if (end > (list.Count - 1))
                end = (list.Count - 1);

            System.Random random = new System.Random();
            int index = random.Next(start, end); // [0, list.Count)
            return list[index];
        }

        public static void RemoveAll<T>(List<T> list, Func<T, bool> pred) where T : class
        {
            List<T> relist = new List<T>();
            int cnt = list.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (pred(list[i]))
                {
                    relist.Add(list[i]);
                }
            }

            int cnt2 = relist.Count;
            for (int i = 0; i < cnt2; i++)
            {
                list.Remove(relist[i]);
            }
        }

        public static T PopAt<T>(this List<T> list, int index)
        {
            if ((list != null) && (list.Count != 0))
            {
                var first = list[index];
                list.RemoveAt(index);
                return first;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static T Peek<T>(this List<T> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                var first = list[0];
                return first;
            }
            throw new InvalidOperationException("List is empty");
        }

        public static T PopFirst<T>(this List<T> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                var first = list[0];
                list.RemoveAt(0);
                return first;
            }
            throw new InvalidOperationException("List is empty");
        }

        public static T PopFirstOrDefault<T>(this List<T> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                var first = list[0];
                list.RemoveAt(0);
                return first;
            }
            return default(T);
        }

        public static void MoveToFirst<T>(this List<T> list, int index)
        {
            T temp = list[index];
            list.RemoveAt(index);
            list.Insert(0, temp);
        }

        public static void MoveToFirst<T>(this List<T> list, Predicate<T> match)
        {
            int index = list.FindIndex(match);
            if (index == -1)
            {
                return;
            }

            T temp = list[index];
            list.RemoveAt(index);
            list.Insert(0, temp);
        }
        #endregion

        public static Vector2 GetViewPortPosInCamera(Vector2 screenPos, Camera cam, Vector2 margin)
        {
            Vector2 viewPortPos = cam.ScreenToViewportPoint(screenPos);
            margin.x /= Screen.width;
            margin.y /= Screen.height;
            if ((viewPortPos.x - margin.x) < 0)
                viewPortPos.x = margin.x;
            if ((viewPortPos.x + margin.x) > 1)
                viewPortPos.x = 1 - margin.x;
            if ((viewPortPos.y - margin.y) < 0)
                viewPortPos.y = margin.y;
            if ((viewPortPos.y + margin.y) > 1)
                viewPortPos.y = 1 - margin.y;
            return viewPortPos;
        }

        public static bool IsInCamera(Vector3 worldPos, Camera cam, Vector2 margin)
        {
            Vector2 viewPortPos = cam.WorldToViewportPoint(worldPos);
            if ((viewPortPos.x - margin.x) <= 0)
                return false;
            if ((viewPortPos.x + margin.x) > 1)
                return false;
            if ((viewPortPos.y - margin.y) < 0)
                return false;
            if ((viewPortPos.y + margin.y) > 1)
                return false;
            return true;
        }

        public static Vector2 GetSizeByCorner(Vector3[] corners)
        {
            var result = new Vector2(Screen.width - (corners[0].x - corners[2].x), Screen.height - (corners[0].y - corners[1].y));
            return result;
        }

        public static bool InPercent(float percent)
        {
            return UnityEngine.Random.Range(0f, 100f) <= percent;
        }

        public static bool InPercent_100000(int percent)
        {
            return UnityEngine.Random.Range(0, 100001) <= percent;
        }

        public static int GetInt(string value, int defaultVal = 0)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            int result = 0;
            if (int.TryParse(value, out result))
                return result;
            return defaultVal;
        }

        public static float GetFloat(string value, float defaultVal = 0)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            float result = 0;
            if (float.TryParse(value, out result))
                return result;
            return defaultVal;
        }

        public static bool GetBool(string value, bool defaultVal = false)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            bool result;

            if (bool.TryParse(value, out result))
                return result;

            return defaultVal;
        }

        public static T GetEnum<T>(string value) where T : struct
        {
            if (value == null || string.IsNullOrEmpty(value))
                return default(T);

            T result;
            try
            {
                result = (T)System.Enum.Parse(typeof(T), value);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                result = default(T);
            }
            return result;
        }

        public static float Msec2sec(float milliSec)
        {
            return milliSec * 0.001f;
        }

        public static int FastIndexOf(string source, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException();
            if (pattern.Length == 1) return source.IndexOf(pattern[0]);
            bool found;
            int limit = source.Length - pattern.Length + 1;
            if (limit < 1) return -1;
            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];
            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }
                // Check the rest of "pattern" (starting with the 3rd character)
                found = true;
                for (int j = 2; j < pattern.Length; j++)
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                // If the whole word was found, return its index, otherwise try again
                if (found) return first;
                first = source.IndexOf(c0, ++first, limit - first);
            }
            return -1;
        }
    }
    #endregion

    #region Preference
#if UNITY_EDITOR
    public static class EditorPrefsEx
    {
        public static bool GetBoolOrSet(string key, bool value)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefsEx.GetBool(key);
            }
            else
            {
                EditorPrefsEx.SetBool(key, value);
            }
            return value;
        }

        public static int GetIntOrSet(string key, int value)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetInt(key);
            }
            else
            {
                EditorPrefs.SetInt(key, value);
            }
            return value;
        }

        public static void SetBool(string key, bool value)
        {
            int intValue = value ? 1 : 0;
            EditorPrefs.SetInt(key, intValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            int defaultValueInt = (int)System.Convert.ChangeType(defaultValue, typeof(int));
            int value = EditorPrefs.GetInt(key, defaultValueInt);
            bool result = (value == 1) ? true : false;
            return result;
        }
    }
#endif
    public static class PlayerPrefsEx
    {
        public static bool GetBoolOrSet(string key, bool value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return EditorPrefsEx.GetBool(key);
            }
            else
            {
                PlayerPrefsEx.SetBool(key, value);
            }
            return value;
        }

        public static int GetIntOrSet(string key, int value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            else
            {
                PlayerPrefs.SetInt(key, value);
            }
            return value;
        }

        public static void SetBool(string key, bool value)
        {
            int intValue = value ? 1 : 0;
            PlayerPrefs.SetInt(key, intValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            int defaultValueInt = (int)System.Convert.ChangeType(defaultValue, typeof(int));
            int value = PlayerPrefs.GetInt(key, defaultValueInt);
            bool result = (value == 1) ? true : false;
            return result;
        }
    }
    #endregion

    #region StaticString
    public class DelimiterHelper
    {
        public static readonly char[] Comma = new char[] { ',' };
        public static readonly char[] Period = new char[] { '.' };
        public static readonly char[] Pipe = new char[] { '|' };
        public static readonly char[] Semicolon = new char[] { ';' };
        public static readonly char[] Colon = new char[] { ':' };
        public static readonly char[] Newline = new char[] { '\n' };
        public static readonly char[] Underscore = new char[] { '_' };
    }
    #endregion
}
