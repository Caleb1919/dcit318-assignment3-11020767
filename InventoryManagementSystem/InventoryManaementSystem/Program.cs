#nullable enable
using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    // a) Marker interface for inventory items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem implements IInventoryItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
            if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required.", nameof(brand));
            if (warrantyMonths < 0) throw new ArgumentOutOfRangeException(nameof(warrantyMonths), "Warranty months cannot be negative.");

            Id = id;
            Name = name.Trim();
            Quantity = quantity;
            Brand = brand.Trim();
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"#{Id} | {Name} | Qty: {Quantity} | Brand: {Brand} | Warranty: {WarrantyMonths} mo";
    }

    // c) GroceryItem implements IInventoryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");

            Id = id;
            Name = name.Trim();
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"#{Id} | {Name} | Qty: {Quantity} | Exp: {ExpiryDate:d}";
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d) Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id); // may throw ItemNotFoundException
            item.Quantity = newQuantity;
        }
    }

    // f) WareHouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            // Electronics
            TryAdd(() => _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24)));
            TryAdd(() => _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12)));
            TryAdd(() => _electronics.AddItem(new ElectronicItem(3, "Bluetooth Speaker", 15, "JBL", 18)));

            // Groceries
            TryAdd(() => _groceries.AddItem(new GroceryItem(1, "Rice Bag 25kg", 50, DateTime.Today.AddMonths(6))));
            TryAdd(() => _groceries.AddItem(new GroceryItem(2, "Milk 1L", 30, DateTime.Today.AddDays(10))));
            TryAdd(() => _groceries.AddItem(new GroceryItem(3, "Eggs (Tray)", 40, DateTime.Today.AddDays(20))));
        }

        private static void TryAdd(Action action)
        {
            try { action(); }
            catch (DuplicateItemException ex) { Console.WriteLine($"[Seed Warning] {ex.Message}"); }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var items = repo.GetAllItems();
            if (items.Count == 0)
            {
                Console.WriteLine("  (no items)");
                return;
            }

            foreach (var item in items)
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                if (quantity < 0) throw new InvalidQuantityException("Increase quantity cannot be negative.");
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"[OK] Increased '{item.Name}' (ID {id}) by {quantity}. New Qty: {item.Quantity}");
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"[OK] Removed item with ID {id}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    // Main application flow
    public static class Program
    {
        public static void Main(string[] args)
        {
            var manager = new WareHouseManager();

            // i–ii. Instantiate + seed
            manager.SeedData();

            // iii. Print all grocery items
            Console.WriteLine("=== Grocery Items ===");
            manager.PrintAllItems(manager.GroceriesRepo);

            // iv. Print all electronic items
            Console.WriteLine("\n=== Electronic Items ===");
            manager.PrintAllItems(manager.ElectronicsRepo);

            // v. Try the exception scenarios
            Console.WriteLine("\n=== Exception Scenarios ===");

            // Add a duplicate item (duplicate ID: 1 in electronics)
            try
            {
                manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Duplicate Error] {ex.Message}");
            }

            // Remove a non-existent item (id 999 in groceries)
            try
            {
                manager.GroceriesRepo.RemoveItem(999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[Remove Error] {ex.Message}");
            }

            // Update with invalid quantity (negative)
            try
            {
                manager.GroceriesRepo.UpdateQuantity(1, -10);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[Quantity Error] {ex.Message}");
            }

            // Bonus: show safe modifying methods with internal try-catch
            Console.WriteLine("\n=== Safe Operations via Manager ===");
            manager.IncreaseStock(manager.GroceriesRepo, id: 1, quantity: 5);     // OK
            manager.IncreaseStock(manager.GroceriesRepo, id: 404, quantity: 5);   // Not found
            manager.RemoveItemById(manager.ElectronicsRepo, id: 2);               // OK
            manager.RemoveItemById(manager.ElectronicsRepo, id: 2);               // Already removed -> error
        }
    }
}
