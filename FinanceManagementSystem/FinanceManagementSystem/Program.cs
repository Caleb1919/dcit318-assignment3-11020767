using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FinanceManagementSystem
{
    // a) Define core models using records
    public record Transaction
    {
        public int Id { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Category { get; }

        public Transaction(int id, DateTime date, decimal amount, string category)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.", nameof(category));

            Id = id;
            Date = date;
            Amount = amount;
            Category = category.Trim();
        }
    }

    // b) Interface for transaction processing
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c) Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processing GHS {transaction.Amount:N2} for '{transaction.Category}' (Txn #{transaction.Id}) on {transaction.Date:d}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Debiting wallet GHS {transaction.Amount:N2} for '{transaction.Category}'. Ref: MM-{transaction.Id:0000}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto] Broadcasting on-chain payment for '{transaction.Category}': amount ≈ GHS {transaction.Amount:N2}. TxnId: CW-{transaction.Id:0000}.");
        }
    }

    // d) Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
            if (initialBalance < 0m)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");

            AccountNumber = accountNumber.Trim();
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account {AccountNumber}] Applied GHS {transaction.Amount:N2}. New balance: GHS {Balance:N2}");
        }
    }

    // e) Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"[Savings {AccountNumber}] Insufficient funds. Balance: GHS {Balance:N2}, Attempted: GHS {transaction.Amount:N2}");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[Savings {AccountNumber}] Deducted GHS {transaction.Amount:N2} for '{transaction.Category}'. Updated balance: GHS {Balance:N2}");
        }
    }

    // f) Finance application
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System Demo ===\n");

            var account = new SavingsAccount(accountNumber: "SA-001234567", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount {account.AccountNumber} with starting balance GHS {account.Balance:N2}\n");

            var t1 = new Transaction(id: 1, date: DateTime.Now, amount: 150.00m, category: "Groceries");
            var t2 = new Transaction(id: 2, date: DateTime.Now, amount: 300.00m, category: "Utilities");
            var t3 = new Transaction(id: 3, date: DateTime.Now, amount: 700.00m, category: "Entertainment");

            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);
            Console.WriteLine();

            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);
            Console.WriteLine();

            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("=== Transaction Summary ===");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"#{tx.Id} | {tx.Date:g} | GHS {tx.Amount:N2} | {tx.Category}");
            }

            Console.WriteLine($"\nFinal Balance for {account.AccountNumber}: GHS {account.Balance:N2}");
            Console.WriteLine("\n=== End ===");
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            // Ensure UTF-8 support for console output
            Console.OutputEncoding = Encoding.UTF8;

            var app = new FinanceApp();
            app.Run();
        }
    }
}
