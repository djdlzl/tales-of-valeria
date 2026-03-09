using UnityEngine;

/// <summary>
/// 쿼터뷰 아이소메트릭 카메라 컨트롤러
/// 일렌시아 스타일: 대상을 부드럽게 추적, 고정 각도 유지
///
/// 사용법:
/// 1. Main Camera에 이 스크립트를 추가 (Add Component → IsometricCamera)
/// 2. Target 필드에 플레이어 GameObject를 드래그
/// 3. Play 버튼으로 테스트
/// </summary>
public class IsometricCamera : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;

    [Header("카메라 오프셋 (대상으로부터 거리)")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    [Header("부드러운 이동 속도")]
    [Range(1f, 20f)]
    public float smoothSpeed = 8f;

    [Header("아이소메트릭 카메라 설정")]
    public float orthographicSize = 5f;

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Start()
    {
        SetupIsometricCamera();
    }

    void LateUpdate()
    {
        if (target == null) return;
        FollowTarget();
    }

    void SetupIsometricCamera()
    {
        // 직교(Orthographic) 투영 - 원근감 없음 (아이소메트릭의 핵심)
        _camera.orthographic = true;
        _camera.orthographicSize = orthographicSize;

        // 쿼터뷰 아이소메트릭 회전
        // X=45°: 위에서 45도 내려다봄
        // Y=45°: 남서 방향 (쿼터뷰 특유의 각도)
        transform.rotation = Quaternion.Euler(45f, 45f, 0f);
    }

    void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        transform.position = smoothedPosition;
    }

    // Unity Inspector에서 실시간으로 카메라 설정 미리보기
    void OnValidate()
    {
        if (_camera == null) _camera = GetComponent<Camera>();
        if (_camera != null)
        {
            _camera.orthographic = true;
            _camera.orthographicSize = orthographicSize;
        }
    }
}
