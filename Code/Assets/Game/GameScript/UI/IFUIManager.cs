using UnityEngine;
using System.Collections;

public enum UIPanelState
{
    UIPS_NoPanel,
    UIPS_MenuPanel,//菜单面板
    UIPS_ChapterPanel,//章节选择面板

    UIPS_LevelBegin,
    UIPS_LevelInfoPanel,//关卡信息面板
    UIPS_ShopPanel,//关卡战前商店面板
    UIPS_FailPanel,//关卡失败面板
    UIPS_SettlePanel,//关卡成功结算面板
    UIPS_LevelEnd,

}

public class IFUIManager : MonoBehaviour
{
    /// <summary>
    /// The _ instance.本类的单例模式
    /// </summary>
    static public IFUIManager _Instance = null;

    /// <summary>
    /// 
    /// </summary>
    UIPanelState uiPanelState = UIPanelState.UIPS_NoPanel;

    /// <summary>
    /// 
    /// </summary>
    public bool bShowFPS;

    /// <summary>
    /// fps显示面板
    /// </summary>
    public GameObject fpsPanel;

    /// <summary>
    /// 章节显示面板
    /// </summary>
    public GameObject chapterPanel;

    /// <summary>
    /// 关卡显示面板
    /// </summary>
    public GameObject levelInfoPanel;

    /// <summary>
    /// 商店显示面板
    /// </summary>
    public GameObject shopPanel;

    /// <summary>
    /// 结算显示面板
    /// </summary>
    public GameObject settlePanel;

    /// <summary>
    /// 菜单Panel
    /// </summary>
    public GameObject menuPanel;

    public void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
    public void Start()
    {
        if (fpsPanel)
        {
            fpsPanel.SetActive(bShowFPS);
        }

        ChangePanel(UIPanelState.UIPS_MenuPanel);
    }
    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public void Destroy()
    {
        _Instance = null;
    }
    public void ChangePanel(UIPanelState uips)
    {
        if( uips != uiPanelState )
        {
            uiPanelState = uips;

            if (menuPanel)
            {
                menuPanel.SetActive(UIPanelState.UIPS_MenuPanel == uiPanelState);
            }

            if (chapterPanel)
            {
                chapterPanel.SetActive(UIPanelState.UIPS_ChapterPanel == uiPanelState);
            }

            if (levelInfoPanel)
            {
                levelInfoPanel.SetActive(UIPanelState.UIPS_LevelInfoPanel == uiPanelState);
            }

            if (shopPanel)
            {
                shopPanel.SetActive(UIPanelState.UIPS_ShopPanel == uiPanelState);
            }

            if (settlePanel)
            {
                settlePanel.SetActive(UIPanelState.UIPS_SettlePanel == uiPanelState);
            }
        }
    }
    /// <summary>
    /// 显示提示
    /// </summary>
    public void ShowTips()
    {

    }
}