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

    public int StartX { get; private set; }

    public int StartY { get; private set; }

    public int EndX { get; private set; }

    public int EndY { get; private set; }

    private float _frameTime = 0.0f;
    private int _fps = 0;

    private Canvas _canvas;
    private Color[,] _image;


    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    { }


    protected override void OnLoad()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        StartX = -1;
        StartY = -1;
        EndX = -1;
        EndY = -1;

        _image = new Color[CANVAS_WIDTH, CANVAS_HEIGHT];
        CreateGridImage(_image);

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
        _frameTime += (float)e.Time;
        _fps++;

        if (_frameTime >= 1.0f)
        {
            Title = $"{TITLE} | {_fps} fps";
            _frameTime = 0.0f;
            _fps = 0;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            CreateGridImage(_image);
            _canvas.SetImage(_image);
        }
            
        //if (MouseState.IsButtonDown(MouseButton.Left))

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


    private static void CreateGridImage(Color[,] image)
    {
        int rows = image.GetLength(0);
        int cols = image.GetLength(1);

        bool setColor = false;

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                if (setColor)
                    image[i, j] = Color.Gray;
                else
                    image[i, j] = Color.DarkGray;

                setColor = setColor == false;
            }

            setColor = setColor == false;
        }
    }


    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button != MouseButton.Left)
            return;

        (int canvasX, int canvasY) = _canvas.GetCoord(MouseState.X, MouseState.Y);

        if (SetLineCoord(canvasX, canvasY))
        {
            Line.CreateLineDDA(StartX, StartY, EndX, EndY, _image, Color.Blue);
            _canvas.SetImage(_image);

            ClearLineCoor();
        }
    }


    private bool SetLineCoord(int x, int y)
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


    private void ClearLineCoor()
    {
        StartX = -1;
        StartY = -1;
        EndX = -1;
        EndY = -1;
    }
}
