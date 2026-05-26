using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CoffinAnim : NetworkBehaviour
{
    public Transform CoffinTop;
    public Transform Mummy;
    public Transform Door;

    [Header("SFX Clips")]
    public AudioClip DoorOpenClip;
    public AudioClip CoffinMoveClip;
    public AudioClip CartoonClip;

    [Header("Story Clips")]
    public List<AudioClip> IntroClips;
    public List<AudioClip> RiddleClips;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource storyAudioSource;

    public UnityEvent OnAnswerCorrect;
    public UnityEvent OnAnswerWrong;

    public List<GameObject> riddleScreens;
    public List<GameObject> winScreens;
    public List<GameObject> loseScreens;

    [Networked] private bool IfAnimDone { get; set; }
    [Networked] private bool IfRiddleSolved { get; set; }

    private readonly float coffin_move_angle = 30f;
    private readonly Vector3 mummy_target_size = Vector3.one;
    private readonly ScaleDef.WeightType riddle_answer = ScaleDef.WeightType.Skull;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (sfxAudioSource == null)
            sfxAudioSource = GetComponent<AudioSource>();
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
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(CoffinAnimation());
    }

    private IEnumerator CoffinAnimation()
    {
        // 0. Door open SFX
        sfxAudioSource.clip = DoorOpenClip;
        sfxAudioSource.Play();

        float elapsedTime = 0f;
        var doorPos = Door.position;
        var doorScale = Door.localScale;

        while (elapsedTime < sfxAudioSource.clip.length)
        {
            Door.position = doorPos - new Vector3(0, Mathf.Lerp(0, 2, elapsedTime / sfxAudioSource.clip.length), 0);
            Door.localScale = new Vector3(
                doorScale.x,
                Mathf.Lerp(1, 0.5f, elapsedTime / sfxAudioSource.clip.length),
                doorScale.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 1. Coffin open SFX
        sfxAudioSource.clip = CoffinMoveClip;
        sfxAudioSource.Play();

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
        sfxAudioSource.Stop();

        yield return new WaitForSeconds(0.5f);

        // 2. Mummy appear SFX
        sfxAudioSource.PlayOneShot(CartoonClip);

        Vector3 initialScale = Mummy.localScale;
        elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            Mummy.localScale = Vector3.Lerp(initialScale, mummy_target_size, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Mummy.localScale = mummy_target_size;

        // 3. Intro story
        int riddleIndex = Mathf.Abs(MQTTProcessor.Instance.Riddle) % IntroClips.Count;

        AudioClip introClip = IntroClips[riddleIndex];
        storyAudioSource.PlayOneShot(introClip);

        yield return new WaitForSeconds(introClip.length);

        // 4. Riddle story
        int riddleClipIndex = Mathf.Abs(MQTTProcessor.Instance.Riddle) % RiddleClips.Count;

        storyAudioSource.clip = RiddleClips[riddleClipIndex];
        storyAudioSource.Play();

        riddleScreens[riddleClipIndex].SetActive(true);

        yield return new WaitForSeconds(storyAudioSource.clip.length);

        if (Object.HasInputAuthority)
            IfAnimDone = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ReceiveAnswer(bool isCorrect)
    {
        IfRiddleSolved = true;

        if (storyAudioSource != null)
            storyAudioSource.Stop();

        if (sfxAudioSource != null)
            sfxAudioSource.Stop();

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