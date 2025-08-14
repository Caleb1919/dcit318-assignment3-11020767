using System;
using System.Collections.Generic;
using System.IO;

// ------------------------
// 1. Marker Interface
// ------------------------
public interface IInventoryEntity
{
    int Id { get; }
}

// ------------------------
// 2. Immutable Inventory Record
// ------------------------
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// ------------------------
// 3. Generic Inventory Logger (Plain Text)
// ------------------------
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            using StreamWriter writer = new(_filePath);
            foreach (var item in _log)
            {
                if (item is InventoryItem inv)
                {
                    writer.WriteLine($"{inv.Id},{inv.Name},{inv.Quantity},{inv.DateAdded:O}");
                }
            }
            Console.WriteLine($"Data saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File not found: {_filePath}");
                return;
            }

            _log.Clear();

            using StreamReader reader = new(_filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length != 4) continue;

                if (int.TryParse(parts[0], out int id) &&
                    int.TryParse(parts[2], out int qty) &&
                    DateTime.TryParse(parts[3], out DateTime date))
                {
                    _log.Add((T)(IInventoryEntity)new InventoryItem(id, parts[1], qty, date));
                }
            }

            Console.WriteLine($"Data loaded from {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
        }
    }
}

// ------------------------
// 4. Inventory App
// ------------------------
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Smartphone", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Headphones", 50, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Keyboard", 30, DateTime.Now));
        Console.WriteLine("Sample data seeded.");
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items to display.");
            return;
        }

        Console.WriteLine("=== Inventory Items ===");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

// ------------------------
// 5. Main Program
// ------------------------
public static class Program
{
    public static void Main()
    {
        string filePath = "inventory.txt";

        // Start app
        var app = new InventoryApp(filePath);

        // Seed and save data
        app.SeedSampleData();
        app.SaveData();

        // Clear memory (simulate new session)
        Console.WriteLine("\n--- Simulating new session ---\n");

        var newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
