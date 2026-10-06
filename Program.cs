using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // === 1. Дані програми (історія обчислень за сеанс) ===
        // Спочатку масив/список порожній і наповнюється під час роботи.
        List<string> history = new List<string>();

        // === 2. Головний цикл меню ===
        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine("\n=== Меню ===");
            Console.WriteLine("0. Вихід");
            Console.WriteLine("1. Обчислити вираз");
            Console.WriteLine("2. Показати історію обчислень");

            int choice = ReadInt("Ваш вибір: ", 0, 2);

            // switch expression повертає false для виходу (0) або true для продовження дій
            isRunning = choice switch
            {
                1 => HandleCalculate(history),
                2 => HandleShowHistory(history),
                0 => false,
                _ => true
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    // === 3. Методи для пунктів меню ===

    // Обробник вибору "1. Обчислити вираз"
    static bool HandleCalculate(List<string> history)
    {
        Console.Write("\nВведіть вираз (наприклад, 12 + 7): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Помилка: введено порожній рядок.");
            return true;
        }

        // Розбираємо рядок та робимо перевірку
        if (!TryParseExpression(input, out double left, out string op, out double right))
        {
            Console.WriteLine("Помилка: некоректний формат! Вводьте у форматі: число оператор число (наприклад, 12 + 7).");
            return true;
        }

        // Намагаємося обчислити результат
        if (TryExecuteOperation(left, op, right, out double result, out string errorMessage))
        {
            // Формуємо рядок з успішним результатом
            string record = $"{left} {op} {right} = {result}";

            // Одразу виводимо результатом окремим рядком
            Console.WriteLine($"Результат: {record}");

            // Додаємо в історію
            history.Add(record);
        }
        else
        {
            // Якщо була помилка (наприклад, ділення на нуль чи непідтримуваний оператор)
            Console.WriteLine($"Помилка обчислення: {errorMessage}");
        }

        return true;
    }

    // Обробник вибору "2. Показати історію обчислень"
    static bool HandleShowHistory(List<string> history)
    {
        Console.WriteLine("\n--- Історія обчислень ---");

        if (history.Count == 0)
        {
            Console.WriteLine("Історія порожня.");
            return true;
        }

        for (int i = 0; i < history.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {history[i]}");
        }

        return true;
    }

    // === 4. Робочі та допоміжні методи ===

    // Розбір введеного рядка на складові (перше число, оператор, друге число)
    static bool TryParseExpression(string input, out double left, out string op, out double right)
    {
        left = 0;
        right = 0;
        op = string.Empty;

        // Заміна коми на крапку для коректної підтримки десяткових дробів
        string formattedInput = input.Replace(',', '.');

        // Розбиваємо рядок за пробілами, видаляючи зайві порожні елементи
        string[] parts = formattedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Перевіряємо, що після розбиття вийшло рівно 3 частини
        if (parts.Length != 3)
        {
            return false;
        }

        // Перевіряємо чи є перша і третя частина дійсними числами
        bool isLeftValid = double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out left);
        bool isRightValid = double.TryParse(parts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out right);

        op = parts[1];

        return isLeftValid && isRightValid;
    }

    // Виконання математичної операції за оператором
    static bool TryExecuteOperation(double a, string op, double b, out double result, out string errorMessage)
    {
        result = 0;
        errorMessage = string.Empty;

        switch (op)
        {
            case "+":
                result = a + b;
                return true;

            case "-":
                result = a - b;
                return true;

            case "*":
                result = a * b;
                return true;

            case "/":
                // Граничний випадок: ділення на нуль
                if (b == 0)
                {
                    errorMessage = "Ділення на нуль неможливе!";
                    return false;
                }
                result = a / b;
                return true;

            default:
                // Граничний випадок: непідтримуваний оператор (наприклад %, ^)
                errorMessage = $"Оператор '{op}' не підтримується! Дозволені лише (+, -, *, /).";
                return false;
        }
    }

    // Безпечне зчитування цілого числа для меню
    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Некоректне значення. Введіть ціле число від {min} до {max}.");
        }
    }
}