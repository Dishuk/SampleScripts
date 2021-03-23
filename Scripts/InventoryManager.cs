using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager: MonoBehaviour {

	public GoodStack goodStack;
	public static InventoryManager instance;

	#region "SINGLETON"
	void Awake(){
		if (instance != null) {
			Debug.LogWarning ("More than one Inventory manager instance found!");
			return;
		}

		instance = this;
	}
	#endregion

	public void AddGoodsToInventory(GoodStack goodStack,  Inventory inventoryToAddTo){
		int remainder = 0;
		bool isAvailableInInventory = CheckIfGoodsAvailableInInventory (goodStack, inventoryToAddTo);

		if (isAvailableInInventory == false) {
			GoodStack goodsToAddStack = new GoodStack ();
			goodsToAddStack.goodName = goodStack.goods.goodName;
			goodsToAddStack.goods = goodStack.goods;
			goodsToAddStack.amount = 0;
			inventoryToAddTo.inventory.Add (goodsToAddStack);
			inventoryToAddTo.CalculateAvailableCapacityForGoods ();
		} 
		for (int i = 0; i < inventoryToAddTo.inventory.Count; i++) {
			if (goodStack.goods == inventoryToAddTo.inventory [i].goods) {
				if (goodStack.amount <= inventoryToAddTo.inventory [i].availableCapacity) {
					GoodStack goodsToAddStack = new GoodStack ();
					goodsToAddStack.goodName = goodStack.goods.goodName;
					goodsToAddStack.goods = goodStack.goods;
					goodsToAddStack.amount = inventoryToAddTo.inventory [i].amount + goodStack.amount;
					inventoryToAddTo.inventory [i] = goodsToAddStack;
				} else if (goodStack.amount > inventoryToAddTo.inventory [i].availableCapacity) {
					for (int futureValue = inventoryToAddTo.currentCapacity + goodStack.amount; futureValue > inventoryToAddTo.cargoCapacity; futureValue--) {
						goodStack.amount--;
						remainder++;
					}
					GoodStack goodsToAddStack = new GoodStack ();
					goodsToAddStack.goodName = goodStack.goods.goodName;
					goodsToAddStack.goods = goodStack.goods;
					goodsToAddStack.amount = inventoryToAddTo.inventory [i].amount + goodStack.amount;
					inventoryToAddTo.inventory [i] = goodsToAddStack;
					goodStack.amount = remainder;
				}
			}
		}
		inventoryToAddTo.ValidateInventory ();
	}

	public bool CheckIfGoodsAvailableInInventory(GoodStack goodStack,  Inventory inventoryToAddTo){
		for (int i = 0; i < inventoryToAddTo.inventory.Count; i++) {
			if (inventoryToAddTo.inventory [i].goods == goodStack.goods) {
				return true;
			}
		}
		return false;
	}

	public void GoodsTransaction(GoodStack goodStack,  Inventory inventoryToAddTo, Inventory inventoryToRemoveFrom){
		RemoveGoodsFromInventory(goodStack,inventoryToRemoveFrom);
		AddGoodsToInventory (goodStack, inventoryToAddTo);
	}

	public void RemoveGoodsFromInventory(GoodStack goodStack,  Inventory inventoryToRemoveFrom){
		int remainder = 0;
		for (int i = 0; i < inventoryToRemoveFrom.inventory.Count; i++) {
			if (goodStack.goods == inventoryToRemoveFrom.inventory [i].goods) {
				if (inventoryToRemoveFrom.inventory [i].amount >= goodStack.amount) {
					GoodStack goodsToRemoveStack = new GoodStack ();
					goodsToRemoveStack.goodName = goodStack.goods.goodName;
					goodsToRemoveStack.goods = goodStack.goods;
					goodsToRemoveStack.amount = inventoryToRemoveFrom.inventory [i].amount - goodStack.amount;
					inventoryToRemoveFrom.inventory [i] = goodsToRemoveStack;
				} else if(inventoryToRemoveFrom.inventory[i].amount < goodStack.amount){
					for (int futureValue = inventoryToRemoveFrom.inventory [i].amount - goodStack.amount ; futureValue < 0; futureValue++) {
						goodStack.amount--;
						remainder++;
					}
					GoodStack goodsToRemoveStack = new GoodStack ();
					goodsToRemoveStack.goodName = goodStack.goods.goodName;
					goodsToRemoveStack.goods = goodStack.goods;
					goodsToRemoveStack.amount = inventoryToRemoveFrom.inventory [i].amount - goodStack.amount;
					inventoryToRemoveFrom.inventory [i] = goodsToRemoveStack;
					goodStack.amount = remainder;
				}
			}
		}
		inventoryToRemoveFrom.ValidateInventory();
	}
}