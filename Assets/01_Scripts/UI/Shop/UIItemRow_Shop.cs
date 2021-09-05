using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OutGameCore;
using SH.Data;
using SH.UI.EnhancedHelper;
using UnityEngine;

namespace SH.Game.Shop
{
    public class UIItemRow_Shop : MonoBehaviour
    {
        private Dictionary<string, UIItem_Shop> shopItems = new Dictionary<string, UIItem_Shop>();
        public UIItem_Shop GetItem(string key) => shopItems.ContainsKey(key) ? shopItems[key] : null;
        public UIItem_Shop GetItem(int index) => shopItems.ContainsIndex(index) ? shopItems.ElementAt(index).Value : null;

        public void SetView(EnhancedScrollerCellRow<ShopBaseData> cellData, int rowCount = 3)
        {
            DespawnItems();

            int itemCount = Mathf.Min(cellData.Count, rowCount);
            for (int i = 0; i < itemCount; i++)
            {
                var data = cellData[i];

                UIItem_Shop item = null;
                if (data == null) continue;
                if (string.IsNullOrEmpty(data.PrefabName) == false)
                {
                    item = SpawnItem(data.PrefabName);
                    shopItems.Add(data.Key, item);

                    if (item != null)
                    {
                        item.SetView(data);
                        item.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void DespawnItems()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (OutGame.Instance.gamePools.IsSpawned(transform.GetChild(i)))
                    OutGame.Instance.gamePools.Despawn(transform.GetChild(i));
                else
                    Destroy(transform.GetChild(i).gameObject);
            }

            shopItems.Clear();
        }

        private UIItem_Shop SpawnItem(string prefab)
        {
            UIItem_Shop item = OutGame.Instance.gamePools.Spawn<UIItem_Shop>(prefab, true, transform);
            return item;
        }
    }
}