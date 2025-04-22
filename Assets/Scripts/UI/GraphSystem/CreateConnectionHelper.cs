using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GraphSystem
{
    public class CreateConnectionHelper : MonoBehaviour
    {
        //public static bool isHighlighted = false;
        SpriteRenderer icon;
        List<LineRenderer> tempLines = new List<LineRenderer>();
        Coroutine ConnectionCoroutine, ContextMenuCoroutine;
        bool tempCheck = false;

        private void Awake()
        {
            icon = transform.GetComponent<SpriteRenderer>();
        }

        public void OnMouseEnter()
        {
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock)
                icon.enabled = true;
        }

        public void OnMouseExit()
        {
            icon.enabled = false;
        }

        public void OnMouseDown()
        {
            //CreateLink.instance.EntitiesToConnect.Clear();
            GraphMechanism.instance.FirstSelectedEntity = transform.parent.GetComponent<EntitySceneHelper>();

            /*if (GraphMechanism.instance.selectedEntitys.Count > 1)
            {
                for (int i = 0; i < GraphMechanism.instance.selectedEntitys.Count; i++)
                {
                    if (GraphMechanism.DragList[i].Ent.EntityObj.Uid != transform.parent.GetComponent<EntitySceneHelper>().CurrentEntity.EntityObj.Uid)
                    {
                        EntityObject[] tempArray = new EntityObject[2];
                        tempArray[0] = GraphMechanism.DragList[i].Ent;
                        CreateLink.instance.EntitiesToConnect.Add(tempArray);
                    }
                }
            }*/

            ConnectionCoroutine = StartCoroutine(StartConnection());
            tempCheck = true;
        }

        public void OnMouseUp()
        {
            tempCheck = false;
        }

        public void OnMouseDrag()
        {
            if (tempCheck)
            {
                tempCheck = false;
            }
        }

        IEnumerator StartConnection()
        {
            icon.enabled = false;
            var graphMechanism = GraphMechanism.instance;

            GraphMechanism.instance.ConnectionLine.DOColor(new Color2(Color.white, Color.red), new Color2(Color.green, Color.black), 1);

            GraphMechanism.instance.ConnectionLine.gameObject.SetActive(true);
            GraphMechanism.instance.ConnectionLine.SetPosition(0, transform.parent.position);
            GraphMechanism.instance.ConnectionLine.startColor = GraphMechanism.instance.LineColor.color;
            GraphMechanism.instance.ConnectionLine.endColor = GraphMechanism.instance.LineColor.color;
            if (GraphMechanism.instance.LineStyle == 0)
                GraphMechanism.instance.ConnectionLine.material = GraphMechanism.instance.FullMaterial;
            else if (GraphMechanism.instance.LineStyle == 1)
                GraphMechanism.instance.ConnectionLine.material = GraphMechanism.instance.CutMaterial;
            else if (GraphMechanism.instance.LineStyle == 2)
                GraphMechanism.instance.ConnectionLine.material = GraphMechanism.instance.DotMaterial;
            
            /*if (graphMechanism.selectedEntitys.Count > 1)
            {
                for (int i = 0; i < graphMechanism.selectedEntitys.Count; i++)
                {
                    if (GraphMechanism.DragList[i].Ent != transform.parent.GetComponent<EntitySceneHelper>().CurrentEntity)
                    {
                        var l = Instantiate(createLink.ConnectionLine);
                        l.transform.SetParent(createLink.ConnectionLine.transform.parent);
                        l.SetPosition(0, GraphMechanism.DragList[i].Ent.transform.position);
                        tempLines.Add(l);
                    }
                }
            }*/

            while (true)
            {
                Camera camera = GraphMechanism.instance.cam;
                var pos = camera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = transform.parent.position.z;
                GraphMechanism.instance.ConnectionLine.SetPosition(1, pos);

                /*foreach(var l in tempLines)
                {
                    l.SetPosition(1, camera.ScreenToWorldPoint(Input.mousePosition));
                }*/

                if (Input.GetMouseButtonUp(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.transform != null)
                    {
                        if (hit.transform.tag == "Scene/Entity")
                        {
                            if (hit.transform != transform.parent)
                            {
                                GraphMechanism.instance.SecondSelectedEntity = hit.transform.GetComponent<EntitySceneHelper>();
                                
                                /*if (graphMechanism.selectedEntitys.Count > 1)
                                {
                                    foreach (var ent in createLink.EntitiesToConnect)
                                    {
                                        var curEntity = hit.transform.GetComponent<EntitySceneHelper>().CurrentEntity;
                                        if (ent[0].EntityObj.Uid != curEntity.EntityObj.Uid)
                                            ent[1] = curEntity;
                                        else
                                            ent[1] = null;
                                    }
                                }*/

                                CancelConnection();
                                GraphMechanism.instance.CreateConnection();
                            }
                            else
                                CancelConnection();
                        }
                        else
                            CancelConnection();
                    }
                    else
                        CancelConnection();
                }
                
                if (Input.GetMouseButtonDown(1))
                    CancelConnection();

                yield return null;
            }
        }

        void CancelConnection()
        {
            GraphMechanism.instance.ConnectionLine.gameObject.SetActive(false);
            if (ConnectionCoroutine != null)
                StopCoroutine(ConnectionCoroutine);
            
            /*foreach (var l in tempLines)
            {
                Destroy(l.gameObject);
            }
            tempLines.Clear();*/
        }
    }
}
