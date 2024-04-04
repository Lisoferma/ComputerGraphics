using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using FoxCanvas;

namespace DrawingLines;

internal class Window : GameWindow
{
    private const string TITLE = "Drawing Lines";
    private const int CANVAS_WIDTH = 64;
    private const int CANVAS_HEIGHT = 64;

    private readonly Color COLOR_BG_1 = Color.FromArgb(31, 37, 47);
    private readonly Color COLOR_BG_2 = Color.FromArgb(40, 52, 62);
    private readonly Color COLOR_DDA = Color.FromArgb(0, 200, 220);
    private readonly Color COLOR_BRESENHAM = Color.FromArgb(230, 127, 0);
    private readonly Color COLOR_CIRCLE = Color.FromArgb(5, 220, 0);

    private double _frameTime = 0.0f;
    private int _fps = 0;

    private Canvas _canvas;
    private Color[,] _image1;
    private Color[,] _image2;

    private int StartX = -1;
    private int StartY = -1;
    private int EndX = -1;
    private int EndY = -1;

    private int StartCircleX = -1;
    private int StartCircleY = -1;
    private int EndCircleX = -1;
    private int EndCircleY = -1;

    private bool IsDDA = true;


    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    { }


    protected override void OnLoad()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _image1 = new Color[CANVAS_WIDTH, CANVAS_HEIGHT];
        CreateGridImage(_image1, COLOR_BG_1, COLOR_BG_2);

        _image2 = new Color[CANVAS_WIDTH, CANVAS_HEIGHT];
        CreateGridImage(_image2, COLOR_BG_1, COLOR_BG_2);

        _canvas = new Canvas(CANVAS_WIDTH, CANVAS_HEIGHT, ClientSize.X, ClientSize.Y);
        _canvas.SetImage(_image1);

        base.OnLoad();
    }


    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _canvas.Render();
        SwapBuffers();

        base.OnRenderFrame(e);
    }


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        UpdateTitle(e.Time);
        base.OnUpdateFrame(e);
    }


    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        _canvas.SetViewport(e.Width, e.Height);
            
        base.OnFramebufferResize(e);       
    }


    protected override void OnUnload()
    {
        _canvas.Dispose();
        base.OnUnload();
    }


    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        if (KeyboardState.IsKeyPressed(Keys.Escape))
            Close();

        if (KeyboardState.IsKeyPressed(Keys.Backspace))
            ClearCanvas();

        if (KeyboardState.IsKeyPressed(Keys.Space))
            SwitchDisplayedAlgorithm();

        base.OnKeyDown(e);
    }


    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
            SetLineCoord();

        if (e.Button == MouseButton.Right)
            SetCircleCoord();

        base.OnMouseDown(e);
    }


    private void SetLineCoord()
    {
        (int canvasX, int canvasY) = _canvas.GetCoord(MouseState.X, MouseState.Y);

        if (SetBlankLineCoord(canvasX, canvasY))
        {
            Drawing.DrawLineDDA(StartX, StartY, EndX, EndY, _image1, COLOR_DDA);
            Drawing.DrawLineBresenham(StartX, StartY, EndX, EndY, _image2, COLOR_BRESENHAM);

            if (IsDDA)
                _canvas.SetImage(_image1);
            else
                _canvas.SetImage(_image2);

            ClearLineCoord();
        }
    }


    private bool SetBlankLineCoord(int x, int y)
    {
        if (StartX == -1)
        {
            StartX = x;
            StartY = y;
            return false;
        }
        else
        {
            EndX = x;
            EndY = y;
            return true;
        }
    }


    private void ClearLineCoord()
    {
        StartX = -1;
        StartY = -1;
        EndX = -1;
        EndY = -1;
    }


    private void SetCircleCoord()
    {
        (int canvasX, int canvasY) = _canvas.GetCoord(MouseState.X, MouseState.Y);

        if (SetBlankCircleCoord(canvasX, canvasY))
        {
            int radius = Distance(StartCircleX, StartCircleY, EndCircleX, EndCircleY);

            Drawing.DrawCircleBresenham(StartCircleX, StartCircleY, radius, _image1, COLOR_CIRCLE);
            Drawing.DrawCircleBresenham(StartCircleX, StartCircleY, radius, _image2, COLOR_CIRCLE);

            if (IsDDA)
                _canvas.SetImage(_image1);
            else
                _canvas.SetImage(_image2);

            ClearCircleCoord();
        }
    }


    private bool SetBlankCircleCoord(int x, int y)
    {
        if (StartCircleX == -1)
        {
            StartCircleX = x;
            StartCircleY = y;
            return false;
        }
        else
        {
            EndCircleX = x;
            EndCircleY = y;
            return true;
        }
    }


    private void ClearCircleCoord()
    {
        StartCircleX = -1;
        StartCircleY = -1;
        EndCircleX = -1;
        EndCircleY = -1;
    }


    private void SwitchDisplayedAlgorithm()
    {
        if (IsDDA)
        {
            _canvas.SetImage(_image2);
            IsDDA = false;
        }
        else
        {
            _canvas.SetImage(_image1);
            IsDDA = true;
        }
    }


    private void ClearCanvas()
    {
        CreateGridImage(_image1, COLOR_BG_1, COLOR_BG_2);
        CreateGridImage(_image2, COLOR_BG_1, COLOR_BG_2);
        _canvas.SetImage(_image1);
    }


    private void UpdateTitle(double time)
    {
        _frameTime += time;
        _fps++;

        if (_frameTime >= 1.0f)
        {
            if (IsDDA)
                Title = $"{TITLE} | DDA | {_fps} fps";
            else
                Title = $"{TITLE} | Bresenham | {_fps} fps";

            _frameTime = 0.0f;
            _fps = 0;
        }
    }


    private static void CreateGridImage(Color[,] image, Color color1, Color color2)
    {
        int rows = image.GetLength(0);
        int cols = image.GetLength(1);

        bool setColor = false;

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                if (setColor)
                    image[i, j] = color1;
                else
                    image[i, j] = color2;

                setColor = setColor == false;
            }

            setColor = setColor == false;
        }
    }


    public static int Distance(int x0, int y0, int x1, int y1)
    {
        int dx = x0 - x1;
        int dy = y0 - y1;

        return (int)Math.Sqrt((double)dx * dx + dy * dy);
    }
}
