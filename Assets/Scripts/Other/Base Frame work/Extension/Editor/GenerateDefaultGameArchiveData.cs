using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class GenerateDefaultGameArchiveData : MonoBehaviour
{                          
    private static int DefaultGameArchiveDataCount = 10;

    [MenuItem("Tools/Generate/DefaultGameArchiveData", false, 1)]
    private static void Generate()
    {
        MgrJson.GetInstance().filePath = Application.persistentDataPath + "/Data/JsonData";        
        Data.GetInstance().PathGameArchiveData = "/GameArchiveData";

        List<DataContainer_PanelCellGameArchive> GameArchiveDataCellList = new List<DataContainer_PanelCellGameArchive>();

        for (int i = 0; i < DefaultGameArchiveDataCount; i++)
        {
            GameArchiveDataCellList.Add(new DataContainer_PanelCellGameArchive());            
        }

        #region ��������

        GameArchiveDataCellList[0].DataListCellStore = new List<DataContainer_PanelCellTownStore>()
        {
            new DataContainer_PanelCellTownStore(E_SpriteNamePanelCellTownStore.StoreWood),
            new DataContainer_PanelCellTownStore(E_SpriteNamePanelCellTownStore.StoreIron),
            new DataContainer_PanelCellTownStore(E_SpriteNamePanelCellTownStore.StoreGold),
            new DataContainer_PanelCellTownStore(E_SpriteNamePanelCellTownStore.StoreWood),
        };

        GameArchiveDataCellList[0].DataListCellStore[0].DataListCellStoreItem = new List<DataContainer_PanelCellItem>()
        {
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookedBeef),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodRawChicken),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodApple),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodRawMutton),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
        };

        GameArchiveDataCellList[0].DataListCellStore[2].DataListCellStoreItem = new List<DataContainer_PanelCellItem>()
        {
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
        };

        GameArchiveDataCellList[0].DataStoreAncestralProperty = 
            new DataContainer_PanelStoreAncestralProperty(11, 22, 33, 44, 55);

        GameArchiveDataCellList[0].DataStoreCoin =
            new DataContainer_PanelStoreCoin(11, 22, 33, 44, E_StoreCoinLevel.Silver, 5);

        GameArchiveDataCellList[0].DataListCellShopItem = new List<DataContainer_PanelCellItem>()
        {
            new DataContainer_PanelCellItem(E_Location.PanelTownShopItem, E_SpriteNamePanelCellItem.ItemFoodCookie),
            new DataContainer_PanelCellItem(E_Location.PanelTownShopItem, E_SpriteNamePanelCellItem.ItemFoodCookedBeef),
            new DataContainer_PanelCellItem(E_Location.PanelTownShopItem, E_SpriteNamePanelCellItem.ItemFoodCookedPotato),
            new DataContainer_PanelCellItem(E_Location.PanelTownShopItem, E_SpriteNamePanelCellItem.ItemFoodCoodedChicken),
            new DataContainer_PanelCellItem(E_Location.PanelTownShopItem, E_SpriteNamePanelCellItem.ItemFoodCookie)
        };

        #endregion

        for (int i = 0; i < DefaultGameArchiveDataCount; i++)
        {            
            MgrJson.GetInstance().Save(GameArchiveDataCellList[i], Data.GetInstance().PathGameArchiveData + i);
        }

        DestroyImmediate(Data.GetInstance().gameObject);
        DestroyImmediate(MgrJson.GetInstance().gameObject);
    }
}
