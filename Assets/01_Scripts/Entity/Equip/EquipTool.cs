using UnityEngine;

public class EquipTool : Equip
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    public float attackRate;
    private bool _attacking;
    public float attackDistance;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
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
    private void OnDestroy()
    {
        if (InputManager.Instance)
            InputManager.Instance.OnAttackPressed -= OnAttackInput;
    }

    public override void OnAttackInput()
    {
        if (!_attacking)
        {  
            _attacking = true;
            _animator.SetTrigger(Attack);
            Invoke(nameof(OnCanAttack), attackRate);
        }
    }

    void OnCanAttack()
    {
        _attacking = false;
    }
}