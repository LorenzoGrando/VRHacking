using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FallingObjectsTask : HackTask
{
    #region Generic Task Methods
    public override void HideTask()
    {
        display.HideDisplay(false);
    }

    public override void StartTask(GameSettingsData settingsData)
    {
        ResetTask();
        this.gameSettingsData = settingsData;

        if(fallingObjectPool == null)
            InitiatePool();

        canSpawn = true;

        collectedObjects = 0;
        GenerateTaskTargets();

        display.OnScalingAnimComplete += AwaitStartAnim;
        display.InitiateDisplay();
    }

    protected override bool CheckTaskCompleted()
    {
        return collectedObjects >= objectsToCatch;
    }

    protected override void ResetTask()
    {
        if(gameLoopRoutine != null) {
            StopCoroutine(gameLoopRoutine);
            gameLoopRoutine = null;
        }

        collectedObjects = 0;
        
        DestroyPool();
    }

    protected override void CompleteTask()
    {
        if(gameLoopRoutine != null) {
            StopCoroutine(gameLoopRoutine);
            gameLoopRoutine = null;
        }
        display.OnScalingAnimComplete -= AwaitEndAnim;
        display.HideDisplay(false);
        base.CompleteTask();
    }

    protected override void CallGlitch()
    {
        canSpawn = false;
        glitchManager.OnGlitchFinished += () => canSpawn = true;
        base.CallGlitch();
    }

    #endregion

    [Header("Task Parameters")]
    [SerializeField] private FallingObjectsDisplay display;
    [SerializeField] private GameObject fallingObjectHolder;
    [SerializeField] private Transform fallingObjectSpawnPoint, fallingObjectBottomAnchor, fallingObjectTopAnchor;
    [SerializeField] private AudioSource correctSource, errorSource;
    [SerializeField] private int baseTargetObjects;
    [SerializeField] private float baseObjectMoveSpeed;
    [SerializeField] private float baseSpawnInterval;
    private int objectsToCatch;
    private int collectedObjects;
    private float objectMoveSpeed;
    private float spawnInterval;
    private bool canSpawn;
    public enum FallingObjectType {
        Data, Bug
    }


    private Coroutine gameLoopRoutine;

    private void AwaitStartAnim() {
        gameLoopRoutine = StartCoroutine(routine: ExecuteTaskLoop());
        display.OnScalingAnimComplete -= AwaitStartAnim;
    }

    private void AwaitEndAnim() {
        display.HideDisplay(true);
        display.OnScalingAnimComplete += CompleteTask;
    }

    private void GenerateTaskTargets() {
        objectsToCatch = baseTargetObjects + Mathf.CeilToInt(baseTargetObjects/3 * gameSettingsData.difficulty);
        objectMoveSpeed = baseObjectMoveSpeed + (baseObjectMoveSpeed/4 * gameSettingsData.difficulty);
        spawnInterval = baseSpawnInterval + (baseSpawnInterval/4 * gameSettingsData.difficulty);

        collectedObjects = 0;
    }

    private void PlaceObjectAtSpawn(FallingObjectCatchable fallingObject) {
        fallingObject.transform.position = fallingObjectSpawnPoint.position;
        fallingObject.SetSpeed(objectMoveSpeed);
        fallingObject.gameObject.SetActive(true);
    }

    public void TryCollectObject(FallingObjectCatchable fallingObject, FallingObjectCollector collector) {
        if(fallingObject.thisObjectType == collector.CollectionType) {
            collectedObjects++;
            correctSource.PlayOneShot(correctSource.clip);
        }
        else {
            errorSource.PlayOneShot(errorSource.clip);
        }

        collector.DestroyCollidedObject();
    }

    private void OnHoldObject(FallingObjectCatchable fallingObject) {
        fallingObject.OnHoldObject -= OnHoldObject;

        if(fallingObject.mined) {
            fallingObject.OnExistanceFutile();
            CallGlitch();
        }
    }
    private IEnumerator ExecuteTaskLoop() {
        while(!CheckTaskCompleted()) {
            if(canSpawn) {
                FallingObjectCatchable fallingObject = fallingObjectPool.Get();
                fallingObject.thisObjectType = (FallingObjectType) Random.Range(0,2);

                bool bugStatus = false;
                if(enableMines) {
                    int rnd = UnityEngine.Random.Range(0, 3);
                    if(rnd == 0) {
                        bugStatus = true;
                    }
                }

                fallingObject.UpdateAppearance(bugStatus);
                PlaceObjectAtSpawn(fallingObject);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        AwaitEndAnim();

        yield break;
    }

    #region Pool Methods
    [Header("Pool parameters")]
    [SerializeField] private GameObject fallingObjectPrefab;
    private IObjectPool<FallingObjectCatchable> fallingObjectPool;
    // throw an exception if we try to return an existing item, already in the pool
    [SerializeField] private bool collectionCheck = true;

    // extra options to control the pool capacity and maximum size
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize = 10;

    private void InitiatePool() {
        if(fallingObjectPool == null) {
            fallingObjectPool = new ObjectPool<FallingObjectCatchable>
             (CreateFallingObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        }
    }

    private void DestroyPool() {
        if(fallingObjectPool != null)
            fallingObjectPool.Clear();
    }
    private FallingObjectCatchable CreateFallingObject() {
        FallingObjectCatchable fallingObject = Instantiate(fallingObjectPrefab).GetComponent<FallingObjectCatchable>();
        fallingObject.UpdatePool(fallingObjectPool);
        fallingObject.transform.SetParent(fallingObjectHolder.transform);
        fallingObject.ResetTransformToDefault();
        fallingObject.GetComponent<UIRestrainAnchors>().UpdateAnchors(fallingObjectBottomAnchor, fallingObjectTopAnchor);

        fallingObject.OnHoldObject += OnHoldObject;
        return fallingObject;
    }

    private void OnReleaseToPool(FallingObjectCatchable fallingObject) {
        fallingObject.gameObject.SetActive(false);
    }

    private void OnGetFromPool(FallingObjectCatchable fallingObject) {
        fallingObject.gameObject.SetActive(true);
    }

    private void OnDestroyPooledObject(FallingObjectCatchable fallingObject) {
        Destroy(fallingObject);
    }

    #endregion
}