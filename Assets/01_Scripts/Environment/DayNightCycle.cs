using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // 현재 시간 (0.0 = 자정, 0.5 = 정오, 1.0 = 다시 자정)
    [Range(0f, 1f)] public float timeOfDay;
    
    // 하루의 길이 (초 단위)
    public float lengthOfDay;
    
    // 시작 시간 (기본값: 오전 10시 정도)
    public float startTime = 0.4f;
    
    // 시간 증가 속도 (초당 변화율)
    public float timeOfDayRate;

    // 태양이 정오일 때의 방향 (기본적으로 (90, 0, 0))
    public Vector3 sunDirectionAtNoon;

    [Header("태양 설정")]
    public Light sun; // 태양 (Directional Light)
    public Gradient sunColor; // 태양 색상 (시간대별 변화)
    public AnimationCurve sunIntensity; // 태양 강도 (시간대별 변화)

    [Header("달 설정")]
    public Light moon; // 달 (Directional Light)
    public Gradient moonColor; // 달 색상 (시간대별 변화)
    public AnimationCurve moonIntensity; // 달 강도 (시간대별 변화)

    [Header("기타 조명 설정")]
    public AnimationCurve lightingIntensityMultiplier; // 환경광 강도 (시간대별 변화)
    public AnimationCurve reflectionIntensityMultiplier; // 반사광 강도 (시간대별 변화)

    // 태양과 달의 위상 오프셋 (태양 0.25, 달 0.75)
    private const float SUN_PHASE_OFFSET = 0.25f;
    private const float MOON_PHASE_OFFSET = 0.75f;

    // 하루 동안 태양과 달이 회전하는 배율 (360도를 기준으로 변환)
    private const float FULL_ROTATION_MULTIPLIER = 4.0f;

    private void Start()
    {
        // 하루의 길이에 따라 시간 증가 속도 설정
        timeOfDayRate = 1f / lengthOfDay;
        timeOfDay = startTime; // 시작 시간을 초기화
    }

    private void Update()
    {
        // 시간 진행 (0~1 범위를 유지하도록 모듈러 연산 적용)
        timeOfDay = (timeOfDay + timeOfDayRate * Time.deltaTime) % 1;

        // 태양과 달의 조명 업데이트
        UpdateLighting(sun, sunColor, sunIntensity, SUN_PHASE_OFFSET);
        UpdateLighting(moon, moonColor, moonIntensity, MOON_PHASE_OFFSET);

        // 환경광 및 반사광 강도 업데이트
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(timeOfDay);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(timeOfDay);
    }

    /// <summary>
    /// 특정 광원(태양 또는 달)의 조명을 업데이트하는 함수
    /// </summary>
    private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve, float phaseOffset)
    {
        // 시간대에 따른 조명 강도 설정
        float intensity = intensityCurve.Evaluate(timeOfDay);
        lightSource.intensity = intensity;

        // 조명의 회전 각도를 태양 방향을 기준으로 설정
        lightSource.transform.eulerAngles = sunDirectionAtNoon * ((timeOfDay - phaseOffset) * FULL_ROTATION_MULTIPLIER);

        // 조명의 색상을 시간대에 맞게 변경
        lightSource.color = colorGradient.Evaluate(timeOfDay);

        // 조명의 활성화 여부를 업데이트 (빛이 완전히 사라지면 비활성화)
        UpdateLightActivation(lightSource.gameObject, intensity);
    }

    /// <summary>
    /// 조명의 활성화 여부를 조절하는 함수
    /// </summary>
    private void UpdateLightActivation(GameObject lightObject, float intensity)
    {
        if (intensity == 0 && lightObject.activeInHierarchy)
        {
            lightObject.SetActive(false); // 강도가 0이면 비활성화
        }
        else if (intensity > 0 && !lightObject.activeInHierarchy)
        {
            lightObject.SetActive(true); // 강도가 0보다 크면 활성화
        }
    }
}