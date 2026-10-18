using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeVisual : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;

    [SerializeField] private int ropeSegments = 20;
    [SerializeField] private float maxSag = 1.5f;

    private LineRenderer ropeLine;

    private void Awake()
    {
        ropeLine = GetComponent<LineRenderer>();
        ropeLine.useWorldSpace = true;
        ropeLine.positionCount = ropeSegments + 1;
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

        float slack = Mathf.Max(0f, ropeLength - distance);
        float sag = Mathf.Min(slack, maxSag);

        // for loop that draws line points based on player/ball distance
        for (int i = 0; i <= ropeSegments; i++)
        {
            float t = (float)i / ropeSegments;

            Vector2 point = Vector2.Lerp(start, end, t);

            point.y -= sag * ropeLength * t * (1f - t);

            ropeLine.SetPosition(i, point);
        }
    }
}