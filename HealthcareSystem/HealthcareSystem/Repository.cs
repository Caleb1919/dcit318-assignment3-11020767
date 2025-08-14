#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    public class Repository<T> where T : class
    {
        private readonly List<T> items = new();

        public void Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }

        public List<T> GetAll() => new(items);

        public T? GetById(Func<T, bool> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            var toRemove = items.FirstOrDefault(predicate);
            if (toRemove is null) return false;
            return items.Remove(toRemove);
        }
    }
}
