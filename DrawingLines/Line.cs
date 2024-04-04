using System.Drawing;

namespace DrawingLines;

public static class Line
{
    /// <summary>
    /// Нарисовать линию на заданном изобрежение по алгоритму DDA.
    /// </summary>
    /// <param name="x0">Начальная координата X.</param>
    /// <param name="y0">Начальная координата Y.</param>
    /// <param name="x1">Конечная координата X.</param>
    /// <param name="y1">Конечная координата Y.</param>
    /// <param name="image">Изображение на котором рисовать линию.</param>
    /// <param name="color">Цвет линии.</param>
    public static void DrawLineDDA(int x0, int y0, int x1, int y1, Color[,] image, Color color)
    {
        int dx = x1 - x0;
        int dy = y1 - y0;

        // Количество шагов алгоритма для построения линии
        int steps;

        if (Math.Abs(dx) > Math.Abs(dy))
            steps = Math.Abs(dx);
        else
            steps = Math.Abs(dy);

        // Приращение к координатам на каждом шаге
        float addX = (float)dx / steps;
        float addY = (float)dy / steps;

        // Текущие координаты шага. Начинаем с startX, startY
        float x = x0;
        float y = y0;

        for (int i = 0; i <= steps; ++i)
        {
            image[(int)Math.Floor(y), (int)Math.Floor(x)] = color;
            x += addX;
            y += addY;
        }
    }


    /// <summary>
    /// Нарисовать линию на заданном изобрежение по алгоритму Брезенхема.
    /// </summary>
    /// <param name="x0">Начальная координата X.</param>
    /// <param name="y0">Начальная координата Y.</param>
    /// <param name="x1">Конечная координата X.</param>
    /// <param name="y1">Конечная координата Y.</param>
    /// <param name="image">Изображение на котором рисовать линию.</param>
    /// <param name="color">Цвет линии.</param>
    public static void DrawLineBresenham(int x0, int y0, int x1, int y1, Color[,] image, Color color)
    {
        // Проверяем рост отрезка по оси X и по оси Y
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        // Отражаем линию по диагонали, если угол наклона слишком большой
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }

        // Если линия растёт не слева направо, то меняем местами начало и конец отрезка
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int dx = x1 - x0;
        int dy = Math.Abs(y1 - y0);
        int error = dx / 2;

        // Выбираем направление роста координаты y
        int ystep = (y0 < y1) ? 1 : -1; 
        int y = y0;

        for (int x = x0; x <= x1; ++x)
        {
            // Не забываем вернуть координаты на место
            image[steep ? x : y, steep ? y : x] = color;  

            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }
    }


    public static void Swap<T>(ref T a, ref T b)
    {
        (b, a) = (a, b);
    }
}
