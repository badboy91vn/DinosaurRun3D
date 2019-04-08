using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopCharacterList : ShopList
{
    private bool m_IsLoadingCharacter;
    protected readonly Quaternion k_FlippedYAxisRotation = Quaternion.Euler(0f, 220f, 0f);

    void Start()
    {
        PlayerData.Create();
        Populate();
    }

    public IEnumerator PopulateCharacters(Transform charPosition, int charIndex)
    {
        if (!m_IsLoadingCharacter)
        {
            m_IsLoadingCharacter = true;
            GameObject newChar = null;
            while (newChar == null)
            {
                Character c = CharacterDatabase.GetCharacter(PlayerData.instance.characters[charIndex]); // PlayerData.instance.usedCharacter

                if (c != null)
                {
                    newChar = Instantiate(c.gameObject);
                    Helpers.SetRendererLayerRecursive(newChar, k_UILayer);
                    newChar.transform.SetParent(charPosition, false);
                    newChar.transform.rotation = k_FlippedYAxisRotation;
                    newChar.transform.localScale = new Vector3(100f, 100f, 100f);

                    //if (m_Character != null)
                    //    Destroy(m_Character);

                    //m_Character = newChar;

                    //m_Character.transform.localPosition = Vector3.right * 1000;
                    ////animator will take a frame to initialize, during which the character will be in a T-pose.
                    ////So we move the character off screen, wait that initialised frame, then move the character back in place.
                    ////That avoid an ugly "T-pose" flash time
                    //yield return new WaitForEndOfFrame();
                    //m_Character.transform.localPosition = Vector3.zero;
                }
                else
                    yield return new WaitForSeconds(1.0f);
            }
            m_IsLoadingCharacter = false;
        }
    }

    public override void Populate()
    {
        m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        int charIndex = 0;
        foreach (KeyValuePair<string, Character> pair in CharacterDatabase.dictionary)
        {
            Character c = pair.Value;
            if (c != null)
            {
                GameObject newEntry = Instantiate(prefabItem);
                newEntry.transform.SetParent(listRoot, false);

                ShopItemChar itm = newEntry.GetComponent<ShopItemChar>();

                StartCoroutine(PopulateCharacters(itm.charPosition, charIndex));

    //            itm.nameText.text = c.characterName;
				//itm.pricetext.text = c.cost.ToString();

				//itm.buyButton.image.sprite = itm.buyButtonSprite;

				//if (c.premiumCost > 0)
				//{
				//	itm.premiumText.transform.parent.gameObject.SetActive(true);
				//	itm.premiumText.text = c.premiumCost.ToString();
				//}
				//else
				//{
				//	itm.premiumText.transform.parent.gameObject.SetActive(false);
				//}

				//itm.buyButton.onClick.AddListener(delegate () { Buy(c); });

				//m_RefreshCallback += delegate() { RefreshButton(itm, c); };
				//RefreshButton(itm, c);

                charIndex++;
            }
        }
    }

	protected void RefreshButton(ShopItemChar itm, Character c)
	{
		if (c.cost > PlayerData.instance.coins)
		{
			itm.buyButton.interactable = false;
			itm.pricetext.color = Color.red;
		}
		else
		{
			itm.pricetext.color = Color.black;
		}

		if (c.premiumCost > PlayerData.instance.premium)
		{
			itm.buyButton.interactable = false;
			itm.premiumText.color = Color.red;
		}
		else
		{
			itm.premiumText.color = Color.black;
		}

		if (PlayerData.instance.characters.Contains(c.characterName))
		{
			itm.buyButton.interactable = false;
			itm.buyButton.image.sprite = itm.disabledButtonSprite;
			itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Owned";
		}
	}

	public void Buy(Character c)
    {
        PlayerData.instance.coins -= c.cost;
		PlayerData.instance.premium -= c.premiumCost;
        PlayerData.instance.AddCharacter(c.characterName);
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "store";
        var level = PlayerData.instance.rank.ToString();
        var itemId = c.characterName;
        var itemType = "non_consumable";
        var itemQty = 1;

        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Soft,
            transactionContext,
            itemQty,
            itemId,
            itemType,
            level,
            transactionId
        );
        
        if (c.cost > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                c.cost,
                itemId,
                PlayerData.instance.coins, // Balance
                itemType,
                level,
                transactionId
            );
        }

        if (c.premiumCost > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                c.premiumCost,
                itemId,
                PlayerData.instance.premium, // Balance
                itemType,
                level,
                transactionId
            );
        }
#endif

        // Repopulate to change button accordingly.
        Populate();
    }

    public void CloseScene()
    {
        SceneManager.UnloadSceneAsync("CharShop");
        LoadoutState loadoutState = GameManager.instance.topState as LoadoutState;
        if (loadoutState != null)
        {
            loadoutState.Refresh();
        }
    }
}
