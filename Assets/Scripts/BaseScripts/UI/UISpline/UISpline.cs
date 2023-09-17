using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class UISpline : MonoBehaviour
{
    public CurveType curveType;
    public bool drawSpline;
    public bool drawTangets;
    public bool drawNormals;
    [Range(20,200)]
    public int splineResolution = 20;

    [SerializeField] private List<SplineControllPoint> splineControllPoints;
    private Vector2 tangetSize = new Vector2(4f, 4f);
    private Vector2 controlPointSize = new Vector2(8f, 8f);



    private void Update()
    {
        if (splineControllPoints == null)
            return;
        foreach (SplineControllPoint scp in splineControllPoints)
        {
            scp.UpdatePoint();
        }
    }

    public SplinePoint GetSplinePoint(float t)
    {
        t = Mathf.Clamp(t, 0f, 1f);

        float[] lenghts = GetSplineLenghts();
        float splineLenght = GetSplineLenght();

        float tLenght = Mathf.Lerp(0f, splineLenght, t);

        float prevLenghts = 0;
        float nextLenghts = 0;
        int index = 0;
        for (int i = 0; i < lenghts.Length; i++)
        {
            prevLenghts = nextLenghts;
            nextLenghts += lenghts[i];
            if (prevLenghts <= tLenght && tLenght < nextLenghts)
            {
                index = i;
                break;
            }

            if (i == lenghts.Length-1 && tLenght == nextLenghts)
            {
                //Debug.LogError("asdasdadas das");
                index = i;
                break;
            }
        }

        float gottenT = Mathf.InverseLerp(prevLenghts, nextLenghts, tLenght);
        //Debug.Log(gottenT);
        float nextT = gottenT + 1f / 20f;

        //Debug.Log($"splineControllPoints count = {splineControllPoints.Count}");
        //Debug.Log($"index = {index}");

        Vector3 pos = GetSplinePointPosition(gottenT, splineControllPoints[index], splineControllPoints[index + 1]);

        bool toForward = true;
        if (nextT > 1f)
        {
            nextT = gottenT - 1f / 20f;
            toForward = false;
        }

        Vector3 nextPos = GetSplinePointPosition(nextT, splineControllPoints[index], splineControllPoints[index + 1]);

        Vector3 normal = Vector3.Cross(Vector3.forward, nextPos - pos).normalized;
        if (!toForward)
            normal *= -1f;

        return new SplinePoint(pos, normal);
    }

    public float GetClosestT(Vector3 aP, int aSteps)
    {
        float step = 1f / (float)aSteps;
        float Res = 0;
        float Ref = float.MaxValue;
        for (int i = 0; i < aSteps; i++)
        {
            float t = step * i;
            float L = (GetSplinePoint(t).Position - aP).sqrMagnitude;
            if (L < Ref)
            {
                Ref = L;
                Res = t;
            }
        }
        return Res;
    }

    public void AddPoint()
    {
        if (splineControllPoints == null)
            splineControllPoints = new List<SplineControllPoint>();

        splineControllPoints.Add(new SplineControllPoint(transform));
    }

    public void RemovePoint()
    {
        if (splineControllPoints.Count > 0)
        {
            SplineControllPoint scp = splineControllPoints[splineControllPoints.Count - 1];
            DestroyImmediate(scp.controllPoint.gameObject);
            splineControllPoints.Remove(scp);
        }
    }

    [ContextMenu("Clear Childs")]
    public void ClearChilds()
    {
        if (!Application.isPlaying)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
                Debug.Log("Destroyed");
            }
            splineControllPoints.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        if (splineControllPoints == null)
            return;
        Gizmos.matrix = transform.localToWorldMatrix;
        //Debug.Log(Gizmos.matrix);
        foreach (SplineControllPoint scp in splineControllPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(scp.controllPoint.anchoredPosition, controlPointSize);

            if (!drawTangets)
                continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(scp.controllPoint.anchoredPosition + scp.firstTanget.anchoredPosition, scp.controllPoint.anchoredPosition);
            Gizmos.DrawLine(scp.controllPoint.anchoredPosition + scp.secondTanget.anchoredPosition, scp.controllPoint.anchoredPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(scp.controllPoint.anchoredPosition + scp.firstTanget.anchoredPosition, tangetSize);
            Gizmos.DrawCube(scp.controllPoint.anchoredPosition + scp.secondTanget.anchoredPosition, tangetSize);
        }

        if (splineControllPoints.Count > 1)
        {
            Gizmos.color = Color.green;
            if (splineResolution < 20)
                splineResolution = 20;

            float length = GetSplineLenght();
            double tStep = splineResolution / length;

            //Debug.Log($"lenght = {length}");
            //Debug.Log($"splineResolution / length = tStep =  {splineResolution / length}");
            double t = 0;
            SplinePoint ps = new SplinePoint(Vector3.zero, Vector3.zero);
            SplinePoint pe= new SplinePoint(Vector3.zero, Vector3.zero);
            while (t<1)
            {
                //Debug.Log($"t =  {t}");
                //Debug.Log($"t + tStep =  {t+tStep}");
                ps = GetSplinePoint((float)t);
                pe = GetSplinePoint((float)t + (float)tStep);

                if (drawSpline)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(ps.Position, pe.Position);
                }

                if (drawNormals)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(ps.Position, ps.Position + ps.Normal * 20f);
                }

                t += tStep;
            }

            ps.Position = pe.Position;
            pe = GetSplinePoint(1f);
            if (drawSpline)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(ps.Position, pe.Position);
            }
        }
    }

    private Vector3 GetSplinePointPosition(float t, SplineControllPoint scp1, SplineControllPoint scp2)
    {
        switch (curveType)
        {
            case CurveType.Parabola:
                return GetParabolaPoint(t, scp1, scp2);
            case CurveType.Bezier:
                return GetBezierPoint(t, scp1, scp2);
            default:
                return Vector3.zero;
        }
    }

    private float GetSplineLenght()
    {
        float[] lenghts = GetSplineLenghts();
        float splineLenght = 0;
        for (int i = 0; i < lenghts.Length; i++)
        {
            splineLenght += lenghts[i];
        }
        return splineLenght;
    }

    private float[] GetSplineLenghts()
    {
        float[] lenghts = new float[splineControllPoints.Count-1];

        for (int i = 1; i < splineControllPoints.Count; i++)
        {
            float il = 0;
            float t = 0;
            for (int j = 0; j < 20; j++)//Not correct
            {
                Vector3 ps = Vector3.zero;
                Vector3 pe = Vector3.zero;
                switch (curveType)
                {
                    case CurveType.Parabola:
                        {
                            ps = GetParabolaPoint(t, splineControllPoints[i - 1], splineControllPoints[i]);
                            pe = GetParabolaPoint(t + 1f / 20f, splineControllPoints[i - 1], splineControllPoints[i]);
                        }
                        break;
                    case CurveType.Bezier:
                        {
                            ps = GetBezierPoint(t, splineControllPoints[i - 1], splineControllPoints[i]);
                            pe = GetBezierPoint(t + 1f / 20f, splineControllPoints[i - 1], splineControllPoints[i]);
                        }
                        break;
                    default:
                        break;
                }
                t = j * 1f / 20f;
                il += Vector3.Distance(ps, pe);
            }

            lenghts[i - 1] = il;
        }
        return lenghts;
    }

    private Vector3 GetBezierPoint(float t, SplineControllPoint scp1, SplineControllPoint scp2)
    {
        Vector3 p0 = scp1.controllPoint.anchoredPosition;
        Vector3 p1 = scp1.secondTanget.anchoredPosition;
        p1 += p0;

        Vector3 p3 = scp2.controllPoint.anchoredPosition;
        Vector3 p2 = scp2.firstTanget.anchoredPosition;
        p2 += p3;

        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);

        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p123 = Vector3.Lerp(p12, p23, t);

        Vector3 p0123 = Vector3.Lerp(p012, p123, t);

        return p0123;
    }

    private Vector3 GetParabolaPoint(float t, SplineControllPoint scp1, SplineControllPoint scp2)
    {
        Vector3 p0 = scp1.controllPoint.anchoredPosition;
        Vector3 p2 = scp2.controllPoint.anchoredPosition;

        Vector3 v01 = scp1.secondTanget.anchoredPosition;
        Vector3 v21 = scp2.firstTanget.anchoredPosition;

        if (!TangetLineIntersection(out Vector3 p1, p0, v01, p2, v21))
        {
            p1 = Vector3.Lerp(p0, p2, 0.5f);
        }

        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);

        Vector3 p012 = Vector3.Lerp(p01, p12, t);

        return p012;
    }

    private bool TangetLineIntersection(out Vector3 intersection, Vector3 p0, Vector3 v01, Vector3 p2, Vector3 v21)
    {

        Vector3 v30 = p2 - p0;
        Vector3 crossV01V32 = Vector3.Cross(v01, v21);
        Vector3 crossV30V32 = Vector3.Cross(v30, v21);

        float planarFactor = Vector3.Dot(v30, crossV01V32);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossV01V32.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossV30V32, crossV01V32) / crossV01V32.sqrMagnitude;
            intersection = p0 + (v01 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }
}
