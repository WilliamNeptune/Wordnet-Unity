using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphSystem
{
    public class ConnectionPointHelper : MonoBehaviour
    {
        public Connection CurrentConnection;
        public int positionNum;
        Vector3 startPoint, difference; 
        public List<Vector3> linePoints = new List<Vector3>();
        public bool isDragging = false;
        public bool wasMoved = false;
        public SpriteRenderer renderer;
        public Vector3 controlPoint;

        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void OnMouseEnter()
        {
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock  && !Input.GetMouseButton(0))
            {
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // * Current.orthographicSize / InitialOrtographicSize;
                renderer.enabled = true;
                renderer.color = CurrentConnection.ConnectionLineRenderer.startColor;
            }
        }

        public void OnMouseExit()
        {
            if (!isDragging && !wasMoved)
                renderer.enabled = false;
        }

        public void OnMouseDown()
        {
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock && CurrentConnection.ConMode == ConnectionMode.Straight)
            {
                startPoint = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition);
                difference = Input.mousePosition - GraphMechanism.instance.cam.WorldToScreenPoint(transform.position);

                linePoints.Clear();
                for (int i = 0; i < CurrentConnection.ConnectionLineRenderer.positionCount; i++)
                {
                    linePoints.Add(CurrentConnection.ConnectionLineRenderer.GetPosition(i));
                }

                // if (!wasMoved)
                // {
                //     transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                //     var lhPoint = Instantiate(gameObject);
                //     lhPoint.transform.parent = transform.parent;
                //     lhPoint.transform.name = "point";
                //     var lhHelper = lhPoint.GetComponent<ConnectionPointHelper>();
                //     lhHelper.positionNum = positionNum;
                //     lhHelper.CurrentConnection = CurrentConnection;
                //     lhHelper.wasMoved = false;
                //     lhHelper.isDragging = false;
                //     var rhPoint = Instantiate(gameObject);
                //     rhPoint.transform.parent = transform.parent;
                //     rhPoint.transform.name = "point";
                //     var rhHelper = rhPoint.GetComponent<ConnectionPointHelper>();
                //     rhHelper.positionNum = positionNum + 2;
                //     rhHelper.CurrentConnection = CurrentConnection;
                //     rhHelper.wasMoved = false;
                //     rhHelper.isDragging = false;
                //     var lhSprite = lhPoint.GetComponent<SpriteRenderer>();
                //     var rhSprite = rhPoint.GetComponent<SpriteRenderer>();
                //     lhSprite.enabled = false;
                //     rhSprite.enabled = false;
                //     lhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                //     rhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                //     lhPoint.transform.position = CurrentConnection.GetBezierPointBetweenTwoPoints(linePoints[positionNum], linePoints[positionNum - 1], 0.5f);
                //     lhPoint.transform.position = new Vector3(lhPoint.transform.position.x, lhPoint.transform.position.y, transform.position.z);
                //     rhPoint.transform.position = CurrentConnection.GetBezierPointBetweenTwoPoints(linePoints[positionNum], linePoints[positionNum + 1], 0.5f);
                //     rhPoint.transform.position = new Vector3(rhPoint.transform.position.x, rhPoint.transform.position.y, transform.position.z);
                //     lhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrtographicSize / InitialOrtographicSize;
                //     rhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrtographicSize / InitialOrtographicSize;

                //     CurrentConnection.ConPoints.Add(lhSprite);
                //     CurrentConnection.ConPoints.Add(rhSprite);

                //     var newLinePoints = new List<Vector3>();
                //     for (int i = 0; i < positionNum; i++)
                //     {
                //         newLinePoints.Add(linePoints[i]);
                //     }
                //     newLinePoints.Add(CurrentConnection.GetBezierPointBetweenTwoPoints(linePoints[positionNum], linePoints[positionNum - 1], 0.5f));
                //     newLinePoints.Add(linePoints[positionNum]);
                //     newLinePoints.Add(CurrentConnection.GetBezierPointBetweenTwoPoints(linePoints[positionNum], linePoints[positionNum + 1], 0.5f));
                //     for (int i = positionNum + 1; i < linePoints.Count; i++)
                //     {
                //         newLinePoints.Add(linePoints[i]);
                //     }
                //     linePoints.Clear();
                //     linePoints = new List<Vector3>(newLinePoints);

                //     foreach (var h in CurrentConnection.ConPointHelpers)
                //     {
                //         if (h != this)
                //         {
                //             if (h.positionNum > positionNum)
                //             {
                //                 h.positionNum += 2;
                //             }
                //         }
                //     }

                //     positionNum += 1;
                    
                //     CurrentConnection.ConPointHelpers.Add(lhHelper);
                //     CurrentConnection.ConPointHelpers.Add(rhHelper);

                //     var temp = CurrentConnection.ConPointHelpers.OrderBy(ConnectionPointHelper => ConnectionPointHelper.positionNum).ToList();
                //     CurrentConnection.ConPointHelpers.Clear();
                //     CurrentConnection.ConPointHelpers = new List<ConnectionPointHelper>(temp);

                //     foreach (var h in CurrentConnection.ConPointHelpers)
                //     {
                //         if (h != this)
                //         {
                //             h.linePoints.Clear();
                //             h.linePoints = new List<Vector3>(linePoints);
                //         }
                //     }

                //     CurrentConnection.ConnectionLineRenderer.positionCount = linePoints.Count;
                //     CurrentConnection.ConnectionLineRenderer.SetPositions(linePoints.ToArray());
                //     CurrentConnection.straightPoints.Clear();
                //     CurrentConnection.straightPoints.AddRange(linePoints);

                //     lhHelper.controlPoint = lhPoint.transform.position;
                //     rhHelper.controlPoint = rhPoint.transform.position;
                //     lhHelper.controlPoint.z = 0;
                //     rhHelper.controlPoint.z = 0;
                // }
            }

            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock && CurrentConnection.ConMode == ConnectionMode.Bezier)
            {
                if (!wasMoved)
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

                    var lhPoint = Instantiate(gameObject);
                    lhPoint.transform.parent = transform.parent;
                    lhPoint.transform.name = "point";
                    var lhHelper = lhPoint.GetComponent<ConnectionPointHelper>();
                    lhHelper.positionNum = positionNum;
                    lhHelper.CurrentConnection = CurrentConnection;
                    lhHelper.wasMoved = false;
                    lhHelper.isDragging = false;

                    var rhPoint = Instantiate(gameObject);
                    rhPoint.transform.parent = transform.parent;
                    rhPoint.transform.name = "point";
                    var rhHelper = rhPoint.GetComponent<ConnectionPointHelper>();
                    rhHelper.positionNum = positionNum + 2;
                    rhHelper.CurrentConnection = CurrentConnection;
                    rhHelper.wasMoved = false;
                    rhHelper.isDragging = false;

                    var lhSprite = lhPoint.GetComponent<SpriteRenderer>();
                    var rhSprite = rhPoint.GetComponent<SpriteRenderer>();
                    lhSprite.enabled = false;
                    rhSprite.enabled = false;
                    lhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    rhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                    var points = new List<Vector3>();
                    if (CurrentConnection.bezierPoints.Count < 1)
                    {
                        var start = CurrentConnection.Start.transform.position;
                        start.z = 0;
                        var end = CurrentConnection.End.transform.position;
                        end.z = 0;
                        points.Add(start);
                        points.Add(CurrentConnection.GetBezierPoint(0.5f));
                        points.Add(end);
                        points.Add(end);
                        CurrentConnection.bezierPoints = new List<Vector3>(points);
                    }
                    else
                    {
                        var start = CurrentConnection.Start.transform.position;
                        start.z = 0;
                        var end = CurrentConnection.End.transform.position;
                        end.z = 0;

                        points.Add(start);
                        int con = 0;
                        for (con = 0; con < CurrentConnection.ConPointHelpers.Count; con++)
                        {
                            if (CurrentConnection.ConPointHelpers[con] != this)
                            {
                                if (CurrentConnection.ConPointHelpers[con].wasMoved)
                                {
                                    points.Add(CurrentConnection.ConPointHelpers[con].controlPoint);
                                }
                            }
                            else
                                break;
                        }
                        points.Add(controlPoint);

                        if (con + 1 < CurrentConnection.ConPointHelpers.Count)
                            for (int con2 = con + 1; con2 < CurrentConnection.ConPointHelpers.Count; con2++)
                            {
                                if (CurrentConnection.ConPointHelpers[con2].wasMoved)
                                {
                                    points.Add(CurrentConnection.ConPointHelpers[con2].controlPoint);
                                }
                            }
                        points.Add(end);
                        points.Add(end);
                    }

                    //-----------------
                    var newLhPoints = new List<Vector3>();
                    var newRhPoints = new List<Vector3>();
                    int c_ = 0;
                    float dL = 0.5f,dR = 0.5f;
                    for (int i_ = 0; i_ < points.Count; i_++)
                    {
                        if (points[i_].x == controlPoint.x && points[i_].y == controlPoint.y)
                            c_ = i_;
                    }
                    
                    //Debug.Log("c_ = " + c_ + " " + points.Count);

                    int c1 = c_ - 1;
                    if (c1 < 0)
                        c1 = 0;
                    int c2 = c_ - 2;
                    if (c2 < 0)
                        c2 = 0;
                    var prevLh = points[c2];
                    var prevRh = points[c1];

                    newRhPoints.Add(points[c_]); //controlPoint
                    newRhPoints.Add(points[c_ + 1]);
                    newRhPoints.Add(points[c_ + 2]);
                    newLhPoints.Add(points[c1]);
                    newLhPoints.Add(points[c_]); //controlPoint
                    newLhPoints.Add(points[c_ + 1]);
                    //-----------------

                    var newLhPoint = CurrentConnection.GetPointFromCurve(newLhPoints, dL, prevLh.y, prevLh.x);
                    var newRhPoint = CurrentConnection.GetPointFromCurve(newRhPoints, dR, prevRh.y, prevRh.x);

                    lhPoint.transform.position = newLhPoint; 
                    lhPoint.transform.position = new Vector3(lhPoint.transform.position.x, lhPoint.transform.position.y, transform.position.z);
                    rhPoint.transform.position = newRhPoint; 
                    rhPoint.transform.position = new Vector3(rhPoint.transform.position.x, rhPoint.transform.position.y, transform.position.z);
                    lhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrtographicSize / InitialOrtographicSize;
                    rhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrtographicSize / InitialOrtographicSize;

                    CurrentConnection.ConPoints.Add(lhSprite);
                    CurrentConnection.ConPoints.Add(rhSprite);

                    foreach (var h in CurrentConnection.ConPointHelpers)
                    {
                        if (h != this)
                        {
                            if (h.positionNum > positionNum)
                            {
                                h.positionNum += 2;
                            }
                        }
                    }

                    positionNum += 1;

                    CurrentConnection.ConPointHelpers.Add(lhHelper);
                    CurrentConnection.ConPointHelpers.Add(rhHelper);

                    var temp = CurrentConnection.ConPointHelpers.OrderBy(ConnectionPointHelper => ConnectionPointHelper.positionNum).ToList();
                    CurrentConnection.ConPointHelpers.Clear();
                    CurrentConnection.ConPointHelpers = new List<ConnectionPointHelper>(temp);

                    lhHelper.controlPoint = lhPoint.transform.position;
                    lhHelper.controlPoint.z = 0;
                    rhHelper.controlPoint = rhPoint.transform.position;
                    rhHelper.controlPoint.z = 0;

                    controlPoint = transform.position;
                    controlPoint.z = 0;
                }
                
                startPoint = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public void OnMouseUp()
        {
            if (isDragging)
                CurrentConnection.UpdateEdgeCollider();
            
            isDragging = false;
            GraphMechanism.instance.isConnectionPointDragging = false;
        }

        public void OnMouseDrag()
        {
            if (CurrentConnection.ConMode == ConnectionMode.Straight)
            {
                GraphMechanism.instance.isConnectionPointDragging = true;
                isDragging = true;
                wasMoved = true;
                CurrentConnection.PointsWereChanged = true;
                renderer.enabled = true;
                var mouse = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition) - startPoint;
                transform.position += new Vector3(mouse.x, mouse.y, 0);

                linePoints[positionNum] = new Vector3(/*centerPoint*/controlPoint.x + mouse.x, /*centerPoint*/controlPoint.y + mouse.y, linePoints[0].z);

                //point sticking (for straight lines) 
                #region Point sticking

                var posBefore = CurrentConnection.ConnectionLineRenderer.GetPosition(positionNum - 1);
                var posAfter = CurrentConnection.ConnectionLineRenderer.GetPosition(positionNum + 1);
                var distanceX0 = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).x - GraphMechanism.instance.cam.WorldToScreenPoint(posBefore).x);
                var distanceX2 = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).x - GraphMechanism.instance.cam.WorldToScreenPoint(posAfter).x);
                var distanceY0 = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).y - GraphMechanism.instance.cam.WorldToScreenPoint(posBefore).y);
                var distanceY2 = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).y - GraphMechanism.instance.cam.WorldToScreenPoint(posAfter).y);
                
                if (distanceY0 < 5)
                {
                    linePoints[positionNum] = new Vector3(linePoints[positionNum].x, posBefore.y, linePoints[positionNum].z);
                    transform.position = new Vector3(transform.position.x, linePoints[positionNum].y, transform.position.z);
                }
                if (distanceY2 < 5)
                {
                    linePoints[positionNum] = new Vector3(linePoints[positionNum].x, posAfter.y, linePoints[positionNum].z);
                    transform.position = new Vector3(transform.position.x, linePoints[positionNum].y, transform.position.z);
                }
                if (distanceX0 < 5)
                {
                    linePoints[positionNum] = new Vector3(posBefore.x, linePoints[positionNum].y, linePoints[positionNum].z);
                    transform.position = new Vector3(linePoints[positionNum].x, transform.position.y, transform.position.z);
                }
                if (distanceX2 < 5)
                {
                    linePoints[positionNum] = new Vector3(posAfter.x, linePoints[positionNum].y, linePoints[positionNum].z);
                    transform.position = new Vector3(linePoints[positionNum].x, transform.position.y, transform.position.z);
                }

                startPoint = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition);

                var disY = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).y - (Input.mousePosition.y - difference.y));
                if (disY > 15)
                {
                    linePoints[positionNum] = new Vector3(linePoints[positionNum].x, GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition - difference).y, linePoints[positionNum].z);
                    transform.position = new Vector3(linePoints[positionNum].x, linePoints[positionNum].y, transform.position.z);
                }
                var disX = Mathf.Abs(GraphMechanism.instance.cam.WorldToScreenPoint(transform.position).x - (Input.mousePosition.x - difference.x));
                if (disX > 15)
                {
                    linePoints[positionNum] = new Vector3(GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition - difference).x, linePoints[positionNum].y, linePoints[positionNum].z);
                    transform.position = new Vector3(linePoints[positionNum].x, linePoints[positionNum].y, transform.position.z);
                }

                #endregion

                controlPoint = linePoints[positionNum];
                CurrentConnection.ConnectionLineRenderer.SetPositions(linePoints.ToArray());

                CurrentConnection.straightPoints.Clear();
                CurrentConnection.straightPoints = new List<Vector3>(linePoints);

                CurrentConnection.UpdatePositions();
            }

            if (CurrentConnection.ConMode == ConnectionMode.Bezier)
            {
                isDragging = true;
                CurrentConnection.PointsWereChanged = true;
                wasMoved = true;
                renderer.enabled = true;

                var mouse = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition) - startPoint;
                mouse.z = 0;
                controlPoint += mouse /** 2.671f*/;
                var middle = new Vector3(controlPoint.x, controlPoint.y, 0);

                List<Vector3> points = new List<Vector3>();
                var begin = CurrentConnection.Start.transform.position;
                begin.z = 0;
                points.Add(begin);
                
                int con = 0;
                for (con = 0; con < CurrentConnection.ConPointHelpers.Count; con++)
                {
                    if (CurrentConnection.ConPointHelpers[con] != this)
                    {
                        if (CurrentConnection.ConPointHelpers[con].wasMoved)
                        {
                            points.Add(CurrentConnection.ConPointHelpers[con].controlPoint);
                        }
                    }
                    else
                        break;
                }
                
                transform.position = new Vector3(middle.x, middle.y, transform.position.z);
                points.Add(middle);

                if (con + 1 < CurrentConnection.ConPointHelpers.Count)
                    for (int con2 = con + 1; con2 < CurrentConnection.ConPointHelpers.Count; con2++)
                    {
                        if (CurrentConnection.ConPointHelpers[con2].wasMoved)
                        {
                            points.Add(CurrentConnection.ConPointHelpers[con2].controlPoint);
                        }
                    }

                var end = CurrentConnection.End.transform.position;
                end.z = 0;
                points.Add(end);
                points.Add(end);

                //-----------------
                for (int i = 0; i < CurrentConnection.ConPointHelpers.Count; i++)
                {
                    if (!CurrentConnection.ConPointHelpers[i].wasMoved)
                    {
                        var newRhPoints = new List<Vector3>();
                        int c_ = 0;
                        int l = i - 1;
                        if (l < 0)
                            l = 0;

                        var firstPoint = CurrentConnection.ConPointHelpers[l].controlPoint;
                        if (CurrentConnection.ConPointHelpers[i].positionNum == 1)
                        {
                            firstPoint = points[0];
                        }
                        for (int i_ = 0; i_ < points.Count; i_++)
                        {
                            if (points[i_].x == firstPoint.x && points[i_].y == firstPoint.y)
                                c_ = i_;
                        }

                        int c1 = c_ - 1;
                        if (c1 < 0)
                            c1 = 0;
                        int c2 = c_ - 2;
                        if (c2 < 0)
                            c2 = 0;
                        var prevRh = points[c1];

                        newRhPoints.Add(points[c_]); //controlPoint
                        newRhPoints.Add(points[c_ + 1]);
                        newRhPoints.Add(points[c_ + 2]);

                        var newPoint = CurrentConnection.GetPointFromCurve(newRhPoints, 0.5f, prevRh.y, prevRh.x); 

                        CurrentConnection.ConPointHelpers[i].controlPoint = newPoint; 
                        var pos = CurrentConnection.ConPointHelpers[i].transform.position;
                        CurrentConnection.ConPointHelpers[i].transform.position = new Vector3(CurrentConnection.ConPointHelpers[i].controlPoint.x, CurrentConnection.ConPointHelpers[i].controlPoint.y, pos.z);
                    }
                }
                //-----------------

                CurrentConnection.DrawByCurves(points);
                CurrentConnection.UpdateArrowsForChangedBezierLine();

                startPoint = GraphMechanism.instance.cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public void RemovePoint()
        {
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf /*&& !GraphMechanism.instance.UIMouseBlock*/ && CurrentConnection.ConMode == ConnectionMode.Straight && wasMoved && renderer.enabled)
            {
                linePoints.Clear();
                for (int i = 0; i < CurrentConnection.ConnectionLineRenderer.positionCount; i++)
                {
                    linePoints.Add(CurrentConnection.ConnectionLineRenderer.GetPosition(i));
                }

                wasMoved = false;
                renderer.enabled = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                //----------------------------------------------------------
                int prev = positionNum - 1;
                int next = positionNum + 1;
                int keffStart = 0, keffEnd = CurrentConnection.ConnectionLineRenderer.positionCount-1;
                var newLinePoints = new List<Vector3>();

                while (prev > 0 )
                {
                    if (CurrentConnection.ConPointHelpers[prev-1].wasMoved)
                    {
                        keffStart = prev;
                        break;
                    }

                    prev -= 1;
                }

                for (int i = 0; i <= keffStart; i++)
                {
                    newLinePoints.Add(CurrentConnection.ConnectionLineRenderer.GetPosition(i));
                }
                newLinePoints.Add(CurrentConnection.ConnectionLineRenderer.GetPosition(positionNum));

                while (next < CurrentConnection.ConnectionLineRenderer.positionCount - 1)
                {
                    if (CurrentConnection.ConPointHelpers[next-1].wasMoved)
                    {
                        keffEnd = next;
                        break;
                    }

                    next += 1;
                }

                for (int i = keffEnd; i < CurrentConnection.ConnectionLineRenderer.positionCount; i++)
                {
                    newLinePoints.Add(CurrentConnection.ConnectionLineRenderer.GetPosition(i));
                }

                int count = 0, countBefore = 0;
                int s = keffStart;
                if (s < 0)
                    s = 0;
                for (int i = s; i < keffEnd - 1; i++)
                {
                    var g = CurrentConnection.ConPointHelpers[i - count].gameObject;
                    if (g != gameObject)
                    {
                        if (CurrentConnection.ConPointHelpers[i - count].positionNum < positionNum)
                            countBefore++;
                        if (CurrentConnection.ConPointHelpers.Contains(CurrentConnection.ConPointHelpers[i - count]))
                            CurrentConnection.ConPointHelpers.Remove(CurrentConnection.ConPointHelpers[i - count]);
                        if (CurrentConnection.ConPoints.Contains(g.GetComponent<SpriteRenderer>()))
                            CurrentConnection.ConPoints.Remove(g.GetComponent<SpriteRenderer>());
                        Destroy(g);
                        count++;
                    }
                }

                foreach (var c in CurrentConnection.ConPointHelpers)
                {
                    if (c.positionNum > positionNum && c != this)
                        c.positionNum -= count;
                }
                positionNum -= countBefore;
                if (positionNum <= 0)
                    positionNum = 1;

                CurrentConnection.ConnectionLineRenderer.positionCount = newLinePoints.Count;
                CurrentConnection.ConnectionLineRenderer.SetPositions(newLinePoints.ToArray());

                CurrentConnection.straightPoints.Clear();
                CurrentConnection.straightPoints = new List<Vector3>(newLinePoints);

                if (CurrentConnection.ConnectionLineRenderer.positionCount == 3)
                {
                    CurrentConnection.PointsWereChanged = false;
                    CurrentConnection.ConPoint = renderer;
                }
                //----------------------------------------------------------

                CurrentConnection.UpdatePositions();
                CurrentConnection.UpdateEdgeCollider();
            }

            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf /*&& !GraphMechanism.instance.UIMouseBlock*/ && CurrentConnection.ConMode == ConnectionMode.Bezier && wasMoved && renderer.enabled)
            {
                wasMoved = false;
                renderer.enabled = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                //----------------------------------------------------------
                int prev = positionNum - 1;
                int next = positionNum + 1;
                int keffStart = 0, keffEnd = CurrentConnection.ConPointHelpers.Count+1;
                
                while (prev > 0)
                {
                    if (CurrentConnection.ConPointHelpers[prev-1].wasMoved)
                    {
                        keffStart = prev;
                        break;
                    }

                    prev -= 1;
                }

                while (next < CurrentConnection.ConPointHelpers.Count+1)
                {
                    if (CurrentConnection.ConPointHelpers[next-1].wasMoved)
                    {
                        keffEnd = next;
                        break;
                    }

                    next += 1;
                }

                int count = 0, countBefore = 0;
                int s = keffStart;
                if (s < 0)
                    s = 0;
                if (keffEnd > CurrentConnection.ConPointHelpers.Count+1)
                    keffEnd = CurrentConnection.ConPointHelpers.Count+1;

                for (int i = s; i < keffEnd-1; i++)
                {
                    var g = CurrentConnection.ConPointHelpers[i - count].gameObject;
                    if (g != gameObject)
                    {
                        if (CurrentConnection.ConPointHelpers[i - count].positionNum < positionNum)
                            countBefore++;
                        if (CurrentConnection.ConPointHelpers.Contains(CurrentConnection.ConPointHelpers[i - count]))
                            CurrentConnection.ConPointHelpers.Remove(CurrentConnection.ConPointHelpers[i - count]);
                        if (CurrentConnection.ConPoints.Contains(g.GetComponent<SpriteRenderer>()))
                            CurrentConnection.ConPoints.Remove(g.GetComponent<SpriteRenderer>());
                        Destroy(g);
                        count++;
                    }
                }

                foreach (var c in CurrentConnection.ConPointHelpers)
                {
                    if (c.positionNum > positionNum && c != this)
                        c.positionNum -= count;
                }
                positionNum -= countBefore;
                if (positionNum <= 0)
                    positionNum = 1;

                if (CurrentConnection.ConPointHelpers.Count == 1 )
                {
                    CurrentConnection.PointsWereChanged = false;
                    CurrentConnection.ConPoint = renderer;
                }

                CurrentConnection.bezierPoints.Clear();
                var start = CurrentConnection.Start.transform.position;
                start.z = 0;
                var end = CurrentConnection.End.transform.position;
                end.z = 0;
                CurrentConnection.bezierPoints.Add(start);
                for(int p = 0; p < CurrentConnection.ConPointHelpers.Count; p++)
                {
                    if (CurrentConnection.ConPointHelpers[p].wasMoved)
                        CurrentConnection.bezierPoints.Add(CurrentConnection.ConPointHelpers[p].controlPoint);
                }
                CurrentConnection.bezierPoints.Add(end);
                CurrentConnection.bezierPoints.Add(end);

                if (CurrentConnection.ConPointHelpers.Count == 3)
                {
                    CurrentConnection.bezierPoints.Clear();
                    CurrentConnection.bezierPoints.Add(start);
                    CurrentConnection.bezierPoints.Add(CurrentConnection.ConPointHelpers[1].controlPoint);
                    CurrentConnection.bezierPoints.Add(end);
                    CurrentConnection.bezierPoints.Add(end);
                }

                CurrentConnection.DrawByCurves(CurrentConnection.bezierPoints);
                
                if (!CurrentConnection.PointsWereChanged)
                {
                    CurrentConnection.UpdatePositions();
                    CurrentConnection.ConPointHelpers[0].transform.position = new Vector3(CurrentConnection.GetBezierPoint(0.5f).x, CurrentConnection.GetBezierPoint(0.5f).y, CurrentConnection.ConPointHelpers[0].transform.position.z);
                }
                else
                {
                    //-----------------
                    for (int i = 0; i < CurrentConnection.ConPointHelpers.Count; i++)
                    {
                        if (!CurrentConnection.ConPointHelpers[i].wasMoved)
                        {
                            var newRhPoints = new List<Vector3>();
                            int c_ = 0;
                            int l = i - 1;
                            if (l < 0)
                                l = 0;

                            var firstPoint = CurrentConnection.ConPointHelpers[l].controlPoint;
                            if (CurrentConnection.ConPointHelpers[i].positionNum == 1)
                            {
                                firstPoint = CurrentConnection.bezierPoints[0];
                            }
                            for (int i_ = 0; i_ < CurrentConnection.bezierPoints.Count; i_++)
                            {
                                if (CurrentConnection.bezierPoints[i_].x == firstPoint.x && CurrentConnection.bezierPoints[i_].y == firstPoint.y)
                                    c_ = i_;
                            }

                            int c1 = c_ - 1;
                            if (c1 < 0)
                                c1 = 0;
                            int c2 = c_ - 2;
                            if (c2 < 0)
                                c2 = 0;

                            var prevRh = CurrentConnection.bezierPoints[c1];

                            newRhPoints.Add(CurrentConnection.bezierPoints[c_]);
                            newRhPoints.Add(CurrentConnection.bezierPoints[c_ + 1]);
                            newRhPoints.Add(CurrentConnection.bezierPoints[c_ + 2]);

                            var newPoint = CurrentConnection.GetPointFromCurve(newRhPoints, 0.5f, prevRh.y, prevRh.x);

                            CurrentConnection.ConPointHelpers[i].controlPoint = newPoint;
                            var pos = CurrentConnection.ConPointHelpers[i].transform.position;
                            CurrentConnection.ConPointHelpers[i].transform.position = new Vector3(CurrentConnection.ConPointHelpers[i].controlPoint.x, CurrentConnection.ConPointHelpers[i].controlPoint.y, pos.z);
                        }
                    }
                    //-----------------

                    CurrentConnection.UpdateArrowsForChangedBezierLine();
                }
                CurrentConnection.UpdateEdgeCollider();
            }
        }

    }

}
