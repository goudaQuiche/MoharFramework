using Verse;
using UnityEngine;

namespace MoharCustomHAR
{
    public static class BodyTypeDependant
    {
        public static bool IsThereContent(this string newPath)
        {
            return ContentFinder<Texture2D>.Get(newPath + "_south", false) != null;
        }

        public static string InsertBodyTypeDirectoryBeforeFileName(this string originalPath, string bodyType)
        {
            int lastSlashPos = originalPath.LastIndexOf("/") + 1;
            string path = originalPath.Substring(0, lastSlashPos);
            string fileName = originalPath.Substring(lastSlashPos, originalPath.Length - lastSlashPos);

            return path + bodyType + "/" + fileName;
        }
    }
}
