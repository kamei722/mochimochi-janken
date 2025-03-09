//タイトル画面のアニメーション

using UnityEngine;

public class MultiMover : MonoBehaviour
{
    // 移動の振幅（上下の幅）
    public float amplitude = 10f;

    // 移動速度
    public float frequency = 1f;

    // 最初の移動方向（trueなら上、falseなら下）
    public bool moveUpFirst = true;

    // 初期位置を保持する変数
    private Vector3 startPosition;

    public float phaseOffset = 0f;

    void Start()
    {
        // 初期位置を保存
        startPosition = transform.localPosition;

        // 初期方向を位相オフセットに反映
        phaseOffset += moveUpFirst ? 0f : Mathf.PI;
    }

    void Update()
    {
        // サイン波を計算して位置を更新
        float offsetY = Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;
        transform.localPosition = new Vector3(startPosition.x, startPosition.y + offsetY, startPosition.z);
    }
}


