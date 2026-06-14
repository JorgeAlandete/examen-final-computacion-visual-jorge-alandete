using UnityEngine;

public class ConveyorButton : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Botón de la cinta transportadora presionado.");
        ConveyorManager.Instance.ConveyorButtonPressed();
    }
}