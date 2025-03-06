using UnityEngine;

public class EquipTool : Equip
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    public float attackRate;
    private bool _attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resource Gathering")] public bool doesGatherResources;

    [Header("Combat")] public bool doesDealDamage;
    public int damage;


    private Animator _animator;
    private Camera _equipCamera;

    private void Awake()
    {
        _equipCamera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnAttackPressed += OnAttackInput;
    }

    private void OnDisable()
    {
        if (InputManager.Instance)
            InputManager.Instance.OnAttackPressed -= OnAttackInput;
    }

    public override void OnAttackInput()
    {
        if (_attacking) return;
        bool canAttack = SignalManager.Instance.EmitSignal<float, bool>("CanAttack", useStamina);
        if (!canAttack) return;
        _attacking = true;
        _animator.SetTrigger(Attack);
        Invoke(nameof(OnCanAttack), attackRate);
    }

    //
    // if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
    // +            {
    //     +                attacking = true;
    //     +                animator.SetTrigger("Attack");
    //     +                Invoke("OnCanAttack", attackRate);
    //     +            }
    void OnCanAttack()
    {
        _attacking = false;
    }

    public void OnHit()
    {
        var ray = _equipCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (!Physics.Raycast(ray, out var hit, attackDistance)) return;
        if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
        {
            resource.Gather(hit.point, hit.normal);
        }
        
        if ( doesDealDamage && hit.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }
}