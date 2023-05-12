using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public class MemoryManager
    {
        private const int MaxSegmentSize = 1024; // Максимальный размер сегмента памяти

        private class MemoryBlock
        {
            public int Size { get; set; }
            public bool IsUsed { get; set; }
            public int Address { get; set; }
        }

        private readonly List<MemoryBlock> memoryBlocks;

        public MemoryManager()
        {
            memoryBlocks = new List<MemoryBlock>();
        }

        public void AllocateMemory(int size)
        {
            if (size <= 0 || size > MaxSegmentSize)
            {
                Console.WriteLine("Неудачное выделение памяти. Размер блока превышает максимально допустимый размер.");
                return;
            }

            int availableAddress = GetAvailableAddress();
            if (availableAddress == -1)
            {
                Console.WriteLine("Неудачное выделение памяти. Недостаточно свободного места.");
                return;
            }

            MemoryBlock block = new MemoryBlock
            {
                Size = size,
                IsUsed = true,
                Address = availableAddress
            };

            memoryBlocks.Add(block);
            Console.WriteLine($"Выделена память блоком размером {size} на адресе {availableAddress}.");

            LogMemoryUsage();
        }

        public void FreeMemory(int address)
        {
            MemoryBlock block = memoryBlocks.Find(b => b.Address == address && b.IsUsed);
            if (block != null)
            {
                block.IsUsed = false;
                Console.WriteLine($"Освобождена память блока размером {block.Size} на адресе {block.Address}.");

                LogMemoryUsage();

                OptimizeMemory();
            }
            else
            {
                Console.WriteLine($"Неудачное освобождение памяти. Блок на адресе {address} не найден или уже освобожден.");
            }
        }

        private int GetAvailableAddress()
        {
            int address = 0;
            foreach (MemoryBlock block in memoryBlocks)
            {
                if (!block.IsUsed)
                {
                    return block.Address;
                }
                address = block.Address + block.Size;
            }
            return (address < MaxSegmentSize) ? address : -1;
        }

        private void OptimizeMemory()
        {
            int startIndex = -1;
            for (int i = 0; i < memoryBlocks.Count; i++)
            {
                if (!memoryBlocks[i].IsUsed && startIndex == -1)
                {
                    startIndex = i;
                }
                else if (memoryBlocks[i].IsUsed && startIndex != -1)
                {
                    int mergedSize = 0;
                    for (int j = startIndex; j < i; j++)
                    {
                        mergedSize += memoryBlocks[j].Size;
                    }

                    memoryBlocks[startIndex].Size = mergedSize;
                    memoryBlocks[startIndex].IsUsed = false;

                    for (int j = startIndex + 1; j < i; j++)
                    {
                        memoryBlocks.RemoveAt(startIndex + 1);
                    }

                    startIndex = -1;
                }
            }

            if (startIndex != -1)
            {
                int mergedSize = 0;
                for (int i = startIndex; i < memoryBlocks.Count; i++)
                {
                    mergedSize += memoryBlocks[i].Size;
                }

                memoryBlocks[startIndex].Size = mergedSize;
                memoryBlocks[startIndex].IsUsed = false;

                for (int i = startIndex + 1; i < memoryBlocks.Count; i++)
                {
                    memoryBlocks.RemoveAt(startIndex + 1);
                }
            }
        }

        public void LogMemoryUsage()
        {
            int totalReservedMemory = 0;
            foreach (MemoryBlock block in memoryBlocks)
            {
                totalReservedMemory += block.Size;
                Console.WriteLine($"Блок памяти размером {block.Size} на адресе {block.Address}. Статус: {(block.IsUsed ? "используется" : "не используется")}.");
            }

        Console.WriteLine($"Общий размер памяти, зарезервированный транслятором: {totalReservedMemory}.");
        }
}
internal class Program
{

    static void Main(string[] args)
    {
        MemoryManager memoryManager = new MemoryManager();

        // Запросить пять блоков памяти разного размера
        memoryManager.AllocateMemory(100);
        memoryManager.AllocateMemory(200);
        memoryManager.AllocateMemory(150);
        memoryManager.AllocateMemory(300);
        memoryManager.AllocateMemory(250);

        // Освободить память для 2 и 4 блока
        memoryManager.FreeMemory(200);
        memoryManager.FreeMemory(800);

        // Освободить память для 3 блока
        memoryManager.FreeMemory(300);

        // Запросить память чуть большего размера, чем можно выделить для одного сегмента
        memoryManager.AllocateMemory(1200);

        // Запросить три раза памяти размером в половину из предыдущего пункта
        memoryManager.AllocateMemory(600);
        memoryManager.AllocateMemory(600);
        memoryManager.AllocateMemory(600);

        // Вывести статистику использования памяти
        memoryManager.LogMemoryUsage();

        Console.ReadLine();
    }
}
}