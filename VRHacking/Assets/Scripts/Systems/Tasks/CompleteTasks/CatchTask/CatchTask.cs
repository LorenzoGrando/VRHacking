using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CatchTask : HackTask
{
    #region Generic Task Methods
    public override void StartTask(GameSettingsData settingsData)
    {
        ResetTask();
        this.gameSettingsData = settingsData;

        if(catchablePool == null) {
            catchablePool = new ObjectPool<CatchTaskCatchable>
             (CreateCatchable, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        }

        GenerateTaskTargets();
        display.OnScalingAnimComplete += AwaitStartAnim;
        display.InitiateDisplay(settingsData);

        slider.OnCollectCatchable += OnCollectCatchable;

        
    }

    protected override bool CheckTaskCompleted()
    {
        if(numberOfCollectedObjects >= objectsToCatch){
            return true;
        }

        return false;
    }

    protected override void ResetTask()
    {
        numberOfCollectedObjects = 0;
        display.ResetDisplay();
    }

    public override void HideTask()
    {
        display.HideDisplay(false);
    }

    protected override void CompleteTask()
    {
        display.HideDisplay(false);
        base.CompleteTask();
    }

    #endregion

    #region Catch Methods
    [SerializeField]
    private CatchTaskDisplay display;
    [SerializeField]
    private AudioSource catchAudio;
    [SerializeField]
    CatchTaskSlider slider;
    [SerializeField]
    private Transform[] spawnPositions;
    [SerializeField]
    private Transform catachableBottomAnchor, catchableTopAnchor;
    [SerializeField]
    private Transform catchableHolder;
    private Coroutine gameLoopRoutine;
    
    [SerializeField]
    private int baseAmountOfObjects;
    private int objectsToCatch;
    private int numberOfCollectedObjects;
    private int currentSpawnerIndex;
    private float thisTaskMoveSpeed;

    [SerializeField]
    private float baseSpawnObjectInterval;
    [SerializeField]
    private float baseObjectMoveSpeed;

    
    private void AwaitStartAnim() {
        gameLoopRoutine = StartCoroutine(routine: GameLoopRoutine());
        display.OnScalingAnimComplete -= AwaitStartAnim;
    }

    private void AwaitEndAnim() {
        display.OnScalingAnimComplete -= AwaitEndAnim;
        CompleteTask();
    }

    private void GenerateTaskTargets() {
        objectsToCatch = baseAmountOfObjects + Mathf.CeilToInt(baseAmountOfObjects/3 * gameSettingsData.difficulty);
        thisTaskMoveSpeed = baseObjectMoveSpeed + (thisTaskMoveSpeed/3 * gameSettingsData.difficulty);
        //Set initial spawn pos to a random object
        currentSpawnerIndex = UnityEngine.Random.Range(0, spawnPositions.Length);
    }

    private int GetNextSpawnerIndex() {
        //generate a value bewteen -1 and 1, but higher changes for the extremes instead of 0
        int nextDirectionOfSpawn = Mathf.Clamp(UnityEngine.Random.Range(-3, 4), -1, 1);

        return Mathf.Clamp(currentSpawnerIndex + nextDirectionOfSpawn, 0, spawnPositions.Length - 1);
    }

    private void PlaceCatchableAtSpawn(CatchTaskCatchable catchable) {
        catchable.transform.position = spawnPositions[currentSpawnerIndex].transform.position;
        catchable.SetSpeed(thisTaskMoveSpeed);
        catchable.gameObject.SetActive(true);
    }

    private void OnCollectCatchable() {
        numberOfCollectedObjects++;
        catchAudio.Play();
        
        if(CheckTaskCompleted()) {
            if(gameLoopRoutine != null) {
            StopCoroutine(gameLoopRoutine);
            }
            catchablePool.Clear();

            slider.OnCollectCatchable -= OnCollectCatchable;
            display.OnScalingAnimComplete += AwaitEndAnim;
            display.HideDisplay(true);
        }
    }

    private IEnumerator GameLoopRoutine() {
        while (!CheckTaskCompleted()) {
            PlaceCatchableAtSpawn(catchablePool.Get());
            currentSpawnerIndex = GetNextSpawnerIndex();

            yield return new WaitForSeconds(baseSpawnObjectInterval);
        }

        yield break;
    }



    #endregion

    #region Pool Methods

    // Pool Paramters
    [Header("Pool Parameters")]
    [SerializeField]
    private GameObject catchablePrefab;
    private IObjectPool<CatchTaskCatchable> catchablePool;
    // throw an exception if we try to return an existing item, already in the pool
    [SerializeField] private bool collectionCheck = true;

    // extra options to control the pool capacity and maximum size
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize = 10;

    private CatchTaskCatchable CreateCatchable() {
        CatchTaskCatchable catchable = Instantiate(catchablePrefab).GetComponent<CatchTaskCatchable>();
        catchable.UpdatePool(catchablePool);
        catchable.transform.SetParent(catchableHolder);
        catchable.transform.localScale = catchable.targetLocalScale;
        catchable.transform.localRotation = Quaternion.Euler(Vector3.zero);
        catchable.GetComponent<UIRestrainAnchors>().UpdateAnchors(catachableBottomAnchor, catchableTopAnchor);
        return catchable;
    }

    private void OnReleaseToPool(CatchTaskCatchable catchable) {
        catchable.gameObject.SetActive(false);
    }

    private void OnGetFromPool(CatchTaskCatchable catchable) {
        catchable.gameObject.SetActive(true);
    }

    private void OnDestroyPooledObject(CatchTaskCatchable catchable) {
        Destroy(catchable);
    }



    #endregion
}
