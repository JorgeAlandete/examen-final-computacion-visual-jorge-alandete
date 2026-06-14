using System.Collections;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public static ConveyorManager Instance;

    [Header("Prefabs")]
    public GameObject unsealedPrefab;

    [Header("Positions")]
    public Transform spawnPoint;
    public Transform workPoint;
    public Transform exitPoint;
    public Transform containerPoint;

    [Header("Movement")]
    public float moveSpeed = 3f;

    private GameObject currentBox;

    private bool boxWaitingForSeal = false;

    private bool boxSealed = false;

    void Awake()
    {
        Instance = this;
    }

    public void ConveyorButtonPressed()
    {
        if(currentBox == null)
        {
            SpawnNewBox();
            return;
        }

        if(boxWaitingForSeal)
        {
            if(boxSealed)
            {
                StartCoroutine(
                    MoveToContainer()
                );
            }
            else
            {
                Debug.Log(
                    "La caja debe sellarse primero."
                );
            }
        }
    }

    void SpawnNewBox()
    {
        currentBox = Instantiate(
            unsealedPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        StartCoroutine(
            MoveToWorkStation()
        );
    }

    IEnumerator MoveToWorkStation()
    {
        while(Vector3.Distance(
            currentBox.transform.position,
            workPoint.position) > 0.05f)
        {
            currentBox.transform.position =
                Vector3.MoveTowards(
                    currentBox.transform.position,
                    workPoint.position,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }

        boxWaitingForSeal = true;
    }

    IEnumerator MoveToContainer()
    {
        boxWaitingForSeal = false;

        while(Vector3.Distance(
            currentBox.transform.position,
            exitPoint.position) > 0.05f)
        {
            currentBox.transform.position =
                Vector3.MoveTowards(
                    currentBox.transform.position,
                    exitPoint.position,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }

        while(Vector3.Distance(
            currentBox.transform.position,
            containerPoint.position) > 0.05f)
        {
            currentBox.transform.position =
                Vector3.MoveTowards(
                    currentBox.transform.position,
                    containerPoint.position,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }

        currentBox.transform.parent =
            containerPoint;

        Debug.Log("Caja movida al contenedor.");

        Destroy(currentBox);

        currentBox = null;

        boxSealed = false;

        SpawnNewBox();
    }

    public void CurrentBoxSealed()
    {
        boxSealed = true;

        currentBox =
            FindFirstObjectByType<BoxController>()
            .gameObject;
    }
}