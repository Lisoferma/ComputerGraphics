using System.Drawing;

namespace DrawingLines;

public static class Line
{
    /// <summary>
    /// Нарисовать линию на заданном изобрежение по алгоритму DDA.
    /// </summary>
    /// <param name="startX">Начальная координата X.</param>
    /// <param name="startY">Начальная координата Y.</param>
    /// <param name="endX">Конечная координата X.</param>
    /// <param name="endY">Конечная координата Y.</param>
    /// <param name="image">Изображение на котором рисовать линию.</param>
    /// <param name="color">Цвет линии.</param>
    public static void DrawLineDDA(int startX, int startY, int endX, int endY, Color[,] image, Color color)
    {
        int dx = endX - startX;
        int dy = endY - startY;

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
        float x = startX;
        float y = startY;

        for (int i = 0; i <= steps; ++i)
        {
            image[(int)Math.Floor(y), (int)Math.Floor(x)] = color;
            x += addX;
            y += addY;
        }
    }


    public static void DrawLineBresenham(int x1, int y1, int x2, int y2, Color[,] image, Color color)
    {
        int x, y;
        int step = 1;
        int d, d1, d2;

        int dxabs = Math.Abs(x2 - x1);
        int dyabs = Math.Abs(y2 - y1);

        if ((dxabs > dyabs && x2 < x1) || (dxabs <= dyabs && y2 < y1))
        {
            x = x1;
            x1 = x2;
            x2 = x;

            y = y1;
            y1 = y2;
            y2 = y;
        }

        image[y1, x1] = color;

        int dx = x2 - x1;
        int dy = y2 - y1;
        dxabs = Math.Abs(dx);
        dyabs = Math.Abs(dy);

        if (dxabs > dyabs)
        {
            if (dy < 0)
            {
                step = -1;
                dy = -dy;
            }

            d = (dy * 2) - dx;
            d1 = dy * 2;
            d2 = (dy - dx) * 2;
            y = y1;

            for (x = x1 + 1; x < x2; ++x)
            {
                if (d > 0)
                {
                    y += step;
                    d += d2;
                }
                else
                {
                    d += d1;
                }

                image[y1, x1] = color;
            }
        }
        else
        {
            if (dx < 0)
            {
                step = -1;
                dx = -dx;
            }

            d = (dx * 2) - dy;
            d1 = dx * 2;
            d2 = (dx - dy) * 2;
            x = x1;

            for (y = y1 + 1; y < y2; ++y)
            {
                if (d > 0)
                {
                    x += step;
                    d += d2;
                }
                else
                {
                    d += d1;
                }

                image[y1, x1] = color;
            }
        }
    }
}
