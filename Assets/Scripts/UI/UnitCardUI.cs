using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class UnitCardUI 
    : MonoBehaviour
    , IReactsToEvent<ITurnPropertyChanged.UnitChanged>
    , IReactsToEvent<ITurnPropertyChanged.UnitDescVisibilityChanged>
{    
    public static bool IsTapped = false;

    public Transform OffscreenPivot;
    public Transform OnscreenPivot;
    public Transform HalfwayPivot;

    public GameObject SkipButton;
    public SpriteRenderer Image;
    
    public TMPro.TextMeshPro Power;
    public TMPro.TextMeshPro Health;
    public GameObject Ability;
    public SpriteRenderer AbilityBar;
    public Sprite AttackAbilityBar;
    public Sprite HitAbilityBar;

    public TMPro.TextMeshPro AbilityText;
    public SpriteRenderer AbilityIcon;
    public Sprite HitAbility;
    public Sprite DamageAbility;

    public TMPro.TextMeshPro NameText;

    private Vector3 _OnscreenPivot;
    private Vector3 _OffscreenPivot;
    private Vector3 _HalfwayPivot;

    public void Awake()
    {
        TurnManager.Instance.OnUnitChanged += React_OnUnitChanged;
        TurnManager.Instance.OnUnitDescVisibilityChanged += React_OnUnitDescVisibilityChanged;
    }

    private void React_OnUnitDescVisibilityChanged(UnitVisibility oldVis, UnitVisibility newVis)
    {
        switch(newVis)
        {
            case UnitVisibility.Hidden:
                GoOffscreen(); break;
            case UnitVisibility.Halfway:
                GoHalfway(); break;
            case UnitVisibility.Shown:
                GoOnscreen(); break;
        }
    }

    private void React_OnUnitChanged(TurnBasedUnit oldUnit, TurnBasedUnit newUnit)
    {
        if (newUnit != null)
            Load(newUnit.Definition);
        else
            GoOffscreen();
    }

    public void Start()
    {
        _OnscreenPivot = OnscreenPivot.localPosition;
        _OffscreenPivot = OffscreenPivot.localPosition;
        _HalfwayPivot = HalfwayPivot.localPosition;
    }

    public void GoHalfway()
    {
        this.transform.DOLocalMove(_HalfwayPivot, 0.25f);
    }

    public void GoOffscreen()
    {
        this.transform.DOLocalMove(_OffscreenPivot, 0.25f);
    }

    public void GoOnscreen()
    {
        this.transform.DOLocalMove(_OnscreenPivot, 0.25f);
    }

    public void Load(UnitScriptable unit)
    {
        SkipButton.SetActive(unit.Friend);

        NameText.text = unit.Name;
        Image.sprite = unit.Image;
        Image.flipX = !unit.Friend;

        Power.text = $"{unit.Power}";
        Health.text = $"{unit.Health}";

        if (unit.SuccessfulAttack != null)
        {
            Ability.SetActive(true);
            AbilityBar.sprite = AttackAbilityBar;
            AbilityIcon.sprite = HitAbility;
            AbilityText.text = unit.SuccessfulAttack.AbilityName;
        }
        else if (unit.TakingDamage != null)
        {
            Ability.SetActive(true);
            AbilityBar.sprite = HitAbilityBar;
            AbilityIcon.sprite = DamageAbility;
            AbilityText.text = unit.TakingDamage.AbilityName;
        }
        else
        {
            Ability.SetActive(false);
        }
    }

    public void OnMouseDown()
    {
        IsTapped = true;
    }

    public void OnMouseUp()
    {
        IsTapped = false;
    }
}
