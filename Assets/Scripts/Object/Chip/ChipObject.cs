using GamConstant;
using UnityEngine;
using UniRx;

public class ChipObject : MonoBehaviour, Interactable
{
    [SerializeField] Animator animator;
    [SerializeField] MeshFilter meshFilter;

    [SerializeField] Mesh mesh_5;
    [SerializeField] Mesh mesh_25;
    [SerializeField] Mesh mesh_100;
    [SerializeField] Mesh mesh_500;
    [SerializeField] Mesh mesh_1000;
    [SerializeField] Mesh mesh_5000;

    [SerializeField] int chipValue;

    // Animation Keys
    protected const string ANI_KEY_Spawn = "Spawn";
    protected const string ANI_KEY_Hide = "Hide";
    protected const string ANI_KEY_Hit = "Hit";

    Rigidbody rb;
    Subject<int> chipValueSubject = new Subject<int>();

    public int ChipValue => chipValue;

    public void Interact(KeyCode keyCode)
    {
        if(keyCode == KeyCode.Mouse0)
        {
            chipValueSubject.OnNext(chipValue);
        }
    }

    public bool IsInteractable()
    {
        if (!GameController.Instance.IsBetPhase()) return false;

        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && rb.isKinematic;
    }

    public void SetChipValue(ChipType value)
    {
        chipValueSubject = new Subject<int>();
        rb = GetComponent<Rigidbody>();

        chipValue = (int)value;

        switch (value)
        {
            case ChipType.Chip_5:
                meshFilter.mesh = mesh_5;
                break;
            case ChipType.Chip_25:
                meshFilter.mesh = mesh_25;
                break;
            case ChipType.Chip_100:
                meshFilter.mesh = mesh_100;
                break;
            case ChipType.Chip_500:
                meshFilter.mesh = mesh_500;
                break;
            case ChipType.Chip_1000:
                meshFilter.mesh = mesh_1000;
                break;
            case ChipType.Chip_5000:
                meshFilter.mesh = mesh_5000;
                break;
        }
    }

    public void SetKinematic(bool set)
    {
        rb.isKinematic = set;
    }

    public Subject<int> OnChipInteracted() => chipValueSubject;

    public void Show()
    {
        gameObject.SetActive(true);
        animator.SetTrigger(ANI_KEY_Spawn);
    }

    public void Hide()
    {
        animator.SetTrigger(ANI_KEY_Hide);
    }

    public void Hit()
    {
        animator.SetTrigger(ANI_KEY_Hit);

    }
}
