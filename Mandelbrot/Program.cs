using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Szark;

namespace Mandelbrot
{
  class Program : SzarkEngine
  {
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;

    private Random random;
    private SpriteRenderer renderer;

    int maxIteration = 100;
    double middleR = -0.75;
    double middleI = 0;
    double rangeR = 3.5;
    double rangeI = 2;

    bool needsUpdate = true;
    int lastScrolValue = OpenTK.Input.Mouse.GetCursorState().ScrollWheelValue;

    Program() : base("Mandelbrot", WINDOW_WIDTH, WINDOW_HEIGHT, 1)
    {
    }
      

    protected override void Start()
    {
      random = new Random();
      renderer = CreateRenderer(new Sprite(ScreenWidth, ScreenHeight));
    }

    protected override void Update(float deltaTime)
    {
      if (Input.GetKey(OpenTK.Input.Key.KeypadMinus))
      {
        maxIteration--;
        needsUpdate = true;
      }
      if (Input.GetKey(OpenTK.Input.Key.KeypadPlus))
      {
        maxIteration++;
        needsUpdate = true;
      }
      if (Input.GetKey(OpenTK.Input.Key.W))
      {
        middleI -= 0.1 * rangeI * deltaTime;
        needsUpdate = true;
      }
      if (Input.GetKey(OpenTK.Input.Key.S))
      {
        middleI += 0.1 * rangeI * deltaTime;
        needsUpdate = true;
      }
      if (Input.GetKey(OpenTK.Input.Key.A))
      {
        middleR -= 0.1 * rangeR * deltaTime;
        needsUpdate = true;
      }
      if (Input.GetKey(OpenTK.Input.Key.D))
      {
        middleR += 0.1 * rangeR * deltaTime;
        needsUpdate = true;
      }
      if (lastScrolValue - OpenTK.Input.Mouse.GetCursorState().ScrollWheelValue < 0)
      {
        rangeR *= 0.8;
        rangeI *= 0.8;
        lastScrolValue = OpenTK.Input.Mouse.GetCursorState().ScrollWheelValue;
        needsUpdate = true;
      }
      else if (lastScrolValue - OpenTK.Input.Mouse.GetCursorState().ScrollWheelValue > 0)
      {
        rangeR /= 0.8;
        rangeI /= 0.8;
        lastScrolValue = OpenTK.Input.Mouse.GetCursorState().ScrollWheelValue;
        needsUpdate = true;
      }
    }

    protected override void Draw(float deltaTime)
    {
      if (needsUpdate)
      {
        Task[] multipleTasks;
        multipleTasks = new[]
        {
          Task.Run(() => Calculation(ScreenWidth / 16 * 0, ScreenWidth / 16 * 1)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 1, ScreenWidth / 16 * 2)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 2, ScreenWidth / 16 * 3)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 3, ScreenWidth / 16 * 4)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 4, ScreenWidth / 16 * 5)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 5, ScreenWidth / 16 * 6)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 6, ScreenWidth / 16 * 7)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 7, ScreenWidth / 16 * 8)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 8, ScreenWidth / 16 * 9)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 9, ScreenWidth / 16 * 10)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 10, ScreenWidth / 16 * 11)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 11, ScreenWidth / 16 * 12)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 12, ScreenWidth / 16 * 13)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 13, ScreenWidth / 16 * 14)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 14, ScreenWidth / 16 * 15)),
          Task.Run(() => Calculation(ScreenWidth / 16 * 15, ScreenWidth / 16 * 16))
        };

        Task.WaitAll(multipleTasks);

        needsUpdate = false;
      }
      renderer.Render(0, 0, 0, 1, -1, true);
      renderer.Refresh();
    }


    private void Calculation(int startX, int endX)
    {
      for (int x = startX; x < endX; x++)
      {
        for (int y = 0; y < ScreenHeight; y++)
        {
          double xPercentage = x / (double)ScreenWidth;
          double yPercentage = y / (double)ScreenHeight;

          double cReal = xPercentage * rangeR + middleR - rangeR / 2;
          double cImaginary = yPercentage * rangeI + middleI - rangeI / 2;

          double zReal = 0;
          double zImaginary = 0;

          int iteration = 0;
          while (iteration < maxIteration && zReal * zReal + zImaginary * zImaginary <= 4)
          {
            double temp = zReal * zReal - zImaginary * zImaginary + cReal;
            zImaginary = 2 * zReal * zImaginary + cImaginary;
            zReal = temp;
            iteration++;
          }


          double colorVal = iteration / (double)maxIteration;
          ColorConverter.HsvToRgb(-colorVal * 360 + 240, 1, 1 - colorVal, out byte r, out byte g, out byte b);
          Pixel pixel = new Pixel(r, g, b);


          renderer.Graphics.Draw(x, y, pixel);
        }
      }
    }

    protected override void Destroyed() { }

    static void Main(string[] theArgs) =>
        new Program();
  }
}