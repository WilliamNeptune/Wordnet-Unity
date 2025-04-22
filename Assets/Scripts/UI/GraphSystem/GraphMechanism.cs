using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using DG.Tweening;

namespace GraphSystem
{                                                                                           
    public class GraphMechanism : MonoBehaviour
    {
        public static GraphMechanism instance;

        //public GameObject EntityPrefab;
        //public GameObject EntityPrefabBranch;
        public GameObject ConnectionPrefab;
        public GameObject LinePointPrefab;
        //public GameObject ChangeModeButton;
        public Transform EntityParent;
        public Transform ConnectionParent;
        public LineRenderer ConnectionLine;
        public Image LineColor;
        public Material FullMaterial, CutMaterial, DotMaterial;
        public Sprite OneDirection, TwoDirection;
        //public Toggle ShowConnectionLabels;
        //public RectTransform LineTypeList, LineStyleList, LineDirectionList;

        [HideInInspector]
        public int LineType;
        [HideInInspector]
        public int LineStyle;
        [HideInInspector]
        public int LineDirection;
        public List<GameObject> Entities = new List<GameObject>();
        [HideInInspector]
        public List<Connection> Connections = new List<Connection>();
        [HideInInspector]
        public GameObject DragableObject;
        [HideInInspector]
        public EntitySceneHelper EntityHepler;
        [HideInInspector]
        public ConnectionSceneHelper ConnectionHepler;
        Vector2 DeltaPos = new Vector2(0, 0);
        [HideInInspector]
        public Camera cam;
        Vector3 startPointForMove;
        //LineRenderer ConnectionLineRenderer;
        [HideInInspector]
        public EntitySceneHelper FirstSelectedEntity, SecondSelectedEntity;
        [HideInInspector]
        public float DefaultConnectionLabelSize = 1;
        [HideInInspector]
        public float DefaultEntitySize = 1f;
        [HideInInspector]
        public float DefaultConnectionSize = 1f;
        [HideInInspector]
        public bool isConnectionPointDragging = false, UIMouseBlock = false;
        [HideInInspector]
        public Image previousStyleImage, previousTypeImage, previousDirectionImage;

        private void Start()
        {
            cam = Camera.main;
            // previousStyleImage = LineStyleList.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
            // previousTypeImage = LineTypeList.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
            // previousDirectionImage = LineDirectionList.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();

        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            // if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
            // {
            //     CreateEntityByMouse();
            // }

            // if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
            // {
            //     RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //     if (hit.transform != null)
            //     {
            //         UnityEngine.Debug.Log("hi11111111t");
            //         if (hit.transform.tag == "Scene/Point")
            //             hit.transform.GetComponent<ConnectionPointHelper>().RemovePoint();
            //     }
            // }
            
            // if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
            // {
            //     RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //     if (hit.transform != null)
            //     {
            //         if (hit.transform.tag == "Scene/Connection")
            //             hit.transform.GetComponent<ConnectionSceneHelper>().CurrentConnection.RemoveInstanse();
            //     }
            // }

            DragEntityInUpdate();

            // #region Move

            // if (Input.GetMouseButtonDown(1))
            // {
            //     startPointForMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            // }

            // if (Input.GetMouseButton(1))
            // {
            //     MoveCamera();
            // }
            
            // #endregion

        }

        #region Camera
        public void MoveCamera()
        {
            var mousePosForMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            var moveVector = mousePosForMove - startPointForMove;
            cam.transform.position += new Vector3(-moveVector.x/100, -moveVector.y/100, 0);

            startPointForMove = mousePosForMove;
        }
        #endregion

        #region Entity
        public void CreateEntityByMouse()
        {
            // var g = Instantiate(EntityPrefab);
            // g.transform.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
            // g.transform.position -= new Vector3(0, 0, 0.1f);
            // g.transform.parent = EntityParent;
            // g.transform.localScale = Vector3.one * DefaultEntitySize; //* CurrentOrthographicSize / InitialOrtographicSize;
            // Entities.Add(g);
        }

        public void DestroyEntity(GameObject EntityObject)
        {
            if (Entities.Contains(EntityObject))
                Entities.Remove(EntityObject);

            List<Connection> connections = new List<Connection>(EntityObject.GetComponent<EntitySceneHelper>().connections);
            foreach(var c in connections)
            {
                c.RemoveInstanse();
            }

            Destroy(EntityObject);
        }

