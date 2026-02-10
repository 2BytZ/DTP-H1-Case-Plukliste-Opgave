using FKTV_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FKTV_BLL
{
    public class ItemsRepository
    {
        public StoreItem GetItem(string id)
        {
            var storeItems = GetItems();
            var item = storeItems.FirstOrDefault(i => i.ProductID == id);
            if (item == null)
                throw new KeyNotFoundException($"Item with ProductID '{id}' not found.");
            return item;
        }

        public List<StoreItem> GetItems()
        {
            var dataAccess = new DataAccess();
            var json = dataAccess.GetData();
            var deserialized = JsonSerializer.Deserialize<List<StoreItem>>(json);
            return deserialized ?? new List<StoreItem>();
        }

        public void UpdateAmount(string productID, int amount, bool isCalledFromPlukliste = false)
        {
            var storeItems = GetItems();
            var storeItem = storeItems.FirstOrDefault(i => i.ProductID == productID);
            if (storeItem == null)
                throw new KeyNotFoundException($"Item with ProductID '{productID}' not found.");

            if (isCalledFromPlukliste)
            {
                storeItem.Amount -= amount;
                if (storeItem.Amount < 0) storeItem.Amount = 0;
            }
            else
            {
                storeItem.Amount = amount;
            }

            var json = JsonSerializer.Serialize(storeItems);
            var dataAccess = new DataAccess();
            dataAccess.UpdateAmount(json);

            // If update originates from the web app (not from Plukliste), propagate to Plukliste and FKTV_DAL
            if (!isCalledFromPlukliste)
            {
                try
                {
                    dataAccess.SyncData(false);
                }
                catch (Exception ex)
                {
                    // Log to console for now; replace with ILogger in web app if desired.
                    Console.Error.WriteLine($"ItemsRepository.UpdateAmount: SyncData failed: {ex.Message}");
                }
            }
        }

        public int GetStorageAmount(string productID)
        {
            var storeItems = GetItems();
            var item = storeItems.FirstOrDefault(i => i.ProductID == productID);
            return item?.Amount ?? 0;
        }
    }
}
