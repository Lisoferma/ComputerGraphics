using System.Drawing;

namespace EllipseRotation;


/// <summary>
/// Содержит метод для рисования эллипсов.
/// </summary>
public static class Ellipse
{
    /// <summary>
    /// Нарисовать эллипс на заданом изображении используя алгоритм Брезенхема.
    /// </summary>
    /// <param name="angle">Угол на который повёрнут эллипс.</param>
    /// <param name="image">Изображение на котором нарисовать эллипс.</param>
    /// <param name="color">Цвет эллипса.</param>
    public static void Draw(int x0, int y0, int x1, int y1, double angle, Color[,] image, Color color)
    {
        // Значения диаметра
        int a = Math.Abs(x1 - x0);
        int b = Math.Abs(y1 - y0);
        int b1 = b & 1;

        // Увеличение ошибки
        long dx = 4 * (1 - a) * b * b;
        long dy = 4 * (b1 + 1) * a * a;

        // Ошибка на 1 шаге
        long err = dx + dy + b1 * a * a;
        long e2;

        // Центр эллипса
        int cx = x0 + a / 2;
        int cy = y0 + b / 2;

        image[cy, cx] = color;

        if (x0 > x1)
        {
            x0 = x1;
            x1 += a;
        }

        if (y0 > y1)
        {
            y0 = y1;
        }

        // Начальный пиксель
        y0 += (b + 1) / 2;
        y1 = y0 - b1;     
        a *= 8 * a;
        b1 = 8 * b * b;

        do
        {
            // Координаты пикселя после поворота
            int xr, yr;

            (xr, yr) = Rotate(x1, y0, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x0, y0, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x0, y1, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x1, y1, cx, cy, angle);
            image[yr, xr] = color;

            e2 = 2 * err;

            // Шаг y
            if (e2 <= dy)
            {
                y0++;
                y1--;
                err += dy += a;
            }

            // Шаг x
            if (e2 >= dx || 2 * err > dy)
            {
                x0++;
                x1--;
                err += dx += b1;
            }
        }
        while (x0 <= x1);

        // Ранняя остановка плоских эллипсов a = 1
        // -> завершающий элемент эллипса
        while (y0 - y1 < b)
        {
            // Координаты пикселя после поворота
            int xr, yr;

            (xr, yr) = Rotate(x0 - 1, y0, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x1 + 1, y0++, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x0 - 1, y1, cx, cy, angle);
            image[yr, xr] = color;

            (xr, yr) = Rotate(x1 + 1, y1--, cx, cy, angle);
            image[yr, xr] = color;
        }
    }


    public static (int newX, int newY) Rotate(int x, int y, int cx, int cy, double angle)
    {
        double radians = angle * Math.PI / 180.0;
        double cos = Math.Cos(radians);
        double sin = Math.Sin(radians);
        int newX = (int)Math.Floor((cos * (x - cx)) + (sin * (y - cy)) + cx);
        int newY = (int)Math.Floor((cos * (y - cy)) - (sin * (x - cx)) + cy);

        return (newX, newY);
    }
}
