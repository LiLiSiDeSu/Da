using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelRoleList : PanelBaseDynamicScrollView
{
    private bool IsOpen = true;

    public GameObject ScrollView_;
    public GameObject BtnPackUp;
    public GameObject BtnOpen;
    public Transform ImgDecorateFrameTop;
    public Transform ImgDecorateFrameBottom;
    public Image ImgPackUp;
    public Image ImgOpen;

    protected override void Awake()
    {
        base.Awake();

        Hot.CenterEvent_.AddEventListener<KeyCode>("KeyDown",
        (key) =>
        {
            if (Hot.e_NowPlayerLocation != E_PlayerLocation.OnExpedition && key == Hot.MgrInput_.PanelRole)
            {

                if (Hot.PoolNowPanel_.ListNowPanel.Contains("PanelRoleList"))
                    Hot.MgrUI_.HidePanel(false, Hot.PanelRoleList_.gameObject, "PanelRoleList");
                else
                    Hot.MgrUI_.ShowPanel<PanelRoleList>(true, "PanelRoleList");
            }
        });

        ScrollView_ = transform.FindSonSonSon("ScrollView_").gameObject;
        BtnPackUp = transform.FindSonSonSon("BtnPackUp").gameObject;
        BtnOpen = transform.FindSonSonSon("BtnOpen").gameObject;
        ImgDecorateFrameTop = transform.FindSonSonSon("ImgDecorateFrameTop");
        ImgDecorateFrameBottom = transform.FindSonSonSon("ImgDecorateFrameBottom");
        ImgPackUp = transform.FindSonSonSon("ImgPackUp").GetComponent<Image>();
        ImgOpen = transform.FindSonSonSon("ImgOpen").GetComponent<Image>();
        ImgPackUp.alphaHitTestMinimumThreshold = 0.2f;
        ImgOpen.alphaHitTestMinimumThreshold = 0.2f;

        Content = transform.FindSonSonSon("RoleContent");

        BtnOpen.SetActive(false);
    }

    #region EventSystem�ӿ�ʵ��

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        Hot.e_NowPointerLocation = E_NowPointerLocation.PanelRoleList;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (Hot.DragingPanelCellRoleRecruit != null)
        {
            Hot.PaddingContentStep_.transform.SetParent(Hot.PanelRoleGuildRecruit_.Content, false);
        }

        Hot.e_NowPointerLocation = E_NowPointerLocation.None;
    }

    #endregion

    protected override void Button_OnClick(string controlname)
    {
        base.Button_OnClick(controlname);

        switch (controlname)
        {
            case "BtnPackUp":
                IsOpen = false;
                BtnPackUp.SetActive(false);
                BtnOpen.SetActive(true);
                ScrollView_.SetActive(false);
                ChangePosImgDecorateFrameBottom();
                break;
            case "BtnOpen":
                IsOpen = true;
                BtnPackUp.SetActive(true);
                BtnOpen.SetActive(false);
                ScrollView_.SetActive(true);
                ChangePosImgDecorateFrameBottom();
                break;
        }
    }

    public override void InitContent()
    {
        NowIndex = 0;

        for (int i = 0; i < Hot.DataNowCellGameArchive.ListCellRole.Count; i++)
        {
            int tempi = i;

            Hot.MgrUI_.CreatePanel<PanelCellRole>
            (false, "/PanelCellRole",
            (panel) =>
            {
                panel.Index = tempi;
                panel.CreatePanelCellRoleCanDrag();
                GameObject obj =
                    Hot.MgrRes_.Load<GameObject>("Prefabs/" + "DynamicContentStepFor" + panel.PrefabsDynamicContentStepSuffix);
                obj.name = tempi.ToString();
                obj.transform.SetParent(Content, false);
                obj.GetComponent<DynamicContentStep>().Init(tempi);
                panel.transform.SetParent(obj.GetComponent<DynamicContentStep>().DependentObjRoot, false);
                ListDynamicContentStep.Add(obj.GetComponent<DynamicContentStep>());

                panel.InitInfo(Hot.DataNowCellGameArchive.ListCellRole[tempi]);
            });

            NowIndex++;
        }
    }

    public override void Clear()
    {
        base.Clear();

        ContentStep[] all = Content.GetComponentsInChildren<ContentStep>();
        for (int i = 0; i < all.Length; i++)
        {
            DestroyImmediate(all[i].gameObject);
        }
    }

    public override void SortContent()
    {
        DynamicContentStep[] allDynamicContentStep = transform.GetComponentsInChildren<DynamicContentStep>();
        List<DynamicContentStep> tempList = new();
        for (int i = 0; i < allDynamicContentStep.Length; i++)
        {
            tempList.Add(allDynamicContentStep[i]);
            tempList[i].SetIndex(i);
            tempList[i].gameObject.name = i.ToString();
        }
        ListDynamicContentStep = tempList;

        PanelCellRole[] all = transform.GetComponentsInChildren<PanelCellRole>();
        List<DataContainer_CellRole> tempData = new();
        for (int i = 0; i < all.Length; i++)
        {
            tempData.Add(Hot.DataNowCellGameArchive.ListCellRole[all[i].Index]);
            all[i].Index = i;
        }
        Hot.DataNowCellGameArchive.ListCellRole = tempData;
    }

    private void ChangePosImgDecorateFrameBottom()
    {
        if (IsOpen && ListDynamicContentStep.Count > 0)
            ImgDecorateFrameBottom.transform.localPosition =
                new Vector3(ImgDecorateFrameBottom.transform.localPosition.x, -361f, 0);
        else
            ImgDecorateFrameBottom.transform.position =
                new Vector3(ImgDecorateFrameBottom.transform.position.x,
                ImgDecorateFrameTop.position.y - 40,
                0);
    }

    public void RemoveRole(PanelCellRole toRemove)
    {
        NowIndex--;
        DestroyImmediate(toRemove.RolePortraitCanDrag.gameObject);
        DestroyImmediate(ListDynamicContentStep[toRemove.Index].gameObject);
        SortContent();
    }
}
