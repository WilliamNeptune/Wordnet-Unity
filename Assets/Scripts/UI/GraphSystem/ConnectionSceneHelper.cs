using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GraphSystem
{
    public class ConnectionSceneHelper : MonoBehaviour
    {
        public Connection CurrentConnection;
        ConnectionPointHelper pointHelper, bezierHelper;
        private void Start()
        {
            pointHelper = CurrentConnection.ConPoint.GetComponent<ConnectionPointHelper>();
            pointHelper.CurrentConnection = CurrentConnection;

            //bezierHelper = CurrentConnection.BezierPoint.GetComponent<ConnectionPointHelper>();
            //bezierHelper.CurrentConnection = CurrentConnection;
        }

        public void OnMouseEnter()
        {
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock  && CurrentConnection.ConMode == ConnectionMode.Straight && !Input.GetMouseButton(0))
            {
                foreach (var c in CurrentConnection.ConPoints)
                {
                    c.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* Current.orthographicSize / InitialOrtographicSize;
                    c.enabled = true;
                    c.color = CurrentConnection.ConnectionLineRenderer.startColor;
                }
            }
            if (!GraphMechanism.instance.ConnectionLine.gameObject.activeSelf && !GraphMechanism.instance.UIMouseBlock && CurrentConnection.ConMode == ConnectionMode.Bezier && !Input.GetMouseButton(0))
            {
                //foreach (var c in CurrentConnection.BezierPoints)
                foreach (var c in CurrentConnection.ConPoints)
                {
                    c.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* Current.orthographicSize / InitialOrtographicSize;
                    c.enabled = true;
                    c.color = CurrentConnection.ConnectionLineRenderer.startColor;
                }
            }
        }

        public void OnMouseExit()
        {
            if (!pointHelper.isDragging && CurrentConnection.ConMode == ConnectionMode.Straight)
                foreach (var c in CurrentConnection.ConPoints)
                {
                    if (!c.GetComponent<ConnectionPointHelper>().wasMoved)
                        c.enabled = false;
                }
            if (!pointHelper.isDragging && CurrentConnection.ConMode == ConnectionMode.Bezier)
                //foreach (var c in CurrentConnection.BezierPoints)
                foreach (var c in CurrentConnection.ConPoints)
                {
                    if (!c.GetComponent<ConnectionPointHelper>().wasMoved)
                        c.enabled = false;
                }
        }

        public (string, string) getSynonyms()
        {
            EntitySceneHelper es1 = CurrentConnection.Start;
            EntitySceneHelper es2 = CurrentConnection.End;
            string s1 = es1.getWord();
            string s2 = es2.getWord();
            return (s1, s2);
        }
    }       
}
