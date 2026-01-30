using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private List<AudioClip> bgmClips = null;
    [SerializeField]
    private List<AudioClip> seClips = null;
    [SerializeField]
    private AudioSource bgmSource = null;
    [SerializeField]
    private AudioSource seSource = null;

    public enum BGMType
    {
        NONE = -1,
        OUTGAME,
        BATTLE,
        VICTORY,
        DEFEAT,
        MAX
    }

    public enum SEType
    {
        NONE = -1,
        BUTTON,
        CARD_DRAW,
        CARD_PLAY,
        DAMAGE,
        HEAL,
        CARD_DESTROY,
        MAX
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource.loop = true;
    }

    public void PlayBGM(BGMType type)
    {
        if (type == BGMType.NONE)
        {
            bgmSource.Stop();
            return;
        }
        AudioClip clip = bgmClips[(int)type];
        if (bgmSource.clip == clip)
        {
            return;
        }
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySE(SEType type)
    {
        if (type == SEType.NONE)
        {
            return;
        }
        AudioClip clip = seClips[(int)type];
        seSource.PlayOneShot(clip);
    }

    // シーケンス
    public Sequence PlaySESequence(SEType type)
    {
        Sequence seq = DOTween.Sequence();
        if (type == SEType.NONE)
        {
            return seq;
        }
        AudioClip clip = seClips[(int)type];
        seq.AppendCallback(() => seSource.PlayOneShot(clip));
        return seq;
    }
}
