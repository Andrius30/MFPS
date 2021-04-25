using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl = "http://www.andrejportfolio.lt/Assetbundles/streaming-enviroment";

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.Log("Downloading...");
            yield return null;
        }
        if (www.isDone)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            GameObject[] bundleArray = bundle.LoadAllAssets<GameObject>();
            foreach (var item in bundleArray)
            {
                GameObject gm = Instantiate(item);
                SetLayer(gm);
            }
            Debug.Log($"Download complete {bundle}");
        }
    }
    void SetLayer(GameObject gm)
    {
        foreach (var item in gm.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer("Enviroment");
        }
    }
}