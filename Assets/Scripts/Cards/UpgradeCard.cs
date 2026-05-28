using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private Button upgradeOpener;
    [SerializeField] private Button upgradeCloser;
    [SerializeField] private Button upgradeUnit;
    [SerializeField] private Image upgradePanel;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardHp;
    [SerializeField] private TMP_Text cardMoveSpeed;
    [SerializeField] private TMP_Text cardAttackDamage;
    [SerializeField] private TMP_Text cardAttackRange;
    [SerializeField] private TMP_Text cardAttackSpeed;

    private UnitData cardData;

    void Start()
    {
        Inisialize();
        upgradeOpener.onClick.AddListener(OpenUpgrade);
        upgradeCloser.onClick.AddListener(CloseUpgrade);
        upgradeUnit.onClick.AddListener(UpgradeUnit);
    }
    private void Inisialize()
    {

        cardName.text = cardData.name;
        cardHp.text = "HP : " + cardData.hp.ToString();
        cardMoveSpeed.text = "Move Speed : " + cardData.moveSpeed.ToString();
        cardAttackDamage.text = "Attack Damage : " + cardData.attackDamage.ToString();
        cardAttackRange.text = "Attack Range : " + cardData.attackRange.ToString();
        cardAttackSpeed.text = "Attack Speed : " + cardData.attackSpeed.ToString();
    }
    private void OpenUpgrade()
    {
        upgradePanel.transform.SetParent(PopUpManager.Instance.GetUpgradeCardParent());
        upgradePanel.transform.localPosition = Vector3.zero;
        upgradePanel.rectTransform.sizeDelta = Vector2.zero;
        cardName.rectTransform.sizeDelta = Vector2.zero;
        cardHp.rectTransform.sizeDelta = Vector2.zero;
        cardMoveSpeed.rectTransform.sizeDelta = Vector2.zero;
        cardAttackDamage.rectTransform.sizeDelta = Vector2.zero;
        cardAttackRange.rectTransform.sizeDelta = Vector2.zero;
        cardAttackSpeed.rectTransform.sizeDelta = Vector2.zero;
        upgradeCloser.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        upgradeUnit.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(upgradePanel.rectTransform.DOSizeDelta(new Vector2(1129f, 1811f), 0.4f).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.01f);
        sequence.Append(cardName.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardHp.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardMoveSpeed.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackDamage.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackRange.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackSpeed.rectTransform.DOSizeDelta(new Vector2(992f, 196f), 0.3f).SetEase(Ease.OutBack));

        sequence.AppendInterval(0.01f);

        sequence.Append(upgradeCloser.GetComponent<RectTransform>().DOSizeDelta(new Vector2(890f, 128f), 0.3f).SetEase(Ease.OutBack));
        sequence.Join(upgradeUnit.GetComponent<RectTransform>().DOSizeDelta(new Vector2(890f, 225f), 0.3f).SetEase(Ease.OutBack));
    }

    private void CloseUpgrade()
    {



        Sequence sequence = DOTween.Sequence();





        sequence.Append(cardName.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardHp.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardMoveSpeed.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackDamage.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackRange.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(cardAttackSpeed.rectTransform.DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));

        sequence.AppendInterval(0.01f);
        sequence.Append(upgradeUnit.GetComponent<RectTransform>().DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(upgradeCloser.GetComponent<RectTransform>().DOSizeDelta(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.01f);
        sequence.Append(upgradePanel.rectTransform.DOSizeDelta(Vector2.zero, 0.4f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() => SetOrginalParent());


    }
    private void SetOrginalParent()
    {
        upgradePanel.transform.SetParent(transform);
        upgradePanel.transform.localPosition = Vector3.zero;
    }
    private void UpgradeUnit()
    {

    }
    public void SetData(UnitData data)
    {
        cardData = data;
    }
}
