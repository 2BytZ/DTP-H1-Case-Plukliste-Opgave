using FKTV_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;

namespace FKTV_BLL
{
    public class ItemsRepository
    {
        public StoreItem GetItem(string id)
        {
            List<StoreItem> storeItems = GetItems();
            return storeItems.FirstOrDefault(i => i.ProductID == id);
        }

        public List<StoreItem> GetItems()
        {
            DataAccess dataAccess = new DataAccess();
            string json = dataAccess.GetData();
            return JsonSerializer.Deserialize<List<StoreItem>>(json);
        }

        public void UpdateAmount(string productID, int amount, bool isCalledFromPlukliste = false)
        {
            List<StoreItem> storeItems = GetItems();
            StoreItem storeItem = storeItems.FirstOrDefault(i => i.ProductID == productID);
            if (isCalledFromPlukliste == true) 
            { 
                storeItem.Amount -= amount;
            }
            else
            {
                storeItem.Amount = amount;
            }
            string json = JsonSerializer.Serialize(storeItems);
            DataAccess dataAccess = new DataAccess();
            dataAccess.UpdateAmount(json);
        }

        public int GetStorageAmount(string productID)
        {
            List<StoreItem> storeItems = GetItems();
            StoreItem item = storeItems.FirstOrDefault(i => i.ProductID == productID);
            return item.Amount;
        }
    }
}
