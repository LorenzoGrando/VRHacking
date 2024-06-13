using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using System;
using DG.Tweening;

[RequireComponent(typeof(UIRestrainer), typeof(Rigidbody), typeof(UIPhysicalDraggable))]
public class FallingObjectCatchable : MonoBehaviour
{
    public event Action<FallingObjectCatchable> OnHoldObject;
    [SerializeField] private Vector3 targetLocalScale;
    private IObjectPool<FallingObjectCatchable> poolReference;
    private UIRestrainer restrainer;
    private Rigidbody rb;
    private UIPhysicalDraggable physicalDraggable;

    public bool mined;
    public FallingObjectsTask.FallingObjectType thisObjectType;
    [SerializeField] private AudioSource glitchSource;
    [SerializeField] private Image[] images;
    [SerializeField] private Material mainMat, glitchedMat;
    [SerializeField] private GameObject[] typeSprites;

    private float moveSpeed;

    void OnEnable()
    {
        restrainer = GetComponent<UIRestrainer>();
        rb = GetComponent<Rigidbody>();
        physicalDraggable = GetComponent<UIPhysicalDraggable>();
        transform.DOScale(targetLocalScale, 0.15f);
    }

    void Update()
    {
        if(!physicalDraggable.GetInteractionStatus())
            rb.velocity = transform.up * moveSpeed;
    }

    void LateUpdate()
    {
        if(physicalDraggable.GetInteractionStatus()) {
            OnHoldObject?.Invoke(this);
        }
        else {
            restrainer.TryRestrain(true);
        }
    }

    public void SetSpeed(float speed) {
        moveSpeed = -speed;  
    }

    public void UpdatePool(IObjectPool<FallingObjectCatchable> newPool) => poolReference = newPool;

    public void UpdateAppearance(bool isMined) {
        if(isMined) {
            images[(int)thisObjectType].material = glitchedMat;
            Color32 color = new Color32(255, 0, 0, 255);
            images[(int)thisObjectType].color = color;  
            glitchSource.PlayOneShot(glitchSource.clip);
        }
        else {
            images[(int)thisObjectType].material = mainMat;
            Color32 color = new Color32(255, 255, 255, 255);
            images[(int)thisObjectType].color = color; 
        }
        foreach(GameObject spriteObject in typeSprites) {
            spriteObject.SetActive(false);
        }

        typeSprites[(int)thisObjectType].SetActive(true);
        mined = isMined;
    }

    public void ResetTransformToDefault() {
        transform.localScale = targetLocalScale;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        rb.velocity = Vector2.zero;
    }

    public void OnExistanceFutile() {
        poolReference.Release(this);
    }
}