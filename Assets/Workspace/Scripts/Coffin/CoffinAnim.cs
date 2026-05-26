using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using uPLibrary.Networking.M2Mqtt;

[RequireComponent(typeof(AudioSource))]
public class CoffinAnim : NetworkBehaviour
{
    public Transform CoffinTop;
    public Transform Mummy;
    public Transform Door;
    public AudioClip DoorOpenClip;
    public AudioClip CoffinMoveClip;
    public AudioClip CartoonClip;
    public List<AudioClip> IntroClips;
    public List<AudioClip> RiddleClips;
    public UnityEvent OnAnswerCorrect;
    public UnityEvent OnAnswerWrong;
    public List<GameObject> riddleScreens;
    public List<GameObject> winScreens;
    public List<GameObject> loseScreens;

    [Networked]
    private bool IfAnimDone { get; set; }
    [Networked]
    private bool IfRiddleSolved { get; set; }

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

        if (IfRiddleSolved) return;

        Rpc_ReceiveAnswer(answer == riddle_answer);
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
        // 0. Door open
        audioSource.clip = DoorOpenClip;
        audioSource.Play();
        float elapsedTime = 0f;
        var doorPos = Door.position;
        var doorScale = Door.localScale;
        while (elapsedTime < audioSource.clip.length)
        {
            Door.position = doorPos - new Vector3(0, Mathf.Lerp(0, 2, elapsedTime / audioSource.clip.length), 0);
            Door.localScale = new Vector3(doorScale.x, Mathf.Lerp(1, 0.5f, elapsedTime / audioSource.clip.length), doorScale.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 1. Coffin open
        audioSource.clip = CoffinMoveClip;
        audioSource.Play();
        Quaternion initialRotation = CoffinTop.localRotation;
        elapsedTime = 0f;
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

        // 2. Mummy appear and say intro
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
        audioSource.Stop();
        var introclip = IntroClips[Mathf.Abs(MQTTProcessor.Instance.Riddle) % IntroClips.Count];
        audioSource.PlayOneShot(introclip);
        yield return new WaitForSeconds(introclip.length);

        // 3. Talk Riddle
        audioSource.bypassEffects = false;
        audioSource.clip = RiddleClips[Mathf.Abs(MQTTProcessor.Instance.Riddle) % RiddleClips.Count];
        audioSource.Play();
        riddleScreens[Mathf.Abs(MQTTProcessor.Instance.Riddle) % riddleScreens.Count].SetActive(true);
        new WaitForSeconds(audioSource.clip.length);

        if (Object.HasInputAuthority)
        {
            IfAnimDone = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ReceiveAnswer(bool isCorrect)
    {
        IfRiddleSolved = true;
        audioSource.Stop();
        if (isCorrect)
        {
            OnAnswerCorrect.Invoke();
            winScreens[MQTTProcessor.Instance.Riddle].SetActive(true);
        }
        else
        {
            OnAnswerWrong.Invoke();
            loseScreens[MQTTProcessor.Instance.Riddle].SetActive(true);
        }
    }
}
