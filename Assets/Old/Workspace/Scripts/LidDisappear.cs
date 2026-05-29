using System.Collections;
using Fusion;
using UnityEngine;

public class LidDisappear : NetworkBehaviour
{
    public float liftHeight = 0.005f;
    public float duration = 0.5f;

    private Coroutine openCoverCoroutine;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_OpenCover()
    {
        /* if (!Object.HasStateAuthority)
            return; */

        if (gameObject.activeSelf == false)
        {
            return;
        }

        if (openCoverCoroutine != null)
        {
            StopCoroutine(openCoverCoroutine);
        }
        openCoverCoroutine = StartCoroutine(OpenCoverAnim());
    }

    IEnumerator OpenCoverAnim()
    {
        float time = 0f;

        Vector3 currentStart = transform.localPosition;
        Vector3 targetPos = currentStart + transform.up * liftHeight;

        Vector3 startScale = transform.localScale;

        // --- LIFT ---
        while (time < duration)
        {
            time += Runner.DeltaTime;
            float t = time / duration;

            float ease = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition = Vector3.Lerp(currentStart, targetPos, ease);

            yield return null;
        }

        transform.localPosition = targetPos;

        // --- SHRINK ---
        float shrinkTime = 0f;
        float shrinkDuration = 0.35f;

        while (shrinkTime < shrinkDuration)
        {
            shrinkTime += Runner.DeltaTime;
            float t = shrinkTime / shrinkDuration;

            float ease = t * t;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ease);

            yield return null;
        }

        transform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }
}
