using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    public Button choiceButtonPrefab;
    public string[] itemList = {};
    private bool isBuying;
    private List<Button> buttons = new List<Button>();

    [Header("Reference")]
    private TopDownPlayerMovement playerMovement;
    private RectTransform merchantPanel;
    public RectTransform itemContainer;

    private void Awake()
    {
        // Transform canvas = transform.Find("Canvas");
        // merchantPanel = canvas.Find("MerchantPanel").GetComponent<RectTransform>();
        // itemContainer = canvas.Find("MerchantPanel").Find("ItemContainer").GetComponent<RectTransform>();
        Transform canvas = transform.Find("Canvas");

        if (canvas != null)
        {
            // Find MerchantPanel inside Canvas
            Transform panel = canvas.Find("MerchantPanel");
            if (panel != null)
            {
                merchantPanel = panel.GetComponent<RectTransform>();

                // Find ItemContainer inside MerchantPanel
                Transform container = panel.Find("ItemContainer");
                if (container != null)
                {
                    itemContainer = container.GetComponent<RectTransform>();
                }
                else Debug.LogWarning("ItemContainer not found under MerchantPanel");
            }
            else Debug.LogWarning("MerchantPanel not found under Canvas");
        }
        else Debug.LogWarning("Canvas not found under Merchant");
    }

    private void Start()
    {
        merchantPanel.gameObject.SetActive(false);
    }
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && isBuying)
        {
            CloseMerchant();
        }
    }
    private void OnTriggerEnter2D(Collider2D c)
    {
        Debug.Log("collided with "+c.name);
        playerMovement = c.GetComponent<TopDownPlayerMovement>();
        if (isBuying || !c.CompareTag("Player")) return;
       
        OpenMerchant();
        
    }

    private void OpenMerchant()
    {
        isBuying=true;
        merchantPanel.gameObject.SetActive(true);
        playerMovement.enabled=false;

        ClearButtons();
        CreateButtons();
        SetupNavigation();
        SelectFirstButton();
    }

    private void CloseMerchant()
    {
        isBuying=false;
        merchantPanel.gameObject.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        playerMovement.enabled=true;
    }

    private void ClearButtons()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        buttons.Clear();
    }

    private void CreateButtons()
    {
        for (int i=0; i<itemList.Length; i++)
        {
            int idx = i;
            Button btn = Instantiate(choiceButtonPrefab,itemContainer);
            btn.GetComponentInChildren<Text>().text = itemList[i];
            btn.onClick.AddListener(() => OnItemSelected(idx));
            buttons.Add(btn);
        }
    }

    private void SetupNavigation()
    {
        for (int i=0;i<buttons.Count; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = i>0 ? buttons[i-1] : null;
            nav.selectOnDown = i<buttons.Count-1 ? buttons[i+1] : null;

            buttons[i].navigation = nav;
        }
    }

    private void SelectFirstButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
    }

    private void OnItemSelected(int index)
    {
        Debug.Log("selected "+itemList[index]);
    }
}
