using System;

public class Rectangle
{
    private double length;
    private double width;

    public Rectangle()
    {
        length = 0;
        width = 0;
    }

    public Rectangle(double side)
    {
        length = side;
        width = side;
    }

    public Rectangle(double length, double width)
    {
        this.length = length;
        this.width = width;
    }

    public Rectangle(Rectangle other)
    {
        length = other.length;
        width = other.width;
    }

    public double CalculateArea()
    {
        return length * width;
    }

    public void InputDimensions()
    {
        Console.Write("Введите длину прямоугольника: ");
        while (!double.TryParse(Console.ReadLine(), out length) || length <= 0)
        {
            Console.Write("Ошибка! Введите положительное число для длины: ");
        }

        Console.Write("Введите ширину прямоугольника: ");
        while (!double.TryParse(Console.ReadLine(), out width) || width <= 0)
        {
            Console.Write("Ошибка! Введите положительное число для ширины: ");
        }
    }

    // Деструктор (финализатор)
    ~Rectangle()
    {
        Console.WriteLine($"Прямоугольник с длиной {length} и шириной {width} уничтожен.");
    }

    // Свойства для доступа к полям
    public double Length
    {
        get { return length; }
        set { length = value > 0 ? value : 0; }
    }

    public double Width
    {
        get { return width; }
        set { width = value > 0 ? value : 0; }
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Вычисление площади прямоугольника ===");

        // Создаём прямоугольник и запрашиваем данные у пользователя
        Rectangle userRectangle = new Rectangle();
        userRectangle.InputDimensions();

        // Вычисляем и выводим площадь
        double area = userRectangle.CalculateArea();
        Console.WriteLine($"Площадь прямоугольника: {area}");

        // Деструктор сработает при завершении программы или сборке мусора
        // (Для демонстрации можно принудительно вызвать GC)
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}