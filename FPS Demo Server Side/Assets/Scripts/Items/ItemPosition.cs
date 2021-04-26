using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    [SerializeField] bool haveItem = false; // visible for debuging purposes only, and becouse its not the production version
    Vector3 position;

    // setters
    public void SetItemPosition(Vector3 pos) => position = pos;
    public void SethaveItem(bool have) => haveItem = have;

    // getters
    public Vector3 GetPosition() => position;
    public bool DoesHaveItem() => haveItem;
}