        public void DragEntityInUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                Vector2 rayOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero); // the camera rotation must be 0 in x or y axis
                if (hit.transform != null)
                {
                    
                    DragableObject = hit.transform.gameObject;
                    EntityHepler = DragableObject.GetComponent<EntitySceneHelper>();
                        

                    if (ConnectionHepler != null && ConnectionHepler.transform != DragableObject.transform)
                        ConnectionHepler.CurrentConnection.SelectLine(false);

                    ConnectionHepler = DragableObject.GetComponent<ConnectionSceneHelper>();

                    if (EntityHepler != null)
                    {
                       //DeltaPos = (Vector2)DragableObject.transform.position - (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
                        string word = EntityHepler.getWord();
                        if(!WordnetPanel.instance.isSameLemma(word))
                        {
                            EntityHepler.setObjectSize(0.65f, 0.15f);
                        }
                    }

                    if (ConnectionHepler != null)
                    {
                        UnityEngine.Debug.Log(hit.transform);
                        ConnectionHepler.CurrentConnection.SelectLine(true);
                        //ChangeModeButton.SetActive(true);
                        ChangeSelectedLineTypeHelper.SelectedLine = ConnectionHepler.CurrentConnection;

                        (string s1, string s2) = ConnectionHepler.getSynonyms();
                        if(!PanelManager.isSynonymComparePanelOpen())
                        {
                            Debug.Log("TryToGetAIComparsion: " + s1 + " " + s2);

                            GameObject synonymComparePanel = PanelManager.CreatePanel(ContainerPanel.instance.transform, "SynonymComparePanel", new Vector2(0, 0), true);
                            PanelManager.OpenPanelVertically(synonymComparePanel, null, 0);

                            ServerManager.instance.TryToGetAIComparsion(s1, s2);
                        }
                    }
                    else
                    {
                        if (!UIMouseBlock)
                        {
                            //ChangeModeButton.SetActive(false);
                            ChangeSelectedLineTypeHelper.SelectedLine = null;
                        }
                    }
                }
                else
                {
                    DragableObject = null;
                    if (ConnectionHepler != null)
                        ConnectionHepler.CurrentConnection.SelectLine(false);

                    if (!UIMouseBlock)
                    {
                        //ChangeModeButton.SetActive(false);
                        ChangeSelectedLineTypeHelper.SelectedLine = null;
                    }
                }
            }

            // if (Input.GetMouseButton(0) && DragableObject != null)
            //     if (EntityHepler != null)
            //         DragEntity();

            if (Input.GetMouseButtonUp(0) && DragableObject != null)
            {
                if (EntityHepler != null)
                {
                    //EndDragEntity();
                    string word = EntityHepler.getWord();
                    if(!WordnetPanel.instance.isSameLemma(word)&&!PanelManager.isSynonymComparePanelOpen())
                    {
                        WordnetPanel.instance.updateLemma(word);
                        EntityHepler.switchToCurrentLemma();
                    }
                }

                DragableObject = null;
            }
        }

        public void DragEntity()
        {
            var pos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
            DragableObject.transform.position = pos + DeltaPos;
            DragableObject.transform.position = new Vector3(DragableObject.transform.position.x, DragableObject.transform.position.y, -0.1f);
            DragableObject.transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = false;

            foreach (var c in EntityHepler.connections)
            {
                c.UpdatePositions();
            }
        }

        public void EndDragEntity()
        {
            foreach (var c in EntityHepler.connections)
            {
                c.UpdateEdgeCollider();
            }
        }
        #endregion

        #region Lines

        public void ChangeConnectionMode(int i)
        {
            LineType = i;
        }

        public void CreateConnection()
        {
            //Check for exist connection
            bool hasConnection = false;
            foreach (var c in Connections)
            {
                if ((c.Start == FirstSelectedEntity || c.End == FirstSelectedEntity) && (c.Start == SecondSelectedEntity || c.End == SecondSelectedEntity))
                    hasConnection = true;
            }

            if (!hasConnection)
            {
                Connection NewConnection = new Connection(Instantiate(ConnectionPrefab, ConnectionParent),
                FirstSelectedEntity, SecondSelectedEntity);

                NewConnection.ConnectionGObj.GetComponent<ConnectionSceneHelper>().CurrentConnection = NewConnection;
                Connections.Add(NewConnection);
            }
        }

        public void CreateConnectionForWordnet()
        {
            Connection NewConnection = new Connection(Instantiate(ConnectionPrefab, ConnectionParent),
            FirstSelectedEntity, SecondSelectedEntity);

            NewConnection.ConnectionGObj.GetComponent<ConnectionSceneHelper>().CurrentConnection = NewConnection;
            Connections.Add(NewConnection);
        }

        public void setAllAnimations(bool isOn)
        {
            foreach (var c in Connections)
            {
                // c.LineAnimator.enabled = isOn;
                // c.LineAnimator.SetTrigger("on");


                c.ConnectionLineRenderer.DOColor(new Color2(Color.white, Color.white), new Color2(Color.white, Color.red),3).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                //c.ConnectionGObj.transform.DOScale(2f, 3).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear);

                DOTween.To(
                    () => c.ConnectionLineRenderer.widthMultiplier, 
                   x => c.ConnectionLineRenderer.widthMultiplier = x, 
                    0.7f, 3)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
            }
        }
       #endregion

        #region Customization
        //Coroutine TypeCoroutine, StyleCoroutine, DirectionCoroutine;
        // public void ShowLineTypeList()
        // {
        //     LineTypeList.GetComponent<Animator>().SetTrigger("on");
        //     TypeCoroutine = StartCoroutine(CheckOutsideClickForTypeList());
        // }

        // IEnumerator CheckOutsideClickForTypeList()
        // {
        //     while (true)
        //     {
        //         if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(LineTypeList, Input.mousePosition))
        //         {
        //             LineTypeList.GetComponent<Animator>().SetTrigger("off");
        //             yield break;
        //         }
        //         else
        //         {
        //             yield return null;
        //         }
        //     }
        // }

        // public void ShowLineStyleList()
        // {
        //     LineStyleList.GetComponent<Animator>().SetTrigger("on");
        //     StyleCoroutine = StartCoroutine(CheckOutsideClickForStyleList());
        // }

        // IEnumerator CheckOutsideClickForStyleList()
        // {
        //     while (true)
        //     {
        //         if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(LineStyleList, Input.mousePosition))
        //         {
        //             LineStyleList.GetComponent<Animator>().SetTrigger("off");
        //             yield break;
        //         }
        //         else
        //         {
        //             yield return null;
        //         }
        //     }
        // }

        // public void ShowLineDirectionList()
        // {
        //     LineDirectionList.GetComponent<Animator>().SetTrigger("on");
        //     DirectionCoroutine = StartCoroutine(CheckOutsideClickForDirectionList());
        // }

        // IEnumerator CheckOutsideClickForDirectionList()
        // {
        //     while (true)
        //     {
        //         if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(LineDirectionList, Input.mousePosition))
        //         {
        //             LineDirectionList.GetComponent<Animator>().SetTrigger("off");
        //             yield break;
        //         }
        //         else
        //         {
        //             yield return null;
        //         }
        //     }
        // }
        #endregion
    }

    public class Connection
    {
        public GameObject ConnectionGObj;
        public LineRenderer ConnectionLineRenderer;
        public EdgeCollider2D EdgeCollider;
        public Rigidbody2D Rigidbody;
        //[HideInInspector]
        //public Transform SnapLink;
        [HideInInspector]
        public ConnectionMode ConMode = ConnectionMode.Bezier;

        int LineType;
        int LineStyle;
        int LineDirection;
        string LabelText;
        bool ShowLineText;

        public EntitySceneHelper Start, End;
        public ConnectionPoint StartSide;
        public ConnectionPoint EndSide;
        public Vector3 LabelPos = new Vector3(1000, 1000, 1000);

        private float ConnectOffset;
        public List<Vector3> ConnectionPoints = new List<Vector3>();

        public static float defaultLineWidth = 0.25f;
        //float widthMultiply = 1;
        [HideInInspector]
        public int cameraSize = 1080;
        public Color CurrentLineColor = Color.white;
        public GameObject startArrow, endArrow;
        public Animator LineAnimator;
        public TextMesh connectionLabel;
        Transform textMesh;
        public SpriteRenderer startArrowSprite, endArrowSprite;
        public bool isInaccurateLine = false;

        public SpriteRenderer ConPoint;
        public bool PointsWereChanged = false;
        ConnectionPointHelper conHelper;
        public List<SpriteRenderer> ConPoints = new List<SpriteRenderer>();
        public List<ConnectionPointHelper> ConPointHelpers = new List<ConnectionPointHelper>();
        public List<Vector3> straightPoints = new List<Vector3>(), bezierPoints = new List<Vector3>();

        public Connection(GameObject connectionGO, EntitySceneHelper start, EntitySceneHelper end)
        {
            ConnectionGObj = connectionGO;
            Start = start;
            StartSide = new ConnectionPoint();
            Start.connections.Add(this);
            End = end;
            EndSide = new ConnectionPoint();
            End.connections.Add(this);

            ConnectionLineRenderer = ConnectionGObj.GetComponent<LineRenderer>();
            CurrentLineColor = GraphMechanism.instance.LineColor.color;
            ConnectionLineRenderer.endColor = CurrentLineColor;
            ConnectionLineRenderer.startColor = CurrentLineColor;
            LineType = GraphMechanism.instance.LineType;
            LineStyle = GraphMechanism.instance.LineStyle;
            LineDirection = GraphMechanism.instance.LineDirection;
            if (LineStyle == 0)
                ConnectionLineRenderer.material = GraphMechanism.instance.FullMaterial;
            else if (LineStyle == 1)
                ConnectionLineRenderer.material = GraphMechanism.instance.CutMaterial;
            else if (LineStyle == 2)
                ConnectionLineRenderer.material = GraphMechanism.instance.DotMaterial;
            LabelText = "Your example text";
            //ShowLineText = GraphMechanism.instance.ShowConnectionLabels.isOn;

            ConnectionLineRenderer.startWidth = defaultLineWidth;
            ConnectionLineRenderer.endWidth = defaultLineWidth;
            EdgeCollider = ConnectionGObj.GetComponent<EdgeCollider2D>();
            Rigidbody = ConnectionGObj.GetComponent<Rigidbody2D>();
            LineAnimator = ConnectionGObj.GetComponent<Animator>() ? ConnectionGObj.GetComponent<Animator>() : null;
            //ConnectOffset = Start.gameObject.GetComponent<SpriteRenderer>().size.x / 2;

            startArrow = ConnectionLineRenderer.transform.GetChild(0).gameObject;
            endArrow = ConnectionLineRenderer.transform.GetChild(1).gameObject;
            startArrowSprite = startArrow.transform.GetChild(0).GetComponent<SpriteRenderer>();
            endArrowSprite = endArrow.transform.GetChild(0).GetComponent<SpriteRenderer>();
            startArrowSprite.color = ConnectionLineRenderer.startColor;
            endArrowSprite.color = ConnectionLineRenderer.startColor;

            if (LineType == 0)
                ConMode = ConnectionMode.Bezier;
            else if (LineType == 1)
                ConMode = ConnectionMode.Straight;

            //Comment this block if you plan to save points somewhere, and uncomment CreateInitialPoints() below ---------//
            ConPoint = GameObject.Instantiate(GraphMechanism.instance.LinePointPrefab).GetComponent<SpriteRenderer>();    //
            ConPoint.gameObject.SetActive(false);
            conHelper = ConPoint.GetComponent<ConnectionPointHelper>();                                                   //
            ConPoint.transform.parent = ConnectionGObj.transform.parent;                                                  //
            ConPoints.Add(ConPoint);                                                                                      //
            ConPointHelpers.Add(conHelper);                                                                               //
            conHelper.positionNum = 1;                                                                                    //
            //------------------------------------------------------------------------------------------------------------//
                                                                                                                        //
            //CreateInitialPoints(); <------------------------------------------------------------------------------------//
            
            //-- text label
            textMesh = ConnectionLineRenderer.transform.GetChild(3);
            connectionLabel = textMesh.GetComponent<TextMesh>();
            connectionLabel.color = Color.white;

            UpdateLabelText();
            if (!ConnectionGObj.activeSelf || !ConnectionGObj.transform.parent.gameObject.activeSelf)
                SetLabelVisible(false);
            //----------------

            DefineConnectionSide();
            UpdatePositions();
            UpdateEdgeCollider();

            //Comment this block if you plan to save points somewhere---//
            SetConPointToCenter(ConPoint, conHelper);                   //
            //----------------------------------------------------------//

            SetPointsColor();
        }

        public void SetConPointToCenter(SpriteRenderer ConPoint, ConnectionPointHelper conHelper)
        {
            ConPoint.transform.position = GetBezierPoint(0.5f);
            ConPoint.transform.position = new Vector3(ConPoint.transform.position.x, ConPoint.transform.position.y, connectionLabel.transform.position.z - 0.25f);
            conHelper.controlPoint = ConPoint.transform.position;
            conHelper.controlPoint.z = 0;
            ConPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;
            ConPoint.color = ConnectionLineRenderer.startColor;
        }

        public void SetPointsColor()
        {
            foreach (var p in ConPoints)
            {
                p.color = ConnectionLineRenderer.startColor;
            }
        }

        public void SetPointsSize()
        {
            foreach (var p in ConPoints)
            {
                p.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;
            }
        }

        //------
        #region save points
        //if you plan to save points uncomment the strings inside these functions
        public void SaveBezierPoints()
        {
            if (PointsWereChanged)
            {
                var floatArray = new List<float>();
                for (int i = 0; i < bezierPoints.Count; i++)
                {
                    floatArray.Add(bezierPoints[i].x);
                    floatArray.Add(bezierPoints[i].y);
                    floatArray.Add(bezierPoints[i].z);
                }

                //Put "floatArray" into your list for saving: "ExampleBezierPoints" = new List<Vector3>(floatArray);
            }
            else
            {
                //Clear your list for saving: "ExampleBezierPoints".Clear();
            } 
        }

        //if you plan to save points uncomment this function
        public void SaveStraightPoints()
        {
            if (PointsWereChanged)
            {
                var floatArray = new List<float>();
                floatArray.Add(ConnectionLineRenderer.GetPosition(0).x);
                floatArray.Add(ConnectionLineRenderer.GetPosition(0).y);
                floatArray.Add(ConnectionLineRenderer.GetPosition(0).z);
                for (int i = 0; i < ConPointHelpers.Count; i++)
                {
                    if (ConPointHelpers[i].wasMoved)
                    {
                        floatArray.Add(ConPointHelpers[i].controlPoint.x);
                        floatArray.Add(ConPointHelpers[i].controlPoint.y);
                        floatArray.Add(ConPointHelpers[i].controlPoint.z);
                    }
                }
                floatArray.Add(ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1).x);
                floatArray.Add(ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1).y);
                floatArray.Add(ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1).z);

                //Put "floatArray" into your list for saving: "ExampleBezierPoints" = new List<Vector3>(floatArray);
            }
            else
            {
                //Clear your list for saving: "ExampleBezierPoints".Clear();
            }
        }

        public void CreateInitialPoints(List<Vector3> savedPoints)
        {
            bool flag = false;

            var vectorList = new List<Vector3>();
            if (savedPoints.Count > 0)
                vectorList = new List<Vector3>(savedPoints);
                
            var vectorListForStraight = new List<Vector3>();

            if (vectorList.Count > 0)
            {
                vectorListForStraight = new List<Vector3>(vectorList);
                if (vectorListForStraight[vectorListForStraight.Count - 1] == vectorListForStraight[vectorListForStraight.Count - 2])
                    vectorListForStraight.Remove(vectorListForStraight[vectorListForStraight.Count - 1]);

                var p_ = vectorList[vectorList.Count - 1];
                if (vectorList[vectorList.Count - 1] != vectorList[vectorList.Count - 2])
                    vectorList.Add(p_);
            }

            //straight points
            //---------------------------------------------------------------------------------------
            if (ConMode == ConnectionMode.Straight)
            {
                ConPoint = GameObject.Instantiate(GraphMechanism.instance.LinePointPrefab).GetComponent<SpriteRenderer>();
                conHelper = ConPoint.GetComponent<ConnectionPointHelper>();
                ConPoint.transform.parent = ConnectionGObj.transform.parent;
                ConPoints.Add(ConPoint);
                ConPointHelpers.Add(conHelper);
                conHelper.positionNum = 1;

                ConPoint.gameObject.SetActive(false);

                if (vectorListForStraight.Count > 0)
                {
                    PointsWereChanged = true;
                    flag = true;
                    ConPoint.transform.position = vectorListForStraight[1];
                    ConPoint.enabled = true;
                    ConPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    conHelper.wasMoved = true;
                    conHelper.isDragging = false;
                    conHelper.positionNum = 2;
                    conHelper.CurrentConnection = this;
                }
                else
                    ConPoint.transform.position = GetBezierPoint(0.5f);

                ConPoint.transform.position = new Vector3(ConPoint.transform.position.x, ConPoint.transform.position.y, -0.25f);
                conHelper.controlPoint = ConPoint.transform.position;
                conHelper.controlPoint.z = 0;
                ConPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;
                ConPoint.color = ConnectionLineRenderer.startColor;
                int count = 1, tempCount = 0;

                foreach (var p in vectorListForStraight)
                {
                    if (tempCount != 0)
                    {
                        var rhPoint = GameObject.Instantiate(ConPoint.gameObject);
                        rhPoint.transform.parent = ConPoint.transform.parent;
                        var rhHelper = rhPoint.GetComponent<ConnectionPointHelper>();
                        if (tempCount == 1)
                        {
                            rhHelper.positionNum = count;
                            count++;
                        }
                        else
                        {
                            rhHelper.positionNum = count + 1;
                            count++;
                        }
                        rhHelper.CurrentConnection = this;
                        var rhSprite = rhPoint.GetComponent<SpriteRenderer>();
                        rhHelper.isDragging = false;
                        var pos = GetBezierPointBetweenTwoPoints(p, vectorListForStraight[tempCount - 1], 0.5f);

                        {
                            rhHelper.wasMoved = false;
                            rhSprite.enabled = false;
                            rhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                        }

                        rhPoint.transform.position = pos;
                        rhPoint.transform.position = new Vector3(rhPoint.transform.position.x, rhPoint.transform.position.y, ConPoint.transform.position.z);
                        rhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;

                        ConPoints.Add(rhSprite);
                        ConPointHelpers.Add(rhHelper);
                        rhHelper.controlPoint = rhPoint.transform.position;
                        rhHelper.controlPoint.z = 0;

                        if (tempCount != 1 && tempCount != vectorListForStraight.Count - 1)
                        {
                            var Point = GameObject.Instantiate(ConPoint.gameObject);
                            Point.transform.parent = ConPoint.transform.parent;
                            var Helper = Point.GetComponent<ConnectionPointHelper>();
                            Helper.positionNum = count + 1;
                            count++;
                            Helper.CurrentConnection = this;
                            var Sprite = Point.GetComponent<SpriteRenderer>();
                            Helper.isDragging = false;
                            Helper.wasMoved = true;
                            Sprite.enabled = true;
                            Point.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

                            Point.transform.position = p;
                            Point.transform.position = new Vector3(Point.transform.position.x, Point.transform.position.y, ConPoint.transform.position.z);
                            Point.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;

                            ConPoints.Add(Sprite);
                            ConPointHelpers.Add(Helper);
                            Helper.controlPoint = Point.transform.position;
                            Helper.controlPoint.z = 0;
                        }
                    }
                    tempCount++;
                }

                var temp = ConPointHelpers.OrderBy(ConnectionPointHelper => ConnectionPointHelper.positionNum).ToList();
                ConPointHelpers.Clear();
                ConPointHelpers = new List<ConnectionPointHelper>(temp);

                if (vectorListForStraight.Count > 0)
                {
                    straightPoints.Add(vectorListForStraight[0]);
                    for (int i = 0; i < ConPointHelpers.Count; i++)
                    {
                        straightPoints.Add(ConPointHelpers[i].controlPoint);
                    }
                    straightPoints.Add(vectorListForStraight[vectorListForStraight.Count - 1]);
                }
                bezierPoints = new List<Vector3>(vectorList);
            }

            //---------------------------------------------------------------------------------------

            //bezier points
            //---------------------------------------------------------------------------------------
            if (ConMode == ConnectionMode.Bezier)
            {
                DefineConnectionSide();
                StartSide.CalculateVectors(Start.transform);
                EndSide.CalculateVectors(End.transform);

                ConPoints.Clear();
                ConPointHelpers.Clear();
                straightPoints.Clear();

                ConPoint = GameObject.Instantiate(GraphMechanism.instance.LinePointPrefab).GetComponent<SpriteRenderer>();
                ConPoint.gameObject.SetActive(false);
                conHelper = ConPoint.GetComponent<ConnectionPointHelper>();
                ConPoint.transform.parent = ConnectionGObj.transform.parent;
                ConPoints.Add(ConPoint);
                ConPointHelpers.Add(conHelper);
                conHelper.positionNum = 1;

                bool flag2 = false;
                if (vectorList.Count > 0)
                {
                    flag2 = true;
                    {
                        PointsWereChanged = true;
                        ConPoint.enabled = true;
                        ConPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

                        ConPoint.transform.position = vectorList[1];

                        conHelper.wasMoved = true;
                        conHelper.positionNum = 2;
                    }
                    conHelper.isDragging = false;
                }
                else
                    ConPoint.transform.position = GetBezierPoint(0.5f);

                ConPoint.transform.position = new Vector3(ConPoint.transform.position.x, ConPoint.transform.position.y, -0.25f);
                conHelper.controlPoint = ConPoint.transform.position;
                conHelper.controlPoint.z = 0;
                ConPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;
                ConPoint.color = ConnectionLineRenderer.startColor;

                int count = 1, tempCount = 0;

                foreach (var p in vectorList)
                {
                    if (tempCount != 0 && tempCount != vectorList.Count - 1)
                    {
                        {
                            var lhPoint = GameObject.Instantiate(ConPoint.gameObject);
                            lhPoint.transform.parent = ConPoint.transform.parent;
                            var lhHelper = lhPoint.GetComponent<ConnectionPointHelper>();
                            if (p == vectorList[1])
                            {
                                lhHelper.positionNum = count;
                                count++;
                            }
                            else
                            {
                                lhHelper.positionNum = count + 1;
                                count++;
                            }
                            lhHelper.CurrentConnection = this;
                            var lhSprite = lhPoint.GetComponent<SpriteRenderer>();
                            lhHelper.isDragging = false;
                            lhHelper.wasMoved = false;
                            lhSprite.enabled = false;
                            lhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                            lhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;

                            ConPoints.Add(lhSprite);
                            ConPointHelpers.Add(lhHelper);
                        }

                        if (tempCount != 1 && tempCount != vectorList.Count - 2)
                        {
                            var rhPoint = GameObject.Instantiate(ConPoint.gameObject);
                            rhPoint.transform.parent = ConPoint.transform.parent;
                            var rhHelper = rhPoint.GetComponent<ConnectionPointHelper>();
                            rhHelper.positionNum = count + 1;
                            count++;
                            rhHelper.CurrentConnection = this;
                            var rhSprite = rhPoint.GetComponent<SpriteRenderer>();
                            rhHelper.isDragging = false;
                            rhHelper.wasMoved = true;
                            rhSprite.enabled = true;
                            rhPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

                            rhPoint.transform.position = p;
                            rhPoint.transform.position = new Vector3(rhPoint.transform.position.x, rhPoint.transform.position.y, ConPoint.transform.position.z);
                            rhPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); //* CurrentOrthographicSize / InitialOrtographicSize;

                            ConPoints.Add(rhSprite);
                            ConPointHelpers.Add(rhHelper);
                            rhHelper.controlPoint = rhPoint.transform.position;
                            rhHelper.controlPoint.z = 0;
                        }

                    }
                    tempCount++;
                }

                var temp = ConPointHelpers.OrderBy(ConnectionPointHelper => ConnectionPointHelper.positionNum).ToList();
                ConPointHelpers.Clear();
                ConPointHelpers = new List<ConnectionPointHelper>(temp);

                {
                    bezierPoints = new List<Vector3>(vectorList);
                }

                if (vectorListForStraight.Count > 0)
                {
                    straightPoints.Add(vectorListForStraight[0]);
                    for (int i = 0; i < ConPointHelpers.Count; i++)
                    {
                        straightPoints.Add(ConPointHelpers[i].controlPoint);
                    }
                    straightPoints.Add(vectorListForStraight[vectorListForStraight.Count - 1]);
                }

            }

            //---------------------------------------------------------------------------------------
        }
        
        #endregion
        //------

        void DefineConnectionSide()
        {
            if (ConMode == ConnectionMode.Bezier || ConMode == ConnectionMode.Straight)
            {
                if (Mathf.Abs(Start.transform.position.y - End.transform.position.y) >= Mathf.Abs(Start.transform.position.x - End.transform.position.x)) 
                {
                    if (Start.transform.position.y > End.transform.position.y)
                    {
                        StartSide.Side = ConnectionSide.Down;
                        EndSide.Side = ConnectionSide.Up;
                    }
                    else
                    {
                        StartSide.Side = ConnectionSide.Up;
                        EndSide.Side = ConnectionSide.Down;
                    }

                    var d = 60 * Mathf.Abs(Start.transform.position.x - End.transform.position.x);
                    if (d < (cameraSize / 12))
                    {
                        StartSide.weight = 0;
                        EndSide.weight = 0;
                    }
                }
                else
                {
                    if (Start.transform.position.x > End.transform.position.x)
                    {
                        StartSide.Side = ConnectionSide.Right;
                        EndSide.Side = ConnectionSide.Left;
                    }
                    else
                    {
                        StartSide.Side = ConnectionSide.Left;
                        EndSide.Side = ConnectionSide.Right;
                    }

                    var d = 60 * Mathf.Abs(Start.transform.position.y - End.transform.position.y);

                    if (d < (cameraSize / 12))
                    {
                        StartSide.weight = 0;
                        EndSide.weight = 0;
                    }
                }
            }
        }

        void UpdateArrowPosition()
        {
            if (ConMode == ConnectionMode.Bezier)
            {
                startArrow.transform.position = GetBezierPoint(0.13f);
                endArrow.transform.position = GetBezierPoint(0.87f);
            }
            else
            {
                startArrow.transform.position = GetBezierPoint(0.27f);
                endArrow.transform.position = GetBezierPoint(0.73f);
            }

            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);


            int k = 1;
            if (LineDirection == 0) //0
            {
                startArrow.SetActive(false);
                endArrow.SetActive(false);

            }
            else if (LineDirection == 1)
            {
                {
                    startArrow.SetActive(true);
                    endArrow.SetActive(true);
                }

                startArrowSprite.sprite = GraphMechanism.instance.OneDirection;
                endArrowSprite.sprite = GraphMechanism.instance.OneDirection;

                k = -1;
            }
            else if (LineDirection == 2) //2
            {
                {
                    startArrow.SetActive(true);
                    endArrow.SetActive(true);
                }

                startArrowSprite.sprite = GraphMechanism.instance.TwoDirection;
                endArrowSprite.sprite = GraphMechanism.instance.TwoDirection;

                k = 1;
            }

            if (Start.transform.position.y <= End.transform.position.y)
            {
                startArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right * k, GetBezierPoint(0.14f) - GetBezierPoint(0.12f)) * k);
                endArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.right, GetBezierPoint(0.88f) - GetBezierPoint(0.86f)));

            }
            else
            {
                startArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right * k, GetBezierPoint(0.14f) - GetBezierPoint(0.12f)) * k);
                endArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(Vector2.right, GetBezierPoint(0.88f) - GetBezierPoint(0.86f)));
            }

            int distance = 60 * Mathf.RoundToInt(Vector2.Distance(Start.transform.position, End.transform.position));

            if (distance < (cameraSize / 6))
            {
                startArrow.transform.position = GetBezierPoint(0.5f);
                endArrow.transform.position = GetBezierPoint(0.5f);
            }


            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);

            UpdateLabelPosition();
            CheckUIConfigPanelToggle(0);
        }

        public void SetLabelVisible(bool temp)
        {
            UpdateLabelText();
            connectionLabel.gameObject.SetActive(temp);
        }

        public void InverseLabel()
        {
            if (Start.transform.position.y <= End.transform.position.y && Start.transform.position.x >= End.transform.position.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (Start.transform.position.y <= End.transform.position.y && Start.transform.position.x < End.transform.position.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }
            if (Start.transform.position.y > End.transform.position.y && Start.transform.position.x >= End.transform.position.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (Start.transform.position.y > End.transform.position.y && Start.transform.position.x < End.transform.position.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }
        }

        public void CheckUIConfigPanelToggle(int distance)
        {
            if (ShowLineText)
            {
                if (ConnectionGObj.activeSelf)
                    SetLabelVisible(true);
            }
            else
            {
                SetLabelVisible(false);
            }
        }

        public void UpdateLabelText()
        {
            //you can use start and end tags for customization (bold, italic, etc)
            string startTag = "";
            string endTag = "";
            if (connectionLabel.text != startTag + LabelText + endTag)
                connectionLabel.text = startTag + LabelText + endTag;
        }

        public void UpdateLabelPosition()
        {
            if (Start.transform.position.y <= End.transform.position.y)
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right, GetBezierPoint(0.55f) - GetBezierPoint(0.45f)));
            else
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right, GetBezierPoint(0.55f) - GetBezierPoint(0.45f)));

            InverseLabel();

            int distance = 60 * Mathf.RoundToInt(Vector2.Distance(Start.transform.position, End.transform.position));

            textMesh.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConnectionGObj.transform.position.z);

            var shift = textMesh.up * 0.3f;

            float k = 1; // * InitialOrtographicSize / YourCurrentCamera.orthographicSize;
            connectionLabel.characterSize = 1 / k * GraphMechanism.instance.DefaultConnectionLabelSize;
            shift = textMesh.up * 0.3f / k * GraphMechanism.instance.DefaultConnectionLabelSize;

            if (textMesh.localScale.x > 0)
                textMesh.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConnectionGObj.transform.position.z) + shift;
            if (textMesh.localScale.x < 0)
                textMesh.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConnectionGObj.transform.position.z) - shift;

            UpdateLabelText();
            CheckUIConfigPanelToggle(0);
        }

        public void UpdateLinkWidthForGraph()
        {
            //var w = defaultLineWidth * GraphMechanism.instance.DefaultConnectionSize;
            float k = 0.4f; // * InitialOrtographicSize / YourCurrentCamera.orthographicSize;

            // ConnectionLineRenderer.startWidth = w / k;
            // ConnectionLineRenderer.endWidth = w / k;
            
            startArrow.transform.localScale = new Vector3(0.04f / k, 0.04f / k, 0.04f / k) * GraphMechanism.instance.DefaultConnectionSize;
            endArrow.transform.localScale = new Vector3(0.04f / k, 0.04f / k, 0.04f / k) * GraphMechanism.instance.DefaultConnectionSize;

            var shift = textMesh.up * 0.3f / k * GraphMechanism.instance.DefaultConnectionLabelSize;
            connectionLabel.characterSize = 1 / k * GraphMechanism.instance.DefaultConnectionLabelSize;
            if (!PointsWereChanged)
            {
                if (textMesh.localScale.x > 0)
                    textMesh.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConnectionGObj.transform.position.z) + shift;
                if (textMesh.localScale.x < 0)
                    textMesh.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConnectionGObj.transform.position.z) - shift;
            }
            else if (ConMode == ConnectionMode.Straight && PointsWereChanged)
            {
                var startFirst = ConnectionLineRenderer.GetPosition(0);
                var endFirst = ConnectionLineRenderer.GetPosition(2);
                var startSecond = ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 3);
                var endSecond = ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1);

                if (Vector2.Distance(startFirst, endFirst) <= Vector2.Distance(startSecond, endSecond))
                {
                    startFirst = startSecond;
                    endFirst = endSecond;
                }

                if (textMesh.localScale.x > 0)
                    textMesh.position = new Vector3(GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).x, GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).y, ConnectionGObj.transform.position.z) + shift;
                if (textMesh.localScale.x < 0)
                    textMesh.position = new Vector3(GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).x, GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).y, ConnectionGObj.transform.position.z) - shift;
            }
            else if (ConMode == ConnectionMode.Bezier && PointsWereChanged)
            {
                var startPoints = new List<Vector3>();
                var endPoints = new List<Vector3>();

                var startFirst = bezierPoints[1];
                var endFirst = bezierPoints[2];
                var prev = bezierPoints[0];

                var pS = bezierPoints.Count - 5;
                if (pS < 0)
                    pS = 0;
                var startSecond = bezierPoints[bezierPoints.Count - 4];
                var endSecond = bezierPoints[bezierPoints.Count - 3];
                var prevSecond = bezierPoints[pS];

                startPoints.Add(startFirst);
                startPoints.Add(endFirst);
                startPoints.Add(bezierPoints[3]);

                endPoints.Add(startSecond);
                endPoints.Add(endSecond);
                endPoints.Add(bezierPoints[bezierPoints.Count - 2]);

                float dl = 0.2f;
                float dr = 0.8f;

                if (ConPointHelpers.Count > 3)
                {
                    startFirst = ConnectionLineRenderer.GetPosition(0);
                    endFirst = bezierPoints[1];
                    dl = 0.45f;
                    dr = 0.55f;

                    startPoints.Clear();
                    startPoints.Add(startFirst);
                    startPoints.Add(endFirst);
                    startPoints.Add(bezierPoints[2]);

                    startSecond = bezierPoints[bezierPoints.Count - 3];
                    endSecond = bezierPoints[bezierPoints.Count - 1];
                    prevSecond = bezierPoints[bezierPoints.Count - 4];

                    endPoints.Clear();
                    endPoints.Add(startSecond);
                    endPoints.Add(endSecond);
                    endPoints.Add(endSecond);
                }

                var pointsForText = new List<Vector3>(startPoints);
                var startForText = startFirst;
                var endForText = endFirst;
                var prevForText = prev;

                if (Vector2.Distance(startFirst, endFirst) <= Vector2.Distance(startSecond, endSecond))
                {
                    startForText = startSecond;
                    endForText = endSecond;

                    pointsForText.Clear();
                    prevForText = prevSecond;
                    pointsForText = new List<Vector3>(endPoints);
                }

                var textPos = GetPointFromCurve(pointsForText, 0.5f, prevForText.y, prevForText.x);
                textPos.z = ConnectionGObj.transform.position.z;

                if (textMesh.localScale.x > 0)
                    textMesh.position = textPos + shift;
                if (textMesh.localScale.x < 0)
                    textMesh.position = textPos - shift;
            }
        }

        public void UpdateLabelPositionForChangedBezierLine(List<Vector3> points, Vector3 startFirst, Vector3 endFirst, Vector3 prev)
        {
            if (startFirst.y <= endFirst.y)
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right, endFirst - startFirst));
            else
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right, endFirst - startFirst));

            if (startFirst.y <= endFirst.y && startFirst.x >= endFirst.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (startFirst.y <= endFirst.y && startFirst.x < endFirst.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }
            if (startFirst.y > endFirst.y && startFirst.x >= endFirst.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (startFirst.y > endFirst.y && startFirst.x < endFirst.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }

            var shift = textMesh.up * 0.3f;
            float k = 1; // * InitialOrtographicSize / YourCurrentCamera.orthographicSize;
            connectionLabel.characterSize = 1 / k * GraphMechanism.instance.DefaultConnectionLabelSize;
            shift = textMesh.up * 0.3f / k * GraphMechanism.instance.DefaultConnectionLabelSize;

            var textPos = GetPointFromCurve(points, 0.5f, prev.y, prev.x);
            textPos.z = ConnectionGObj.transform.position.z;

            if (textMesh.localScale.x > 0)
                textMesh.position = textPos + shift;
            if (textMesh.localScale.x < 0)
                textMesh.position = textPos - shift;

            UpdateLabelText();
            CheckUIConfigPanelToggle(0);
        }

        public void UpdateLabelPositionForChangedLine()
        {
            var startFirst = ConnectionLineRenderer.GetPosition(0);
            var endFirst = ConnectionLineRenderer.GetPosition(2);
            var startSecond = ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 3);
            var endSecond = ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1);

            if (Vector2.Distance(startFirst, endFirst) <= Vector2.Distance(startSecond, endSecond))
            {
                startFirst = startSecond;
                endFirst = endSecond;
            }

            if (startFirst.y <= endFirst.y)
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right, endFirst - startFirst));
            else
                connectionLabel.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right, endFirst - startFirst));

            //
            if (startFirst.y <= endFirst.y && startFirst.x >= endFirst.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (startFirst.y <= endFirst.y && startFirst.x < endFirst.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }
            if (startFirst.y > endFirst.y && startFirst.x >= endFirst.x)
            {
                textMesh.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            if (startFirst.y > endFirst.y && startFirst.x < endFirst.x)
            {
                textMesh.localScale = new Vector3(-0.1f, -0.1f, 0.1f);
            }
            //

            textMesh.position = new Vector3(GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).x, GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).y, ConnectionGObj.transform.position.z);

            var shift = textMesh.up * 0.3f;
            float k = 1; // * InitialOrtographicSize / YourCurrentCamera.orthographicSize;
            connectionLabel.characterSize = 1 / k * GraphMechanism.instance.DefaultConnectionLabelSize;
            shift = textMesh.up * 0.3f / k * GraphMechanism.instance.DefaultConnectionLabelSize;

            if (textMesh.localScale.x > 0)
                textMesh.position = new Vector3(GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).x, GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).y, ConnectionGObj.transform.position.z) + shift;
            if (textMesh.localScale.x < 0)
                textMesh.position = new Vector3(GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).x, GetBezierPointBetweenTwoPoints(startFirst, endFirst, 0.5f).y, ConnectionGObj.transform.position.z) - shift;

            UpdateLabelText();
            CheckUIConfigPanelToggle(0);
        }

        public void UpdateArrowsForChangedBezierLine()
        {
            var startPoints = new List<Vector3>();
            var endPoints = new List<Vector3>();

            var startFirst = bezierPoints[1];
            var endFirst = bezierPoints[2];
            var prev = bezierPoints[0];

            var pS = bezierPoints.Count - 5;
            if (pS < 0)
                pS = 0;
            var startSecond = bezierPoints[bezierPoints.Count - 4];
            var endSecond = bezierPoints[bezierPoints.Count - 3];
            var prevSecond = bezierPoints[pS];

            startPoints.Add(startFirst);
            startPoints.Add(endFirst);
            startPoints.Add(bezierPoints[3]);

            endPoints.Add(startSecond);
            endPoints.Add(endSecond);
            endPoints.Add(bezierPoints[bezierPoints.Count - 2]);

            float dl = 0.2f;
            float dr = 0.8f;

            if (ConPointHelpers.Count > 3)
            {
                startFirst = ConnectionLineRenderer.GetPosition(0);
                endFirst = bezierPoints[1];
                dl = 0.45f;
                dr = 0.55f;

                startPoints.Clear();
                startPoints.Add(startFirst);
                startPoints.Add(endFirst);
                startPoints.Add(bezierPoints[2]);

                startSecond = bezierPoints[bezierPoints.Count - 3];
                endSecond = bezierPoints[bezierPoints.Count - 1];
                prevSecond = bezierPoints[bezierPoints.Count - 4];

                endPoints.Clear();
                endPoints.Add(startSecond);
                endPoints.Add(endSecond);
                endPoints.Add(endSecond);
            }

            var pointsForText = new List<Vector3>(startPoints);
            var startForText = startFirst;
            var endForText = endFirst;
            var prevForText = prev;

            if (Vector2.Distance(startFirst, endFirst) <= Vector2.Distance(startSecond, endSecond))
            {
                startForText = startSecond;
                endForText = endSecond;

                pointsForText.Clear();
                prevForText = prevSecond;
                pointsForText = new List<Vector3>(endPoints);
            }

            startArrow.transform.position = GetPointFromCurve(startPoints, dl, prev.y, prev.x); 
            endArrow.transform.position = GetPointFromCurve(endPoints, dr, prevSecond.y, prevSecond.x); 

            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);

            var startTPoint = GetPointFromCurve(startPoints, dl - 0.02f, prev.y, prev.x);
            var endTPoint = GetPointFromCurve(endPoints, dr - 0.03f, prevSecond.y, prevSecond.x);
            var startTempPoint = GetPointFromCurve(startPoints, dl + 0.02f, prev.y, prev.x);
            var endTempPoint = GetPointFromCurve(endPoints, dr + 0.03f, prevSecond.y, prevSecond.x);

            int k = 1;
            if (LineDirection == 0)
            {
                startArrow.SetActive(false);
                endArrow.SetActive(false);
            }
            else if (LineDirection == 1)
            {
                startArrow.SetActive(true);
                endArrow.SetActive(true);

                k = -1;
            }
            else if (LineDirection == 2) //2
            {
                startArrow.SetActive(true);
                endArrow.SetActive(true);

                k = 1;
            }

            if (startTPoint.y <= startTempPoint.y)
                startArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right * k, startTempPoint - startTPoint) * k);
            else
                startArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right * k, startTempPoint - startTPoint) * k);

            if (endTPoint.y <= endTempPoint.y)
                endArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.right, endTempPoint - endTPoint));
            else
                endArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(Vector2.right, endTempPoint - endTPoint));

            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);

            UpdateLabelPositionForChangedBezierLine(pointsForText, startForText, endForText, prevForText);
        }

        public void UpdateArrowsForChangedLine()
        {
            startArrow.transform.position = GetBezierPointBetweenTwoPoints(ConnectionLineRenderer.GetPosition(1), ConnectionLineRenderer.GetPosition(0), 0.25f);
            endArrow.transform.position = GetBezierPointBetweenTwoPoints(ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1), ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 2), 0.75f);

            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);

            int k = 1;
            if (LineDirection == 0)
            {
                startArrow.SetActive(false);
                endArrow.SetActive(false);
            }
            else if (LineDirection == 1)
            {
                startArrow.SetActive(true);
                endArrow.SetActive(true);

                k = -1;
            }
            else if (LineDirection == 2) //2
            {
                startArrow.SetActive(true);
                endArrow.SetActive(true);

                k = 1;
            }

            if (ConnectionLineRenderer.GetPosition(0).y <= ConnectionLineRenderer.GetPosition(1).y)
                startArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(-Vector2.right * k, ConnectionLineRenderer.GetPosition(1) - ConnectionLineRenderer.GetPosition(0)) * k);
            else
                startArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(-Vector2.right * k, ConnectionLineRenderer.GetPosition(1) - ConnectionLineRenderer.GetPosition(0)) * k);

            if (ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 2).y <= ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1).y)
                endArrow.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.right, ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1) - ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 2)));
            else
                endArrow.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(Vector2.right, ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 1) - ConnectionLineRenderer.GetPosition(ConnectionLineRenderer.positionCount - 2)));

            int distance = 60 * Mathf.RoundToInt(Vector2.Distance(Start.transform.position, End.transform.position));

            startArrow.transform.localPosition = new Vector3(startArrow.transform.localPosition.x, startArrow.transform.localPosition.y, -0.11f);
            endArrow.transform.localPosition = new Vector3(endArrow.transform.localPosition.x, endArrow.transform.localPosition.y, -0.11f);

            UpdateLabelPositionForChangedLine();
        }

        public void UpdatePositions()
        {
            DefineConnectionSide();
            ConnectionPoints.Clear();
            int LineResolution = 0;

            // ConnectionLineRenderer.startWidth = 0.035f * GraphMechanism.instance.DefaultConnectionSize;
            // ConnectionLineRenderer.endWidth = 0.035f * GraphMechanism.instance.DefaultConnectionSize;

            startArrow.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f) * GraphMechanism.instance.DefaultConnectionSize;
            endArrow.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f) * GraphMechanism.instance.DefaultConnectionSize;

            switch (ConMode)
            {
                case ConnectionMode.Bezier:
                    float _k = 2;

                    int distance = 30 * Mathf.RoundToInt(Vector2.Distance(Start.transform.position, End.transform.position));

                    if (!PointsWereChanged)
                    {
                        if (distance < cameraSize && (distance / 40) <= 36)
                        {
                            if (distance < (cameraSize / 4))
                            {
                                if (distance < (cameraSize / 8))
                                {
                                    if (distance < (cameraSize / 16)) 
                                    {
                                        LineResolution = 2;
                                        StartSide.weight = 0;
                                        EndSide.weight = 0;
                                    }
                                    else 
                                    {
                                        LineResolution = 12;
                                        StartSide.weight = 0.6f * _k;
                                        EndSide.weight = 0.6f * _k;
                                    }
                                }
                                else 
                                {
                                    LineResolution = 12 + distance / 50;
                                    StartSide.weight = 1f * _k;
                                    EndSide.weight = 1f * _k;
                                }
                            }
                            if ((distance >= (cameraSize / 4)) && (distance < (cameraSize / 2))) 
                            {
                                LineResolution = 12 + distance / 40;
                                StartSide.weight = 1.4f * _k;
                                EndSide.weight = 1.4f * _k;
                            }
                            if ((distance >= (cameraSize / 2)) && (distance < (cameraSize / 2 + cameraSize / 4))) 
                            {
                                LineResolution = 16 + distance / 40;
                                StartSide.weight = 2f * _k;
                                EndSide.weight = 2f * _k;
                            }
                            if ((distance >= (cameraSize / 2 + cameraSize / 4)) && (distance < cameraSize)) 
                            {
                                LineResolution = 20 + distance / 40;
                                StartSide.weight = 2.8f * _k;
                                EndSide.weight = 2.8f * _k;
                            }
                        }
                        else 
                        {
                            LineResolution = 40;
                            StartSide.weight = 3f * _k;
                            EndSide.weight = 3f * _k;
                        }
                    }

                    StartSide.CalculateVectors(Start.transform);
                    EndSide.CalculateVectors(End.transform);

                    if (!PointsWereChanged)
                    {
                        for (int i = 0; i < LineResolution; i++)
                        {
                            ConnectionPoints.Add(GetBezierPoint((float)i / (float)(LineResolution - 1)));
                        }

                        ConPointHelpers[0].transform.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConPointHelpers[0].transform.position.z);

                        UpdateArrowPosition();
                        UpdateLinkWidthForGraph();
                    }

                    break;
                case ConnectionMode.Straight:
                    StartSide.weight = 0;
                    EndSide.weight = 0;

                    if (!PointsWereChanged)
                        LineResolution = 3;
                    else
                        LineResolution = straightPoints.Count;

                    StartSide.Side = ConnectionSide.GraphCenter;
                    EndSide.Side = ConnectionSide.GraphCenter;
                    StartSide.CalculateVectors(Start.transform);
                    EndSide.CalculateVectors(End.transform);

                    if (!PointsWereChanged)
                    {
                        ConnectionPoints.Add(GetBezierPoint(0));
                        ConnectionPoints.Add(GetBezierPoint(0.5f));
                        ConnectionPoints.Add(GetBezierPoint(1));

                        ConPointHelpers[0].controlPoint = GetBezierPoint(0.5f);
                        ConPointHelpers[0].transform.position = new Vector3(GetBezierPoint(0.5f).x, GetBezierPoint(0.5f).y, ConPointHelpers[0].transform.position.z);

                        UpdateArrowPosition();
                    }
                    else
                    {
                        ConnectionPoints.Add(GetBezierPoint(0));

                        var temp = ConPointHelpers.OrderBy(ConnectionPointHelper => ConnectionPointHelper.positionNum).ToList();
                        ConPointHelpers.Clear();
                        ConPointHelpers = new List<ConnectionPointHelper>(temp);
                        for (int p = 0; p < ConPointHelpers.Count; p++)
                        {
                            if (ConPointHelpers[p].wasMoved)
                            {
                                ConnectionPoints.Add(straightPoints[p + 1]);
                            }
                            else
                            {
                                int p_ = p - 1;
                                int b_ = p + 1;
                                var startPos = straightPoints[p];
                                var endPos = straightPoints[p + 2];
                                var ind = 0.5f;
                                int keffStart = 0, keffEnd = 0;
                                while (p_ >= 0)
                                {
                                    if (ConPointHelpers[p_].wasMoved)
                                    {
                                        keffStart = ConPointHelpers[p_].positionNum;
                                        if (GraphMechanism.instance.isConnectionPointDragging)
                                            startPos = ConnectionLineRenderer.GetPosition(keffStart);
                                        else
                                            startPos = straightPoints[ConPointHelpers[p].positionNum - 1];
                                        break;
                                    }
                                    p_ -= 1;
                                }

                                while (b_ < straightPoints.Count - 2)
                                {
                                    if (ConPointHelpers[b_].wasMoved)
                                    {
                                        keffEnd = ConPointHelpers[b_].positionNum;
                                        if (GraphMechanism.instance.isConnectionPointDragging)
                                            endPos = ConnectionLineRenderer.GetPosition(keffEnd);
                                        else
                                            endPos = straightPoints[ConPointHelpers[p].positionNum + 1];
                                        break;
                                    }
                                    b_ += 1;
                                }

                                if (p_ < 0)
                                {
                                    startPos = GetBezierPoint(0);
                                    keffStart = 0;
                                }
                                if (b_ >= straightPoints.Count - 2)
                                {
                                    keffEnd = straightPoints.Count - 1;
                                    endPos = GetBezierPoint(1);
                                }
                                ind = (1f / (keffEnd - keffStart)) * (ConPointHelpers[p].positionNum - keffStart);

                                var pos = GetBezierPointBetweenTwoPoints(startPos, endPos, ind);

                                ConnectionPoints.Add(pos);
                                straightPoints[p + 1] = pos;
                                ConPointHelpers[p].controlPoint = pos;
                                ConPointHelpers[p].transform.position = new Vector3(pos.x, pos.y, ConPointHelpers[p].transform.position.z);
                            }
                        }
                        ConnectionPoints.Add(GetBezierPoint(1));
                        straightPoints.Clear();
                        straightPoints = new List<Vector3>(ConnectionPoints);

                        foreach (var h in ConPoints)
                        {
                            h.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // * Camera.main.orthographicSize / InitialOrtographicSize;
                        }
                    }
                    UpdateLinkWidthForGraph();

                    break;
                case ConnectionMode.Square:
                    LineResolution = 4;
                    ConnectionPoints.Add(Start.transform.position);
                    ConnectionPoints.Add(new Vector2(Start.transform.position.x, (End.transform.position.y + Start.transform.position.y) / 2));
                    ConnectionPoints.Add(new Vector2(End.transform.position.x, (End.transform.position.y + Start.transform.position.y) / 2));
                    ConnectionPoints.Add(End.transform.position);
                    break;
            }

            BuildLines(ConnectionPoints);

            if (PointsWereChanged && ConMode == ConnectionMode.Straight)
            {
                UpdateArrowsForChangedLine();
            }
            if (PointsWereChanged && ConMode == ConnectionMode.Bezier)
            {
                var start = Start.transform.position;
                start.z = 0;
                var end = End.transform.position;
                end.z = 0;
                bezierPoints[0] = start;
                bezierPoints[bezierPoints.Count - 2] = end;
                bezierPoints[bezierPoints.Count - 1] = end;
                
                DrawByCurves(bezierPoints);
                UpdateEdgeCollider();

                //-----------------
                for (int i = 0; i < ConPointHelpers.Count; i++)
                {
                    if (!ConPointHelpers[i].wasMoved)
                    {
                        var newRhPoints = new List<Vector3>();
                        int c_ = 0;
                        int l = i - 1;
                        if (l < 0)
                            l = 0;

                        var firstPoint = ConPointHelpers[l].controlPoint;
                        if (ConPointHelpers[i].positionNum == 1)
                        {
                            firstPoint = bezierPoints[0];
                        }
                        for (int i_ = 0; i_ < bezierPoints.Count; i_++)
                        {
                            if (bezierPoints[i_].x == firstPoint.x && bezierPoints[i_].y == firstPoint.y)
                                c_ = i_;
                        }

                        int c1 = c_ - 1;
                        if (c1 < 0)
                            c1 = 0;
                        int c2 = c_ - 2;
                        if (c2 < 0)
                            c2 = 0;

                        var prevRh = bezierPoints[c1];

                        newRhPoints.Add(bezierPoints[c_]);
                        newRhPoints.Add(bezierPoints[c_ + 1]);
                        newRhPoints.Add(bezierPoints[c_ + 2]);

                        var newPoint = GetPointFromCurve(newRhPoints, 0.5f, prevRh.y, prevRh.x);

                        ConPointHelpers[i].controlPoint = newPoint;
                        var pos = ConPointHelpers[i].transform.position;
                        ConPointHelpers[i].transform.position = new Vector3(ConPointHelpers[i].controlPoint.x, ConPointHelpers[i].controlPoint.y, pos.z);
                    }
                }
                //-----------------

                UpdateArrowsForChangedBezierLine();
            }
        }

        public Vector3 GetBezierPoint(float t, int derivative = 0)
        {
            derivative = Mathf.Clamp(derivative, 0, 2);
            float u = (1f - t);
            Vector3 p1 = StartSide.p, p2 = EndSide.p, c1 = StartSide.c, c2 = EndSide.c;

            if (derivative == 0)
            {
                return u * u * u * p1 + 3f * u * u * t * c1 + 3f * u * t * t * c2 + t * t * t * p2;
            }
            else if (derivative == 1)
            {
                return 3f * u * u * (c1 - p1) + 6f * u * t * (c2 - c1) + 3f * t * t * (p2 - c2);
            }
            else if (derivative == 2)
            {
                return 6f * u * (c2 - 2f * c1 + p1) + 6f * t * (p2 - 2f * c2 + c1);
            }
            else
            {
                return Vector3.zero;
            }
        }

        public Vector3 GetBezierPointBetweenTwoPoints(Vector3 start, Vector3 end, float t, int derivative = 0)
        {
            derivative = Mathf.Clamp(derivative, 0, 2);
            float u = (1f - t);
            Vector3 p1 = start, p2 = end, c1 = start, c2 = end;

            if (derivative == 0)
            {
                return u * u * u * p1 + 3f * u * u * t * c1 + 3f * u * t * t * c2 + t * t * t * p2;
            }
            else if (derivative == 1)
            {
                return 3f * u * u * (c1 - p1) + 6f * u * t * (c2 - c1) + 3f * t * t * (p2 - c2);
            }
            else if (derivative == 2)
            {
                return 6f * u * (c2 - 2f * c1 + p1) + 6f * t * (p2 - 2f * c2 + c1);
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void DrawByCurves(List<Vector3> points)
        {
            List<Vector3> newPoints = new List<Vector3>();
            float dl = 0, dlx = 0;
            newPoints.Add(points[0]);
            for (var i = 0; i < points.Count - 2; i++)
            {
                var dr = (points[i + 2].y - points[i].y) / 2;
                var a3 = dl + dr + 2 * (points[i].y - points[i + 1].y);
                var a2 = points[i + 1].y - a3 - dl - points[i].y;

                var drx = (points[i + 2].x - points[i].x) / 2;
                var a3x = dlx + drx + 2 * (points[i].x - points[i + 1].x);
                var a2x = points[i + 1].x - a3x - dlx - points[i].x;

                int lCount = 20;
                for (float j = 0; j < lCount; j++)
                {
                    float t = (float)j / ((float)lCount - 1);
                    var y = a3; y = y * t + a2; y = y * t + dl; y = y * t + points[i].y;
                    var x = a3x; x = x * t + a2x; x = x * t + dlx; x = x * t + points[i].x;
                    newPoints.Add(new Vector3(x, y, 0));
                }
                dl = dr;
                dlx = drx;
            }
            bezierPoints = new List<Vector3>(points);

            ConnectionLineRenderer.positionCount = newPoints.Count;
            ConnectionLineRenderer.SetPositions(newPoints.ToArray());
            ConnectionLineRenderer.Simplify(0.01f);
        }

        public Vector3 GetPointFromCurve(List<Vector3> points, float d, float prevY, float prevX)
        {
            float dl = (points[1].y - prevY) / 2;//0;
            float dlx = (points[1].x - prevX) / 2;//0;

            if (prevY == points[0].y)
                dl = 0;
            if (prevX == points[0].x)
                dlx = 0;

            var dr = (points[2].y - points[0].y) / 2;
            var a3 = dl + dr + 2 * (points[0].y - points[1].y);
            var a2 = points[1].y - a3 - dl - points[0].y;

            var drx = (points[2].x - points[0].x) / 2;
            var a3x = dlx + drx + 2 * (points[0].x - points[1].x);
            var a2x = points[1].x - a3x - dlx - points[0].x;

            var fx = a3x; fx = fx * d + a2x; fx = fx * d + dlx; fx = fx * d + points[0].x;
            var fy = a3; fy = fy * d + a2; fy = fy * d + dl; fy = fy * d + points[0].y;

            return (new Vector3(fx, fy, 0));
        }

        void BuildLines(List<Vector3> points)
        {
            ConnectionLineRenderer.positionCount = points.Count;
            ConnectionLineRenderer.SetPositions(points.ToArray());
        }

        public void UpdateEdgeCollider()
        {

            var lineStartPositions = ConnectionLineRenderer;

            var points = new Vector2[lineStartPositions.positionCount];

            EdgeCollider.edgeRadius = 0.15f;
            EdgeCollider.offset = new Vector2(0, 0);
            float _x = -ConnectionGObj.transform.position.x;
            float _y = -ConnectionGObj.transform.position.y;
            EdgeCollider.offset = new Vector2(_x, _y);

            Rigidbody.bodyType = RigidbodyType2D.Static; //static || kinematic
            Rigidbody.simulated = true;

            for (int i = 0; i < lineStartPositions.positionCount; i++)
            {
                points[i] = lineStartPositions.GetPosition(i);
            }

            EdgeCollider.points = points;

            //if (CurrentConnection.ConMode == ConnectionMode.Bezier)
            //CurrentConnection.SaveBezierPoints();
            //if (CurrentConnection.ConMode == ConnectionMode.Straight)
            //CurrentConnection.SaveStraightPoints();
        }
    
        public void SelectLine(bool enable)
        {
            if (enable)
            {
                //LineAnimator.enabled = true;
                //ConnectionLineRenderer.widthMultiplier = 0.5f;
                //LineAnimator.SetTrigger("on");

                //Color highlightColor = new Color(1f, 0.6f, 0);
                // Use DOTween to smoothly change from current color to highlight color
                // DOTween.To(() => connectionLabel.color, x => connectionLabel.color = x, highlightColor, 0.3f);
                // DOTween.To(() => ConnectionLineRenderer.startColor, x => ConnectionLineRenderer.startColor = x, highlightColor, 0.3f);
                // DOTween.To(() => ConnectionLineRenderer.endColor, x => ConnectionLineRenderer.endColor = x, highlightColor, 0.3f);
                
                connectionLabel.fontSize = 20;
            }
            else
            {
                //LineAnimator.SetTrigger("off");
                //ConnectionLineRenderer.widthMultiplier = 0.5f;

                // Use DOTween to smoothly change back to the original color
                // DOTween.To(() => connectionLabel.color, x => connectionLabel.color = x, CurrentLineColor, 0.3f);
                // DOTween.To(() => ConnectionLineRenderer.startColor, x => ConnectionLineRenderer.startColor = x, CurrentLineColor, 0.3f);
                // DOTween.To(() => ConnectionLineRenderer.endColor, x => ConnectionLineRenderer.endColor = x, CurrentLineColor, 0.3f);

                // connectionLabel.fontSize = 18;
                // LineAnimator.enabled = false;
            }
        }

        public void ResizeConnection(float value)
        {
            UpdateLinkWidthForGraph();
        }

        public void RemoveInstanse()
        {
            if (ChangeSelectedLineTypeHelper.SelectedLine == this)
            {
                //GraphMechanism.instance.ChangeModeButton.SetActive(false);
                ChangeSelectedLineTypeHelper.SelectedLine = null;
            }

            Start.connections.Remove(this);
            End.connections.Remove(this);
            foreach (var p in ConPointHelpers)
            {
                GameObject.Destroy(p.gameObject);
            }
            GraphMechanism.instance.Connections.Remove(this);
            GameObject.DestroyImmediate(ConnectionGObj);

            
        }

        public void ChangeModeForInstance()
        {
            switch (ConMode)
            {
                case ConnectionMode.Straight:
                    ConMode = ConnectionMode.Bezier;
                    bezierPoints.Clear();
                    var st = Start.transform.position;
                    st.z = 0;
                    var en = End.transform.position;
                    en.z = 0;
                    bezierPoints.Add(st);
                    if (ConPointHelpers.Count > 0)
                    {
                        foreach (var con in ConPointHelpers.GetRange(0, ConPointHelpers.Count))
                        {
                            if (con.wasMoved)
                                bezierPoints.Add(con.controlPoint);
                        }
                    }
                    bezierPoints.Add(en);
                    bezierPoints.Add(en);

                    break;
                case ConnectionMode.Bezier:
                    ConMode = ConnectionMode.Straight;
                    straightPoints.Clear();
                    var st2 = Start.transform.position;
                    st.z = 0;
                    var en2 = End.transform.position;
                    en.z = 0;
                    straightPoints.Add(st2);
                    if (ConPointHelpers.Count > 0)
                    {
                        foreach (var con in ConPointHelpers.GetRange(0, ConPointHelpers.Count))
                        {
                            straightPoints.Add(con.controlPoint);
                        }
                    }
                    straightPoints.Add(en2);

                    break;
            }
            UpdatePositions();
            UpdateEdgeCollider();
        }
    }

    public class ConnectionPoint
    {
        public Color color = Color.white;
        public ConnectionSide Side = ConnectionSide.Up;
        [Range(-1f, 1f)] public float position = 0f;
        public float weight = 0.5f;

        public Vector3 p { get; private set; }
        public Vector3 c { get; private set; }

        public void Reset()
        {
            color = Color.white;
            Side = ConnectionSide.Up;
            position = 0f;
            weight = 0.5f;
        }

        public void CalculateVectors(Transform transform)
        {
            if (!transform) return;

            switch (Side)
            {
                case ConnectionSide.GraphCenter:
                    p = transform.position;
                    p = new Vector3(p.x, p.y, 0);
                    c = p;
                    break;

                case ConnectionSide.Center:
                    p = transform.position;
                    p = new Vector3(p.x, p.y, 99.8f);
                    c = p;
                    break;

                case ConnectionSide.Up:
                    if (transform.GetComponent<BoxCollider2D>())
                    {
                        var _size = transform.GetComponent<BoxCollider2D>().size;
                        p = transform.TransformPoint(
                            _size.x / 2f * position,
                            _size.y / 2f,
                            0);
                    }
                    c = p + transform.up * weight;
                    break;

                case ConnectionSide.Down:
                    if (transform.GetComponent<BoxCollider2D>())
                    {
                        var _size = transform.GetComponent<BoxCollider2D>().size;
                        p = transform.TransformPoint(

                        _size.x / 2f * position,
                        -_size.y / 2f,
                        0);
                    }
                    c = p - transform.up * weight;
                    break;

                case ConnectionSide.Left:
                    if (transform.GetComponent<BoxCollider2D>())
                    {
                        var _size = transform.GetComponent<BoxCollider2D>().size;
                        p = transform.TransformPoint(
                        _size.x / 2f,
                        _size.y / 2f * position,
                        0);
                    }
                    c = p + transform.right * weight;
                    break;

                case ConnectionSide.Right:
                    if (transform.GetComponent<BoxCollider2D>())
                    {
                        var _size = transform.GetComponent<BoxCollider2D>().size;
                        p = transform.TransformPoint(
                        -_size.x / 2f,
                        _size.y / 2f * position,
                        0);
                    }
                    c = p - transform.right * weight;
                    break;

                default:
                    float angle = Mathf.PI / 2f - position * Mathf.PI;
                    var size = transform.GetComponent<SpriteRenderer>().size;
                    p = transform.TransformPoint(
                        size.x / 2f * Mathf.Cos(angle),
                        size.y / 2f * Mathf.Sin(angle),
                        0);
                    c = p + transform.TransformDirection(Mathf.Cos(angle), Mathf.Sin(angle), 0) * weight;
                    break;
            }
        }
    }


    public enum ConnectionSide
    {
        Up,
        Right,
        Down,
        Left,
        Center,
        GraphCenter
    }
    public enum ConnectionMode
    {
        Bezier,
        Straight,
        Square
    }

}

