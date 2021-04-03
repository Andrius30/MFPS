using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MFPS.Bundles
{
    public class SpawnStreamedAssets : MonoBehaviour
    {
        public string test = "TEST STREAMING OBJECT";
        public GameObject[] prefabs;

        UnityWebRequest uwr;


        private void Start()
        {

        }
        public static string GetFilelocation(string path)
        {
            return "file://" + Path.Combine(Application.streamingAssetsPath, path);
        }
    }
}