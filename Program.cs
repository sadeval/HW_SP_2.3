using System;
using System.Threading;

class Account
{
    private object lockObj = new object();
    public int Balance { get; private set; }

    public Account(int initialBalance)
    {
        Balance = initialBalance;
    }

    public void Deposit(int amount)
    {
        lock (lockObj)
        {
            Balance += amount;
            Console.WriteLine($"Пополнение: +{amount} текущий баланс: {Balance}");
        }
    }

    public void Withdraw(int amount)
    {
        lock (lockObj)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Console.WriteLine($"Снятие: -{amount} текущий баланс: {Balance}");
            }
            else
            {
                Console.WriteLine("Недостаточно средств для снятия!");
            }
        }
    }
}

class Client
{
    private Account account;
    private Random random;
    private bool keepRunning = true; 

    public Client(Account account)
    {
        this.account = account;
        random = new Random();
    }

    public void PerformOperations()
    {
        while (keepRunning)
        {
            Thread.Sleep(random.Next(1000, 3000));

            int operation = random.Next(0, 2);
            int amount = random.Next(10, 101);

            if (operation == 0)
            {
                account.Deposit(amount);
            }
            else
            {
                account.Withdraw(amount);
            }
        }
    }

    public void Stop()
    {
        keepRunning = false;
    }
}

class Program
{
    static void Main()
    {
        Account[] accounts = new Account[3];
        for (int i = 0; i < accounts.Length; i++)
        {
            accounts[i] = new Account(1000);
        }

        Client[] clients = new Client[3];
        Thread[] clientThreads = new Thread[3];
        for (int i = 0; i < clientThreads.Length; i++)
        {
            clients[i] = new Client(accounts[i]);
            clientThreads[i] = new Thread(clients[i].PerformOperations);
            clientThreads[i].Start();
        }

        Thread.Sleep(30000);

        for (int i = 0; i < clients.Length; i++)
        {
            clients[i].Stop();
        }

        foreach (Thread thread in clientThreads)
        {
            thread.Join();
        }

        Console.WriteLine("\nПрограмма завершена. Текущие балансы:");
        for (int i = 0; i < accounts.Length; i++)
        {
            Console.WriteLine($"Счет клиента {i + 1}: {accounts[i].Balance}");
        }
    }
}
