using UnityEngine;

// Base class for any list in the shop (Consumable, Character, Themes)
public abstract class ShopList : MonoBehaviour
{
    public GameObject prefabItem;
    public RectTransform listRoot;
    public int k_UILayer;


    public delegate void RefreshCallback();

	protected RefreshCallback m_RefreshCallback;

    private void Awake()
    {
        k_UILayer = LayerMask.NameToLayer("UI");
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Populate();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        m_RefreshCallback = null;
    }

	public void Refresh()
	{
		m_RefreshCallback();
	}

    public abstract void Populate();
}
