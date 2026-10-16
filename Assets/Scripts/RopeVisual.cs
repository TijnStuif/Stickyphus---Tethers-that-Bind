using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeVisual : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;

    [SerializeField] private int segments = 20;
    [SerializeField] private float maxSag = 1.5f;

    private LineRenderer ropeLine;

    private void Awake()
    {
        ropeLine = GetComponent<LineRenderer>();
        ropeLine.useWorldSpace = true;
        ropeLine.positionCount = segments + 1;
    }

    private void LateUpdate()
    {
        if (player == null || ball.activeSelf == false)
        {
            ropeLine.enabled = false;
            return;
        }

        ropeLine.enabled = true;

        Vector2 start = player.transform.position;
        Vector2 end = ball.transform.position;

        float distance = Vector2.Distance(start, end);

        float ropeLength = 4f;

        // more slack means more sag
        float slack = Mathf.Max(0f, ropeLength - distance);
        float sag = Mathf.Min(slack, maxSag);

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;

            Vector2 point = Vector2.Lerp(start, end, t);

            // sag is greatest in the middle and zero at both ends
            point.y -= sag * 4f * t * (1f - t);

            ropeLine.SetPosition(i, point);
        }
    }

    public void SetRopeVisible(bool visible)
    {
        ropeLine.enabled = visible;
    }
}