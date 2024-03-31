using System.Drawing;

namespace DrawingLines;

public static class Line
{
    public static void CreateLineDDA(int startX, int startY, int endX, int endY, Color[,] image, Color color)
    {
        int dx = endX - startX;
        int dy = endY - startY;

        int step;

        if (Math.Abs(dx) > Math.Abs(dy))
            step = Math.Abs(dx);
        else
            step = Math.Abs(dy);

        float addX = (float)dx / step;
        float addY = (float)dy / step;

        float x = startX;
        float y = startY;

        for (int i = 0; i <= step; ++i)
        {
            image[(int)Math.Floor(y), (int)Math.Floor(x)] = color;
            x += addX;
            y += addY;
        }
    }
}
