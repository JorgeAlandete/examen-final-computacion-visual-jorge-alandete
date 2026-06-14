using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool sealedBox;

    public void Seal()
    {
        sealedBox = true;
    }
}