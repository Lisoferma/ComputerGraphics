using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using FoxCanvas;
using System;

namespace EllipseRotation;

internal class Window : GameWindow
{
    private const string TITLE = "Ellipse Rotation";
    private const int CANVAS_WIDTH = 200;
    private const int CANVAS_HEIGHT = 200;

    private readonly Color COLOR_BG_1 = Color.FromArgb(31, 37, 47);
    private readonly Color COLOR_BG_2 = Color.FromArgb(40, 52, 62);
    private readonly Color COLOR_ELLIPSE = Color.FromArgb(0, 200, 220);

    private double _frameTime = 0.0f;
    private int _fps = 0;

    private Canvas _canvas;
    private Color[,] _image;

    private int _startX = -1;
    private int _startY = -1;
    private int _endX = -1;
    private int _endY = -1;
    private int _angle = 0;


    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    { }


    protected override void OnLoad()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _image = new Color[CANVAS_WIDTH, CANVAS_HEIGHT];
        CreateGridImage(_image, COLOR_BG_1, COLOR_BG_2);

        _canvas = new Canvas(CANVAS_WIDTH, CANVAS_HEIGHT, ClientSize.X, ClientSize.Y);
        _canvas.SetImage(_image);

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
        DrawEllipse();
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

        base.OnKeyDown(e);
    }


    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
            SetEllipseCoord();

        base.OnMouseDown(e);
    }


    private void DrawEllipse()
    {
        if (_endX == -1) return;

        CreateGridImage(_image, COLOR_BG_1, COLOR_BG_2);

        ++_angle;

        if (_angle >= 360)
            _angle = 0;

        Ellipse.Draw(_startX, _startY, _endX, _endY, _angle, _image, COLOR_ELLIPSE);
        _canvas.SetImage(_image);
    }


    private void SetEllipseCoord()
    {
        (int canvasX, int canvasY) = _canvas.GetCoord(MouseState.X, MouseState.Y);

        SetNextEllipseCoord(canvasX, canvasY);
    }


    private void SetNextEllipseCoord(int x, int y)
    {
        if (_startX == -1)
        {
            _startX = x;
            _startY = y;
            return;
        }
        else if (_endX == -1)
        {
            _endX = x;
            _endY = y;
            return;
        }

        _endX = -1;
        _endY = -1;
        _startX = x;
        _startY = y;
    }


    private void ClearLineCoord()
    {
        _startX = -1;
        _startY = -1;
        _endX = -1;
        _endY = -1;
    }


    private void ClearCanvas()
    {
        CreateGridImage(_image, COLOR_BG_1, COLOR_BG_2);
        _canvas.SetImage(_image);
    }


    private void UpdateTitle(double time)
    {
        _frameTime += time;
        _fps++;

        if (_frameTime >= 1.0f)
        {
            Title = $"{TITLE} | {_fps} fps";

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
}
