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
            Debug.Log($"Downloading: { progress }");
            if (progress >= 99f)
            {
                Debug.Log($"Download comple!");

                yield return uwr.SendWebRequest();
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                var loadAsset = bundle.LoadAllAssets<GameObject>();
                foreach (var item in loadAsset)
                {
                    Instantiate(item);
                }
            }
            yield return null;
        }
        Debug.Log($"Asset Bundles Loaded successfully! :15:green;".Interpolate());
    }
}