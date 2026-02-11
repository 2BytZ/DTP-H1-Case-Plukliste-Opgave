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
            //get item id in html database based on its productID
            var item = storeItems.FirstOrDefault(lagerItem => lagerItem.ProductID == id);
            if (item == null)
                throw new KeyNotFoundException($"Item with ProductID '{id}' not found.");
            return item;
        }

        public List<StoreItem> GetItems()
        {
            var dataAccess = new DataAccess();
            var json = dataAccess.GetData(); //get json data from DAL
            var deserialized = JsonSerializer.Deserialize<List<StoreItem>>(json); //deserialize data from json
            return deserialized ?? new List<StoreItem>(); //return the deserialized json data, if it is null return empty list of type StoreItem
        }

        public void UpdateAmount(string productID, int amount, bool isCalledFromPlukliste = false)
        {
            var storeItems = GetItems();
            var storeItem = storeItems.FirstOrDefault(lagerItem => lagerItem.ProductID == productID);
            if (storeItem == null)
                throw new KeyNotFoundException($"Item with ProductID '{productID}' not found.");

            if (isCalledFromPlukliste)
            {
                //decrease item amount in html database if it's called from the pluklist program
                storeItem.Amount -= amount;
                //set the amount to zero to prevent amount going into negatives
                //if (storeItem.Amount < 0) storeItem.Amount = 0;
            }
            else
            {
                //set item amount to amount defined by user, if called from Edit page in HTML database
                storeItem.Amount = amount;
            }
            //serialize items to json data
            var json = JsonSerializer.Serialize(storeItems);
            var dataAccess = new DataAccess();
            dataAccess.UpdateAmount(json); //update json file with new item data

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
            //get the amount of item in storage from html database
            var storeItems = GetItems();
            var item = storeItems.FirstOrDefault(i => i.ProductID == productID);
            return item?.Amount ?? 0; //return amount, unless it's null, then return zero
        }
    }
}
