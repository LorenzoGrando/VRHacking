using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class CatchTaskCatchable : MonoBehaviour
{
    public Vector3 targetLocalScale;
    private IObjectPool<CatchTaskCatchable> poolReference;
    private Rigidbody rb;
    [SerializeField]
    private UIRestrainer restrainer;
    public bool bugged;
    private float moveSpeed;
    public Material mainMat, glitchedMat;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite[] sprites;
    private AudioSource glitchSource;


    public void OnEnable()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();
        if(glitchSource == null) {
            glitchSource = GetComponent<AudioSource>();
        }
        
        UpdateStatus(bugged);
    }
    public void UpdatePool (IObjectPool<CatchTaskCatchable> pool) {
        poolReference = pool;
    }

    public void SetSpeed(float speed) {
        moveSpeed = -speed;
        rb.velocity = new Vector3(0, moveSpeed, 0);
    }

    public void OnExistanceFutile() {
        poolReference.Release(this);
    }

    public void UpdateStatus(bool bugged) {
        this.bugged = bugged;
        if(bugged) {
            image.material = glitchedMat;
            image.sprite = sprites[1];
            image.color = Color.red;
            glitchSource.PlayOneShot(glitchSource.clip);
        }
        else {
            image.material = mainMat;
            image.sprite = sprites[0];
            image.color = Color.white;
        }
    }

    void LateUpdate()
    {
        if(restrainer.TryRestrain(true)) {
            OnExistanceFutile();
        }
    }

}