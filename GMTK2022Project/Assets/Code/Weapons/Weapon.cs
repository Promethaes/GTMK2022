﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public GameObject weaponOwner = null;
    public SpriteRenderer spriteRenderer = null;
    public Animator animator = null;
    public string animationName = "weapon attack";

    [Tooltip("-1 for infinite")]
    [SerializeField] protected float cooldown = 0.0f;
    [SerializeField] protected float finishedAttackCooldown = 0.5f;

    public bool CanAttack => (!_attacking && _cooldown < 0.0f);

    [Tooltip("Adds another check for CanAttack() to pass or fail")]

    public UnityEvent OnAttack;
    public UnityEvent OnFinishAttack;
    public UnityEvent OnEnableWeapon;
    public UnityEvent OnDisableWeapon;

    bool _bFinishAttack = true;
    float _finishAttack = 0.0f;
    float _cooldown = 0.0f;
    protected bool _attacking = false;

    bool DebugCanAttack = false;

    bool didDamage = false;

    private void Awake()
    {
        OnAttack.AddListener(PlayAnimation);
    }

    private void OnEnable()
    {
        didDamage = false;
        _cooldown = cooldown;
        _finishAttack = 0.0f;
        OnEnableWeapon.Invoke();
        IEnumerator Cooldown()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _finishAttack += Time.deltaTime;
                if (_finishAttack >= finishedAttackCooldown && !_bFinishAttack)
                {
                    _bFinishAttack = true;
                    OnFinishAttack.Invoke();
                    if(didDamage)
                        gameObject.SetActive(false);
                }
                _cooldown -= Time.deltaTime;
            }
        }
        StartCoroutine(Cooldown());
    }

    private void OnDisable()
    {
        OnDisableWeapon.Invoke();
    }

    protected float GetCooldown()
    {
        return _cooldown;
    }

    public float GetFinishedAttackCooldown()
    {
        return finishedAttackCooldown;
    }

    protected bool IsAttacking()
    {
        return _attacking;
    }

    private void Update()
    {
        DebugCanAttack = CanAttack;
    }

    public void SetAttacking(bool b)
    {
        _attacking = b;
    }

    protected void ResetCooldown()
    {
        _cooldown = cooldown;
        _finishAttack = 0.0f;
        _bFinishAttack = false;
    }

    public virtual void Attack()
    {
        if (!CanAttack)
            return;
        ResetCooldown();
        OnAttack.Invoke();
    }

    public void PlayAnimation()
    {
        animator.Play(animationName);
    }

    public void SetDidDamage(bool yn)
    {
        didDamage = yn;
    }

}
