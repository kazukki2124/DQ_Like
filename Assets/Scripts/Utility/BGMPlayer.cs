using UnityEngine;

/// <summary>
/// アタッチしたオブジェクトがアクティブになった時に、指定したBGMをループ再生する汎用スクリプト。
/// Field_01 や ResultScene などのカメラ等にアタッチして使います。
/// </summary>
public class BGMPlayer : MonoBehaviour
{
    [Header("BGM設定")]
    [Tooltip("再生したいBGMのAudioClipをセットしてください")]
    public AudioClip BGMClip;

    [Tooltip("BGMの音量 (0.0 ～ 1.0)")]
    [Range(0f, 1f)]
    public float Volume = 0.5f;

    private AudioSource bgmSource;

    private void Start()
    {
        if (BGMClip != null)
        {
            // AudioSourceがアタッチされていなければ追加する
            bgmSource = gameObject.GetComponent<AudioSource>();
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }

            bgmSource.clip = BGMClip;
            bgmSource.volume = Volume;
            bgmSource.loop = true;      // BGMなのでループ再生を有効にする
            bgmSource.playOnAwake = false;
            
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} に BGMClip が設定されていません！");
        }
    }
}
