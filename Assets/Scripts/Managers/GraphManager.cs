using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
namespace GraphSystem
{
    public class GraphManager : MonoBehaviour
    {   
        public static GraphManager instance;
        public GraphMechanism graphMechanism;
        [SerializeField] private Transform entityParent;
        private Dictionary<string, GameObject> wordEntities = new Dictionary<string, GameObject>();

        // #endregion
        void Awake()
        {
            instance = this;
        }   

        void Start()
        {
            if (graphMechanism == null)
            {
                graphMechanism = FindObjectOfType<GraphMechanism>();
            }
        }

        public void DestroyEntity()
        {
            List<GameObject> entitiesToDestroy = new List<GameObject>(graphMechanism.Entities);
            if (entitiesToDestroy.Count == 0)
                return;
            foreach(var entity in entitiesToDestroy)
            {
                graphMechanism.DestroyEntity(entity);
            }
            graphMechanism.Entities.Clear();
            wordEntities.Clear();
        }
        public void CreateEntitiesFromWordNet(uint synsetId)
        {
            var words = WordnetPanel.instance.GetWords();

            foreach (var word in words)
            {
                Vector2 wordPosition = WordnetPanel.instance.GetWordPosition(word);
                int entityType;
                var EntityPrefab = CreateGameObject.pathEntityPrefab;
                if(WordnetPanel.instance.isSameLemma(word))
                {
                    entityType = 0;
                }
                else if(CustomLemmaStorage.isCustomLemmasContainsWord(synsetId, word))
                {
                    entityType = 2;
                    EntityPrefab = CreateGameObject.pathEntityPrefabBranch2;
                }
                else
                {
                    entityType = 1;
                    EntityPrefab = CreateGameObject.pathEntityPrefabBranch1;
                }
                Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(wordPosition.x, wordPosition.y, 10));
                GameObject entity = CreateGameObject.Create2DEntity(graphMechanism.EntityParent, worldPosition, EntityPrefab);
                entity.transform.localScale = Vector3.one * 0.6f;
                graphMechanism.Entities.Add(entity);
                entity.GetComponent<EntitySceneHelper>().init(word);
                wordEntities[word] = entity;
            }
        }
        public void CreateConnectionBetweenWords(string word1, string word2)
        {
            if (wordEntities.TryGetValue(word1, out GameObject entity1) && 
                wordEntities.TryGetValue(word2, out GameObject entity2))
            {
                graphMechanism.FirstSelectedEntity = entity1.GetComponent<EntitySceneHelper>();
                graphMechanism.SecondSelectedEntity = entity2.GetComponent<EntitySceneHelper>();
                graphMechanism.CreateConnectionForWordnet();
            }
        }
    }
}