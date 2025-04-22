using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphSystem
{
    public class ChangeSelectedLineTypeHelper : MonoBehaviour
    {
        public static Connection SelectedLine = null;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), Input.mousePosition))
            {
                GraphMechanism.instance.UIMouseBlock = true;
            }
            else
            {
                GraphMechanism.instance.UIMouseBlock = false;
            }
        }

        private void OnMouseEnter()
        {
            GraphMechanism.instance.UIMouseBlock = true;
        }

        private void OnMouseOver()
        {
            GraphMechanism.instance.UIMouseBlock = true;
        }

        private void OnMouseExit()
        {
            GraphMechanism.instance.UIMouseBlock = false;
        }

        public void OnPointerClick()
        {
            if (SelectedLine != null)
                SelectedLine.ChangeModeForInstance();
        }
    }
}
