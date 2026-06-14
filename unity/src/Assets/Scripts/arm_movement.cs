using UnityEngine;
using System.Collections;

public class arm_movement : MonoBehaviour
{
    [Header("Segments")]
    public Transform segment1;
    public Transform segment2;

    [Header("Rotation Speed")]
    public float rotationSpeed = 200f;

    private bool isAnimating = false;

    private Quaternion segment1StartRotation;
    private Quaternion segment2StartRotation;

    void Start()
    {
        segment1StartRotation = segment1.localRotation;
        segment2StartRotation = segment2.localRotation;
    }

    void Update()
    {

        if (isAnimating)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Control del primer segmento (cercano a la base)
        if (Input.GetKey(KeyCode.Q))
        {
            segment1.Rotate(
                Vector3.up,
                mouseX * rotationSpeed * Time.deltaTime,
                Space.Self
            );
        }

        // Control del segundo segmento (cercano al extremo)
        if (Input.GetKey(KeyCode.E))
        {
            segment2.Rotate(
                Vector3.right,
                mouseY * rotationSpeed * Time.deltaTime,
                Space.Self
            );
        }
    }

    public void ResetArm()
    {
        segment1.localRotation = segment1StartRotation;
        segment2.localRotation = segment2StartRotation;
    }


    public void MoveToRandomPose()
    {
        StartCoroutine(RandomPoseAnimation());
    }

    IEnumerator RandomPoseAnimation()
    {
        isAnimating = true;

        Quaternion startRot1 = segment1.localRotation;
        Quaternion startRot2 = segment2.localRotation;

        float angle1 = Random.Range(-60f, 60f);
        float angle2 = Random.Range(-45f, 45f);

        Quaternion targetRot1 =
            Quaternion.Euler(0, angle1, 0);

        Quaternion targetRot2 =
            Quaternion.Euler(0, angle2, 0);

        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            segment1.localRotation =
                Quaternion.Lerp(
                    startRot1,
                    targetRot1,
                    t
                );

            segment2.localRotation =
                Quaternion.Lerp(
                    startRot2,
                    targetRot2,
                    t
                );

            yield return null;
        }

        segment1.localRotation = targetRot1;
        segment2.localRotation = targetRot2;

        isAnimating = false;
    }

}