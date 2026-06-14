using UnityEngine;

public class SealerTool  : MonoBehaviour
{
    public float range = 2f;

    public GameObject sealedPrefab;

    public ParticleSystem redParticles;

    public arm_movement armController;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Trata de sellar.");
            TrySeal();
        }
    }

    void TrySeal()
    {
        Ray ray = new Ray(
            transform.position,
            transform.forward
        );

        if(Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Collide con caja.");

            BoxController box =
                hit.collider.GetComponent<BoxController>();

            if(box == null)
            {
                Debug.Log("Box null.");
                return;
            }

            if(box.sealedBox)
            {
                Debug.Log("La caja ya está sellada.");
                return;
            }

            Vector3 pos = box.transform.position;

            Quaternion rot = box.transform.rotation;

            Destroy(box.gameObject);

            Instantiate(
                sealedPrefab,
                pos,
                rot
            );

            Instantiate(
                redParticles,
                pos + Vector3.up * 0.5f,
                Quaternion.identity
            );

            armController.MoveToRandomPose();

            ConveyorManager.Instance.CurrentBoxSealed();
        }
        else
        {
            Debug.Log("No collide con caja.");
        }
    }
}