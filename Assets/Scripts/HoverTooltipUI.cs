using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject hoverTooltip;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform hoverTooltipRectTransform;

    int itemLayerMask;

    private void OnEnable()
    {
        itemLayerMask = LayerMask.GetMask("Item");

        if (null == hoverTooltip) return;
        hoverTooltipRectTransform = hoverTooltip.GetComponent<RectTransform>();
        canvas = hoverTooltip.GetComponentInParent<Canvas>();
        if (null == canvas) return;
        canvasRectTransform = canvas.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        bool shouldTooltipDisplayed = ShouldTooltipDisplayed();
        hoverTooltip.SetActive(shouldTooltipDisplayed);
    }

    private bool ShouldTooltipDisplayed()
    {
        if (null == hoverTooltip || null == title || null == description) return false;
        if (null == canvas || null == canvasRectTransform || null == hoverTooltipRectTransform) return false;
        if (true != Input.mousePresent) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, itemLayerMask)) return false;

        GameObject prefab = GameManager.Instance.FindPrefabOfPooledGameObject(hit.transform.parent.gameObject);
        if (null == prefab || !GameManager.Instance.itemDataMap.TryGetValue(prefab, out ItemData itemData) || null == itemData.readOnlyDescription) return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localCoord);

        hoverTooltipRectTransform.pivot =
            new Vector2(
                (canvasRectTransform.rect.width / 2 < localCoord.x + hoverTooltipRectTransform.rect.width) ? 1f : 0f,
                (-canvasRectTransform.rect.height / 2 > localCoord.y - hoverTooltipRectTransform.rect.height) ? 0f : 1f);

        hoverTooltipRectTransform.localPosition = localCoord;


        title.text = prefab.name;
        description.text = itemData.readOnlyDescription;

        return true;
    }
}
