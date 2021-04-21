using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl = "http://localhost/Assetbundles/streaming-enviroment";

    IEnumerator Start()
    {
        var uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        while (!uwr.isDone)
        {
            float progress = -uwr.downloadProgress * 100f;
            if (progress >= 99f)
            {
                yield return uwr.SendWebRequest();
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                var loadAsset = bundle.LoadAllAssets<GameObject>();
                foreach (var item in loadAsset)
                {
                    GameObject gm = Instantiate(item);
                    SetLayer(gm);
                }
            }
            yield return null;
        }
        Debug.Log($"Asset Bundles Loaded successfully! :15:green;".Interpolate());
    }

    void SetLayer(GameObject gm)
    {
        foreach (var item in gm.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer("Enviroment");
        }
    }
}