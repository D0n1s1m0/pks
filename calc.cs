using System;

namespace Calculator
{
    class Program
    {
        static double memory = 0; // Область памяти для M+, M-, MR
        static double currentValue = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Классический калькулятор");
            Console.WriteLine("Доступные операции: +, -, *, /, %, 1/x, x^2, sqrt, M+, M-, MR");
            Console.WriteLine("Для выхода введите 'exit'");

            while (true)
            {
                Console.WriteLine($"\nТекущее значение: {currentValue}");
                Console.Write("Введите операцию: ");
                string operation = Console.ReadLine().Trim();

                if (operation.ToLower() == "exit")
                    break;

                switch (operation)
                {
                    case "+":
                        currentValue = PerformBinaryOperation(currentValue, true);
                        break;
                    case "-":
                        currentValue = PerformBinaryOperation(currentValue, false);
                        break;
                    case "*":
                        currentValue = PerformBinaryOperation(currentValue, true, isMultiplication: true);
                        break;
                    case "/":
                        currentValue = PerformDivision(currentValue);
                        break;
                    case "%":
                        currentValue = PerformModulo(currentValue);
                        break;
                    case "1/x":
                        currentValue = PerformReciprocal(currentValue);
                        break;
                    case "x^2":
                        currentValue = Math.Pow(currentValue, 2);
                        break;
                    case "sqrt":
                        currentValue = Math.Sqrt(currentValue);
                        break;
                    case "M+":
                        memory += currentValue;
                        Console.WriteLine($"Массив: {memory}");
                        break;
                    case "M-":
                        memory -= currentValue;
                        Console.WriteLine($"Массив: {memory}");
                        break;
                    case "MR":
                        currentValue = memory;
                        Console.WriteLine($"Массив: {memory}");
                        break;
                    default:
                        // Попытка преобразовать ввод в число
                        if (double.TryParse(operation, out double num))
                        {
                            currentValue = num;
                        }
                        else
                        {
                            Console.WriteLine("Неверная операция или команда");
                        }
                        break;
                }
            }
        }

        static double PerformBinaryOperation(double current, bool isAddition, bool isMultiplication = false)
        {
            Console.Write("Введите число: ");
            if (double.TryParse(Console.ReadLine(), out double operand))
            {
                if (isAddition)
                {
                    return current + operand;
                }
                else if (isMultiplication)
                {
                    return current * operand;
                }
                else
                {
                    return current - operand;
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод числа");
                return current;
            }
        }

        static double PerformDivision(double current)
        {
            Console.Write("Введите делитель: ");
            if (double.TryParse(Console.ReadLine(), out double divisor))
            {
                if (divisor == 0)
                {
                    Console.WriteLine("Ошибка: деление на ноль");
                    return current;
                }
                return current / divisor;
            }
            else
            {
                Console.WriteLine("Некорректный ввод");
                return current;
            }
        }

        static double PerformModulo(double current)
        {
            Console.Write("Введите число для операции %: ");
            if (double.TryParse(Console.ReadLine(), out double modValue))
            {
                return current % modValue;
            }
            else
            {
                Console.WriteLine("Некорректный ввод");
                return current;
            }
        }

        static double PerformReciprocal(double current)
        {
            if (current == 0)
            {
                Console.WriteLine("Ошибка: деление на ноль");
                return current;
            }
            return 1 / current;
        }
    }
}