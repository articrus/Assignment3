using benjohnson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : Singleton<ArtifactManager>
{
    //List<Artifact> artifacts;
    [SerializeField] LinkedList<Artifact> artifactList;

    [SerializeField] List<A_Base> startingArtifacts = new List<A_Base>();
    public List<A_Base> undiscoveredArtifacts;

    [HideInInspector] public List<Artifact> queue;
    float timeSinceTrigger;

    [Header("Components")]
    [SerializeField] GameObject visualizerGO;

    /// <summary>
    /// Returns a random set of artifacts from undiscoveredArtifacts list
    /// </summary>
    public List<A_Base> GetRandomArtifacts(int maxItems)
    {
        List<A_Base> shuffledItems = new List<A_Base>(undiscoveredArtifacts);

        // Shuffle
        for (int i = shuffledItems.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            A_Base temp = shuffledItems[i];
            shuffledItems[i] = shuffledItems[randomIndex];
            shuffledItems[randomIndex] = temp;
        }

        // Select up to maxItems
        List<A_Base> selectedItems = shuffledItems.GetRange(0, Mathf.Min(maxItems, shuffledItems.Count));

        return selectedItems;
    }

    protected override void Awake()
    {
        base.Awake();

        //artifacts = new List<Artifact>();
        artifactList = new LinkedList<Artifact>();
        queue = new List<Artifact>();
    }

    private void Start()
    {
        foreach (A_Base a in startingArtifacts)
            AddArtifact(a);
    }

    private void Update()
    {
        if (queue.Count <= 0) return;

        if (timeSinceTrigger >= 0.05f)
        {
            queue[0].Trigger();
            queue.RemoveAt(0);
            timeSinceTrigger = 0;
        }
        else
            timeSinceTrigger += Time.deltaTime;
    }

    public void AddArtifact(A_Base artifact)
    {
        //Old: Artifact _a = new Artifact(artifact, Instantiate(visualizerGO, transform.position + new Vector3(artifacts.Count * 2.25f, 0), Quaternion.identity, transform).GetComponent<ArtifactVisualizer>());
        Artifact _a = new Artifact(artifact, Instantiate(visualizerGO, transform.position + new Vector3(artifactList.Count * 2.25f, 0), Quaternion.identity, transform).GetComponent<ArtifactVisualizer>());
        artifactList.AddLast(_a);
        undiscoveredArtifacts.Remove(artifact);
        PlayerStats.instance.artifactsDiscovered++;

        // Trigger pickup
        artifact.OnPickup();
        _a.TryTrigger();
    }

    public class Artifact
    {
        public A_Base artifact;
        public ArtifactVisualizer visualizer;

        public Artifact(A_Base artifact, ArtifactVisualizer visualizer)
        {
            this.artifact = artifact;
            this.visualizer = visualizer;
            visualizer.Visualize(artifact);
        }

        public void TryTrigger()
        {
            if (artifact.triggered)
            {
                artifact.triggered = false;
                instance.queue.Add(this);
            }
        }

        public void Trigger()
        {
            artifact.Trigger();
            visualizer.Trigger();
            PlayerStats.instance.artifactsTriggered++;
        }
    }

    #region Triggers

    public void TriggerStartOfTurn()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnStartOfTurn();
            artifact.TryTrigger();
        }

    }

    public void TriggerEndOfTurn()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnEndOfTurn();
            artifact.TryTrigger();
        }
    }

    public void TriggerStartOfEnemyTurn()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnStartOfEnemyTurn();
            artifact.TryTrigger();
        }
    }

    public void TriggerEnterRoom()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnEnterRoom();
            artifact.TryTrigger();
        }
    }

    public void TriggerEnterBossRoom()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnEnterBossRoom();
            artifact.TryTrigger();
        }
    }

    public void TriggerClearRoom()
    {
        foreach(Artifact artifact in artifactList)
        {
            artifact.artifact.OnClearRoom();
            artifact.TryTrigger();
        }
    }

    public void TriggerTakeDamage()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnTakeDamage();
            artifact.TryTrigger();
        }
    }

    public void TriggerDealDamage()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnDealDamage();
            artifact.TryTrigger();
        }
    }

    public void TriggerKillEnemy()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnKillEnemy();
            artifact.TryTrigger();
        }
    }

    public void TriggerChestOpen()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnChestOpen();
            artifact.TryTrigger();
        }
    }

    public void TriggerBossDefeated()
    {
        foreach (Artifact artifact in artifactList)
        {
            artifact.artifact.OnBossDefeated();
            artifact.TryTrigger();
        }
    }

    #endregion
}
