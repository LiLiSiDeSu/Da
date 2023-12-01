using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PanelOtherMiniMapEditor : PanelBase
{
    public string PathFolder;

    public m_Vector2 EntrancePos = new(-1, -1);

    public List<List<PanelCellGridMiniMapEditor>> Grids = new();
    public List<List<Transform>> ItemRoot = new();

    public Image ImgCurrentChoose;
    
    public InputField IptFileName;
    public InputField IptWidth;
    public InputField IptHeight;
    public InputField IptLoad;
    
    public Transform CellRoomScrollView;
    public Transform CellRoomContent;
    public Transform CellHallScrollView;
    public Transform CellHallContent;

    public Transform AllContent;
    public Transform ImgBkContent;
    public Transform ImgStatusContent;
    public Transform ItemContent;
    public Transform ComponentRoot;

    protected override void Awake()
    {
        base.Awake();

        PathFolder = "/MapTemplet";

        Hot.CenterEvent_.AddEventListener<KeyCode>("KeyDown",
        (key) =>
        {
            if (Hot.NowEnterCellMiniMapEditor != null && key == Hot.MgrInput_.Enter)
            {
                Hot.MgrUI_.ShowPanel<PanelOtherRoomEditor>(true, "PanelOtherRoomEditor");
                Hot.NowEditorDependency = Hot.NowEnterCellMiniMapEditor;
                Hot.PanelOtherRoomEditor_.LoadRoomConfig(Hot.NowEditorDependency.Map);
            }
        });

        //这里可以用PoolInvoke来优化
        //但以后再说吧
        Hot.CenterEvent_.AddEventListener<KeyCode>("KeyDown", 
        (key) =>
        {
            if (Hot.PoolNowPanel_.ContainPanel("PanelOtherMiniMapEditor") && key == Hot.MgrInput_.Cancel)
            {
                //删除创建的MiniMapCell
                if (Hot.NowEnterCellMiniMapEditor != null || Hot.ChoseCellMapEditor != null) 
                {
                    if (Hot.ChoseCellMapEditor == null)
                    {
                        if (Hot.NowEnterCellMiniMapEditor.e_Hall != E_CellExpeditionMiniMapHall.None)
                        {
                            for (int i1 = 0; i1 < Hot.BodyDicHall[Hot.NowEnterCellMiniMapEditor.e_Hall].Y; i1++)
                            {
                                for (int i2 = 0; i2 < Hot.BodyDicHall[Hot.NowEnterCellMiniMapEditor.e_Hall].X; i2++)
                                {
                                    Hot.PanelOtherMapEditor_.Grids[Hot.NowEnterCellMiniMapEditor.RootGrid.Y + i1][Hot.NowEnterCellMiniMapEditor.RootGrid.X + i2].CellMiniMapEditor = null;
                                }
                            }
                        }

                        if (Hot.NowEnterCellMiniMapEditor.e_Room != E_CellExpeditionMiniMapRoom.None)
                        {
                            for (int i1 = 0; i1 < Hot.BodyDicRoom[Hot.NowEnterCellMiniMapEditor.e_Room].Y; i1++)
                            {
                                for (int i2 = 0; i2 < Hot.BodyDicRoom[Hot.NowEnterCellMiniMapEditor.e_Room].X; i2++)
                                {
                                    Hot.PanelOtherMapEditor_.Grids[Hot.NowEnterCellMiniMapEditor.RootGrid.Y + i1][Hot.NowEnterCellMiniMapEditor.RootGrid.X + i2].CellMiniMapEditor = null;
                                }
                            }
                        }

                        if (Hot.NowEnterCellMiniMapEditor.e_Room == E_CellExpeditionMiniMapRoom.CellMapRoomEntrance)
                        {
                            EntrancePos = new(-1, -1);
                        }

                        Destroy(Hot.NowEnterCellMiniMapEditor.gameObject);
                    }
                    else
                    {
                        Hot.ChoseCellMapEditor.ImgStatus.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");
                        Hot.ChoseCellMapEditor.ImgCellMiniMapEditor.raycastTarget = true;
                        Hot.ChoseCellMapEditor = null;

                        ClearGridsImgStatus();
                    }
                }
                else //取消现在选择的MiniMapCell
                {
                    ImgCurrentChoose.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");
                    Hot.e_ChoseHall = E_CellExpeditionMiniMapHall.None;
                    Hot.e_ChoseRoom = E_CellExpeditionMiniMapRoom.None;

                    ClearGridsImgStatus();
                }
            }
        });

        Hot.CenterEvent_.AddEventListener<KeyCode>("KeyHold",
        (key) =>
        {
            if (Hot.PoolNowPanel_.ContainPanel("PanelOtherMiniMapEditor"))
            {
                if (key == Hot.MgrInput_.Add)
                {
                    AllContent.localScale +=
                        new Vector3(Hot.ValueChangeMapSize * Time.deltaTime, Hot.ValueChangeMapSize * Time.deltaTime);
                    AllContent.localScale +=
                        new Vector3(Hot.ValueChangeMapSize * Time.deltaTime, Hot.ValueChangeMapSize * Time.deltaTime);
                }

                if (key == Hot.MgrInput_.Reduce)
                {
                    AllContent.localScale -=
                        new Vector3(Hot.ValueChangeMapSize * Time.deltaTime, Hot.ValueChangeMapSize * Time.deltaTime, 0);
                    AllContent.localScale -=
                        new Vector3(Hot.ValueChangeMapSize * Time.deltaTime, Hot.ValueChangeMapSize * Time.deltaTime, 0);
                }
            }            
        });

        ImgCurrentChoose = transform.FindSonSonSon("ImgCurrentChoose").GetComponent<Image>();
        ImgCurrentChoose.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");        

        IptFileName = transform.FindSonSonSon("IptFileName").GetComponent<InputField>();
        IptWidth = transform.FindSonSonSon("IptWidth").GetComponent<InputField>();
        IptHeight = transform.FindSonSonSon("IptHeight").GetComponent<InputField>();
        IptLoad = transform.FindSonSonSon("IptLoad").GetComponent<InputField>();

        transform.FindSonSonSon("ImgSave").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
        transform.FindSonSonSon("ImgGenerate").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
        transform.FindSonSonSon("ImgLoad").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
        transform.FindSonSonSon("ImgClearMap").GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;

        CellRoomScrollView = transform.FindSonSonSon("CellRoomScrollView");
        CellHallScrollView = transform.FindSonSonSon("CellHallScrollView");
        CellRoomContent = transform.FindSonSonSon("CellRoomContent");
        CellHallContent = transform.FindSonSonSon("CellHallContent");

        AllContent = transform.FindSonSonSon("AllContent");
        ImgBkContent = transform.FindSonSonSon("ImgBkContent");
        ImgStatusContent = transform.FindSonSonSon("ImgStatusContent");
        ItemContent = transform.FindSonSonSon("ItemContent");
        ComponentRoot = transform.FindSonSonSon("ComponentRoot");

        InitChooseContent();        

        CellHallScrollView.gameObject.SetActive(false);
        CellRoomScrollView.gameObject.SetActive(false);
    }

    protected override void Button_OnClick(string controlname)
    {
        base.Button_OnClick(controlname);

        switch (controlname)
        {
            case "BtnGenerate":
                if (IptFileName.text != "" && IptWidth.text != "" && IptHeight.text != "")
                {
                    GenerateGrid();
                }
                break;
            case "BtnSave":
                if (IptFileName.text != "" && IptWidth.text != "" && IptHeight.text != "" && EntrancePos.X != -1)
                {
                    Save();
                }
                break;
            case "BtnLoad":
                if (File.Exists(Hot.MgrJson_.filePath + PathFolder + "/" + IptLoad.text + ".json"))
                {
                    Load(IptLoad.text);
                }
                break;
            case "BtnClearMap":
                ClearMap();
                IptFileName.text = "";
                IptWidth.text = "";
                IptHeight.text = "";
                EntrancePos = new();
                break;
            case "BtnChooseCellRoom":
                if (CellRoomScrollView.gameObject.activeSelf)
                {
                    CellRoomScrollView.gameObject.SetActive(false);
                }
                else
                {
                    CellRoomScrollView.gameObject.SetActive(true);
                }
                Hot.e_ChoseRoom = E_CellExpeditionMiniMapRoom.None;
                Hot.e_ChoseHall = E_CellExpeditionMiniMapHall.None;
                Hot.PanelOtherMapEditor_.ImgCurrentChoose.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");
                CellHallScrollView.gameObject.SetActive(false);                
                break;
            case "BtnChooseCellHall":
                if (CellHallScrollView.gameObject.activeSelf)
                {
                    CellHallScrollView.gameObject.SetActive(false);                    
                }
                else
                {
                    CellHallScrollView.gameObject.SetActive(true);
                }
                Hot.e_ChoseRoom = E_CellExpeditionMiniMapRoom.None;
                Hot.e_ChoseHall = E_CellExpeditionMiniMapHall.None;
                Hot.PanelOtherMapEditor_.ImgCurrentChoose.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");
                CellRoomScrollView.gameObject.SetActive(false);                
                break;
        }
    }

    public void InitChooseContent()
    {
        foreach (E_CellExpeditionMiniMapRoom e_CellExpeditionMiniMapRoom in Enum.GetValues(typeof(E_CellExpeditionMiniMapRoom)))
        {
            if (e_CellExpeditionMiniMapRoom != E_CellExpeditionMiniMapRoom.None)
            {
                Hot.MgrUI_.CreatePanel<PanelCellMapEditorChooseRoom>
                (false, "/PanelCellMapEditorChooseRoom",
                (panel) =>
                {
                    panel.Init(e_CellExpeditionMiniMapRoom);
                    panel.transform.SetParent(CellRoomContent, false);
                });
            }
        }

        foreach (E_CellExpeditionMiniMapHall e_CellExpeditionMiniMapHall in Enum.GetValues(typeof(E_CellExpeditionMiniMapHall)))
        {
            if (e_CellExpeditionMiniMapHall != E_CellExpeditionMiniMapHall.None)
            {
                Hot.MgrUI_.CreatePanel<PanelCellMapEditorChooseHall>(false, "/PanelCellMapEditorChooseHall",
                (panel) =>
                {
                    panel.Init(e_CellExpeditionMiniMapHall);
                    panel.transform.SetParent(CellHallContent, false);
                });
            }            
        }
    }

    public void GenerateGrid()
    {        
        ClearMap();

        (AllContent as RectTransform).sizeDelta =
            new(int.Parse(IptWidth.text) * Hot.BodySizeCellMinimap.X,
                int.Parse(IptHeight.text) * Hot.BodySizeCellMinimap.Y);

        transform.FindSonSonSon("ImgBkContent").GetComponent<GridLayoutGroup>().constraintCount = int.Parse(IptWidth.text);
        transform.FindSonSonSon("ImgStatusContent").GetComponent<GridLayoutGroup>().constraintCount = int.Parse(IptWidth.text);

        for (int i1 = 0; i1 < int.Parse(IptHeight.text); i1++)
        {
            int tempi1 = i1;

            Grids.Add(new());
            ItemRoot.Add(new());

            GameObject obj1 = Hot.MgrRes_.Load<GameObject>("Prefabs/" + "ContentStep");
            obj1.transform.SetParent(ItemContent, false);
            GridLayoutGroup glg = obj1.AddComponent<GridLayoutGroup>();
            glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            glg.constraintCount = 1;
            glg.childAlignment = TextAnchor.MiddleCenter;            

            for (int i2 = 0; i2 < int.Parse(IptWidth.text); i2++)
            {
                int tempi2 = i2;

                Grids[tempi1].Add(new());

                GameObject obj2 = Hot.MgrRes_.Load<GameObject>("Prefabs/" + "ContentStep");
                obj2.transform.SetParent(obj1.transform, false);

                ItemRoot[tempi1].Add(obj2.transform);

                Hot.MgrUI_.CreatePanel<PanelCellGridMiniMapEditor>(false, "/PanelCellGridMiniMapEditor",
                (PanelCellMapEditorGrid_) =>
                {
                    Grids[tempi1][tempi2] = PanelCellMapEditorGrid_;

                    PanelCellMapEditorGrid_.transform.SetParent(ComponentRoot, false);
                    PanelCellMapEditorGrid_.ImgBk.transform.SetParent(ImgBkContent, false);
                    PanelCellMapEditorGrid_.ImgStatus.transform.SetParent(ImgStatusContent, false);

                    PanelCellMapEditorGrid_.X = tempi2;
                    PanelCellMapEditorGrid_.Y = tempi1;
                });
            }
        }

        ChangeSize();
    }

    public void GenerateGridByLoadData(DataContainer_Expedition MapData)
    {
        ClearMap();

        (AllContent as RectTransform).sizeDelta =
            new(int.Parse(IptWidth.text) * Hot.BodySizeCellMinimap.X,
                int.Parse(IptHeight.text) * Hot.BodySizeCellMinimap.Y);

        transform.FindSonSonSon("ImgBkContent").GetComponent<GridLayoutGroup>().constraintCount = int.Parse(IptWidth.text);
        transform.FindSonSonSon("ImgStatusContent").GetComponent<GridLayoutGroup>().constraintCount = int.Parse(IptWidth.text);

        EntrancePos = MapData.EntrancePos;

        for (int i1 = 0; i1 < MapData.ListCellMiniMap.Count; i1++)
        {
            int tempi1 = i1;

            Grids.Add(new());
            ItemRoot.Add(new());

            GameObject obj1 = Hot.MgrRes_.Load<GameObject>("Prefabs/" + "ContentStep");
            obj1.transform.SetParent(ItemContent, false);
            GridLayoutGroup glg = obj1.AddComponent<GridLayoutGroup>();
            glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            glg.constraintCount = 1;
            glg.childAlignment = TextAnchor.MiddleCenter;            

            for (int i2 = 0; i2 < MapData.ListCellMiniMap[tempi1].Count; i2++)
            {
                int tempi2 = i2;

                Grids[tempi1].Add(new());

                GameObject obj2 = Hot.MgrRes_.Load<GameObject>("Prefabs/" + "ContentStep");
                obj2.transform.SetParent(obj1.transform, false);

                ItemRoot[tempi1].Add(obj2.transform);

                Hot.MgrUI_.CreatePanel<PanelCellGridMiniMapEditor>(false, "/PanelCellGridMiniMapEditor",
                (PanelCellMapEditorGrid_) =>
                {
                    Grids[tempi1][tempi2] = PanelCellMapEditorGrid_;

                    PanelCellMapEditorGrid_.transform.SetParent(ComponentRoot, false);
                    PanelCellMapEditorGrid_.ImgBk.transform.SetParent(ImgBkContent, false);
                    PanelCellMapEditorGrid_.ImgStatus.transform.SetParent(ImgStatusContent, false);

                    PanelCellMapEditorGrid_.X = tempi2;
                    PanelCellMapEditorGrid_.Y = tempi1;                    
                });
            }
        }

        GenerateItemByLoad(MapData);

        ChangeSize();
    }

    public void GenerateItemByLoad(DataContainer_Expedition MapData)
    {
        for (int i1 = 0; i1 < Grids.Count; i1++)
        {
            int tempi1 = i1;

            for (int i2 = 0; i2 < Grids[tempi1].Count; i2++)
            {
                int tempi2 = i2;

                if (MapData.ListCellMiniMap[tempi1][tempi2].e_Hall != E_CellExpeditionMiniMapHall.None)
                {
                    Hot.MgrUI_.CreatePanel<PanelCellMiniMapEditor>(false, "/PanelCellMiniMapEditor",
                    (PanelCellMiniMapEditor_) =>
                    {
                        PanelCellMiniMapEditor_.transform.SetParent(ItemRoot[tempi1][tempi2].transform, false);
                        PanelCellMiniMapEditor_.transform.localPosition = new Vector3(-20, 20);

                        PanelCellMiniMapEditor_.RootGrid = Grids[tempi1][tempi2];            
                        
                        for (int i3 = 0; i3 < Hot.BodyDicHall[MapData.ListCellMiniMap[tempi1][tempi2].e_Hall].Y; i3++)
                        {
                            for (int i4 = 0; i4 < Hot.BodyDicHall[MapData.ListCellMiniMap[tempi1][tempi2].e_Hall].X; i4++)
                            {
                                Grids[tempi1 + i3][tempi2 + i4].CellMiniMapEditor = PanelCellMiniMapEditor_;
                            }
                        }                        

                        PanelCellMiniMapEditor_.Init(E_CellExpeditionMiniMapRoom.None, MapData.ListCellMiniMap[tempi1][tempi2].e_Hall);

                        for (int i5 = 0; i5 < Hot.BodyExpeditionRoom.Y; i5++)
                        {
                            int tempi5 = i5;

                            for (int i6 = 0; i6 < Hot.BodyExpeditionRoom.X; i6++)
                            {
                                int tempi6 = i6;

                                PanelCellMiniMapEditor_.Map[tempi5][tempi6].Init(tempi5, tempi6);

                                if (MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj != E_MapObject.None)
                                {
                                    PanelCellMiniMapEditor_.Map[tempi5][tempi6].IsHave = true;

                                    for (int j1 = 0; j1 < Hot.BodyDicMapObject[MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj].Y; j1++)
                                    {
                                        for (int j2 = 0; j2 < Hot.BodyDicMapObject[MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj].X; j2++)
                                        {
                                            PanelCellMiniMapEditor_.Map[tempi5 + j1][tempi6 + j2].e_Obj = MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj;
                                        }
                                    }
                                }
                            }
                        }
                    });
                }


                if (MapData.ListCellMiniMap[tempi1][tempi2].e_Room != E_CellExpeditionMiniMapRoom.None)
                {
                    Hot.MgrUI_.CreatePanel<PanelCellMiniMapEditor>(false, "/PanelCellMiniMapEditor",
                    (PanelCellMiniMapEditor_) =>
                    {
                        PanelCellMiniMapEditor_.transform.SetParent(ItemRoot[tempi1][tempi2].transform, false);
                        PanelCellMiniMapEditor_.transform.localPosition = new Vector3(-20, 20);

                        PanelCellMiniMapEditor_.RootGrid = Grids[tempi1][tempi2];

                        for (int i3 = 0; i3 < Hot.BodyDicRoom[MapData.ListCellMiniMap[tempi1][tempi2].e_Room].Y; i3++)
                        {
                            for (int i4 = 0; i4 < Hot.BodyDicRoom[MapData.ListCellMiniMap[tempi1][tempi2].e_Room].X; i4++)
                            {
                                Grids[tempi1 + i3][tempi2 + i4].CellMiniMapEditor = PanelCellMiniMapEditor_;
                            }
                        }

                        //Init Map
                        PanelCellMiniMapEditor_.Init(MapData.ListCellMiniMap[tempi1][tempi2].e_Room, E_CellExpeditionMiniMapHall.None);

                        for (int i5 = 0; i5 < Hot.BodyExpeditionRoom.Y; i5++)
                        {
                            int tempi5 = i5;

                            for (int i6 = 0; i6 < Hot.BodyExpeditionRoom.X; i6++)
                            {
                                int tempi6 = i6;

                                PanelCellMiniMapEditor_.Map[tempi5][tempi6].Init(tempi5, tempi6);

                                if (MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj != E_MapObject.None)
                                {
                                    PanelCellMiniMapEditor_.Map[tempi5][tempi6].IsHave = true;

                                    for (int j1 = 0; j1 < Hot.BodyDicMapObject[MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj].Y; j1++)
                                    {
                                        for (int j2 = 0; j2 < Hot.BodyDicMapObject[MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj].X; j2++)
                                        {
                                            PanelCellMiniMapEditor_.Map[tempi5 + j1][tempi6 + j2].e_Obj = MapData.ListCellMiniMap[tempi1][tempi2].Map[tempi5][tempi6].Obj.e_Obj;
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }
    }

    public void Load(string fileName)
    {
        GenerateGridByLoadData(Hot.MgrJson_.Load<DataContainer_Expedition>(PathFolder, "/" + fileName,
        (data) =>
        {
            IptFileName.text = IptLoad.text;
            IptLoad.text = "";
            IptWidth.text = data.ListCellMiniMap[0].Count.ToString();
            IptHeight.text = data.ListCellMiniMap.Count.ToString();
        }));
    }

    public void ClearGridsImgStatus()
    {
        foreach (List<PanelCellGridMiniMapEditor> list in Grids)
        {
            foreach (PanelCellGridMiniMapEditor item in list)
            {
                item.ImgStatus.sprite = Hot.MgrRes_.Load<Sprite>("Art/" + "ImgEmpty");
            }
        }
    }

    public void ClearMap()
    {
        foreach (List<PanelCellGridMiniMapEditor> list in Grids)
        {
            foreach (PanelCellGridMiniMapEditor item in list)
            {
                Destroy(item.ImgBk.gameObject);
                Destroy(item.ImgStatus.gameObject);
                Destroy(item.gameObject);                
            }
        }

        foreach (ContentStep item in ItemContent.GetComponentsInChildren<ContentStep>())
        {
            Destroy(item.gameObject);
        }

        Grids.Clear();
        ItemRoot.Clear();
    }

    public void Save()
    {
        DataContainer_Expedition MapData = new();

        MapData.EntrancePos = EntrancePos;

        for (int i1 = 0; i1 < ItemRoot.Count; i1++)
        {
            MapData.ListCellMiniMap.Add(new());

            for (int i2 = 0; i2 < ItemRoot[i1].Count; i2++)
            {
                if (ItemRoot[i1][i2].childCount == 0)
                {
                    MapData.ListCellMiniMap[i1].Add(new());
                }
                else
                {
                    MapData.ListCellMiniMap[i1].
                        Add(new(ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().e_Hall, 
                                ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().e_Room, new()));

                    //保存Map 
                    for (int i3 = 0; i3 < ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().Map.Count; i3++)
                    {
                        MapData.ListCellMiniMap[i1][i2].Map.Add(new());

                        for (int i4 = 0; i4 < ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().Map[i3].Count; i4++)
                        {
                            MapData.ListCellMiniMap[i1][i2].Map[i3].Add(new());

                            if (ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().Map[i3][i4].IsHave)
                            {
                                MapData.ListCellMiniMap[i1][i2].Map[i3][i4] = new()
                                {
                                    Obj = new()
                                    {
                                        e_Obj = ItemRoot[i1][i2].GetComponentInChildren<PanelCellMiniMapEditor>().Map[i3][i4].e_Obj,
                                    }
                                };
                            }
                        }
                    }
                }
            }
        }

        Hot.MgrJson_.Save(MapData, PathFolder, "/" + IptFileName.text);
    }    

    public void ChangeSize()
    {
        ImgBkContent.GetComponent<GridLayoutGroup>().cellSize = new(Hot.BodySizeCellItem.X, Hot.BodySizeCellItem.Y);
        ItemContent.GetComponent<GridLayoutGroup>().cellSize = new(Hot.BodySizeCellItem.X, Hot.BodySizeCellItem.Y);
        ImgStatusContent.GetComponent<GridLayoutGroup>().cellSize = new(Hot.BodySizeCellItem.X, Hot.BodySizeCellItem.Y);

        foreach (GridLayoutGroup item in ItemContent.GetComponentsInChildren<GridLayoutGroup>())
        {
            item.cellSize = new(Hot.BodySizeCellItem.X, Hot.BodySizeCellItem.Y);
        }

        foreach (List<Transform> listItem in ItemRoot)
        {
            foreach (Transform item in listItem)
            {
                if (item.childCount > 0)
                {
                    item.GetComponentInChildren<PanelCellItem>().ChangeSize();
                }
            }
        }
    }
}