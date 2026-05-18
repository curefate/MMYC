using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using uPLibrary.Networking.M2Mqtt;

[RequireComponent(typeof(AudioSource))]
public class CoffinAnim : NetworkBehaviour
{
    public Transform CoffinTop;
    public Transform Mummy;
    public AudioClip CoffinMoveClip;
    public AudioClip CartoonClip;
    public List<AudioClip> RiddleClips;
    public UnityEvent OnAnswerCorrect;
    public UnityEvent OnAnswerWrong;

    [Networked]
    private bool IfAnimDone { get; set; }

    private readonly float coffin_move_angle = 30f;
    private readonly Vector3 mummy_target_size = Vector3.one;
    private readonly ScaleDef.WeightType riddle_answer = ScaleDef.WeightType.Skull;

    private AudioSource audioSource;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || !IfAnimDone) return;

        var answer = ScaleDef.GetWeightTypeBySerial(MQTTProcessor.Instance.Hall_4);

        if (answer == ScaleDef.WeightType.None) return;

        if (answer == riddle_answer)
        {
            OnAnswerCorrect.Invoke();
        }
        else
        {
            OnAnswerWrong.Invoke();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_PlayCoffinAnimation()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(CoffinAnimation());
    }

    private IEnumerator CoffinAnimation()
    {
        // 1. Coffin open
        audioSource.clip = CoffinMoveClip;
        audioSource.Play();
        Quaternion initialRotation = CoffinTop.localRotation;
        float elapsedTime = 0f;
        while (elapsedTime < 2.5f)
        {
            float angle = Mathf.Lerp(0, coffin_move_angle, elapsedTime / 2.5f);
            CoffinTop.localRotation = initialRotation * Quaternion.Euler(0, angle, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CoffinTop.localRotation = initialRotation * Quaternion.Euler(0, coffin_move_angle, 0);
        audioSource.Stop();

        yield return new WaitForSeconds(0.5f);

        // 2. Mummy appear
        audioSource.PlayOneShot(CartoonClip);
        Vector3 initialScale = Mummy.localScale;
        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Mummy.localScale = Vector3.Lerp(initialScale, mummy_target_size, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Mummy.localScale = mummy_target_size;

        // 3. Talk Riddle
        audioSource.clip = RiddleClips[Mathf.Abs(MQTTProcessor.Instance.Riddle) % RiddleClips.Count];
        audioSource.Play();
        new WaitForSeconds(audioSource.clip.length);

        if (Object.HasInputAuthority)
        {
            IfAnimDone = true;
        }
    }
}
