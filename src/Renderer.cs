using System;
using System.IO;
using System.Drawing;

namespace WebRenderer {

  public enum ColorEnum {
    Black,
    White,
    Red
  }

  public static class ANSI {
      public static string Reset = "\u001b[0m";
      public static string Red = "\u001b[31m";
      public static string Black = "\u001b[30m";
      public static string White = "\u001b[37m";
      public static string BackRed = "\u001b[41m";
      public static string BackBlack = "\u001b[40m";
      public static string BackWhite = "\u001b[47m";
  }

  public class Renderer {
    Bitmap Map;
    Graphics Surface;
    Pen Black_Pen = new Pen(Brushes.Black);
    MemoryStream Frame = new MemoryStream();
    StreamWriter Writer;
    StreamReader Reader;

    public void Term_Fill(int len, ColorEnum color) {
        for (int i = 0; i < len; i++) {
            Term_DrawString(" ", new PointF(0, 0), color);
        }
    }

    public void Term_DrawString(string str, PointF pos, ColorEnum bgcolor = ColorEnum.White, ColorEnum color = ColorEnum.Black) {
        string back = ANSI.BackWhite;
        string fore = ANSI.Black;

        if (color == ColorEnum.Red) {
            fore = ANSI.Red;
        }

        if (bgcolor == ColorEnum.Red) {
            back = ANSI.BackRed;
        }

        if (bgcolor == ColorEnum.Black) {
            back = ANSI.BackBlack;
        }

        Writer.Write(back);
        Writer.Write(fore);
        Writer.Write(str);
        Writer.Write(ANSI.Reset);
        Writer.Flush();
    }

    public void Term_Clear() {
        Frame.Dispose();
        Frame = new MemoryStream();
        Writer = new StreamWriter(Frame);
        Reader = new StreamReader(Frame);

        Console.Clear();
    }

    public void Term_Render() {
        Frame.Seek(0, SeekOrigin.Begin);
        Console.Write(Reader.ReadToEnd());
        Frame.Seek(0, SeekOrigin.Begin);
    }

    private static Font DefaultFont = new Font("Arial", 20);
    public Font Bold = new Font(DefaultFont, FontStyle.Bold);

    public void Clear() {
      Surface.Clear(Color.White);
    }

    public SizeF Measure(string str, Font font = null) {
      return Surface.MeasureString(str, font == null ? DefaultFont : font);
    }

    public void DrawString(string str, PointF pos, Font f = null, ColorEnum color = ColorEnum.Black) {
      switch (color) {
        case ColorEnum.Black:
          Surface.DrawString(str, f == null ? DefaultFont : f, Brushes.Black, pos);
          break;
        case ColorEnum.Red:
          Surface.DrawString(str, f == null ? DefaultFont : f, Brushes.Red, pos);
          break;
        default:
        break;
      }
    }

    public void DrawRect(Rectangle rect, ColorEnum color = ColorEnum.Black) {
      switch (color) {
        case ColorEnum.Black:
          Surface.FillRectangle(Brushes.Black, rect);
          break;
        case ColorEnum.Red:
          Surface.FillRectangle(Brushes.Red, rect);
          break;
        default:
        break;
      }
    }

    public void Save(string file) {
      Map.Save(file);
    }

    public Renderer(Bitmap img) {
      Map = img;
      Surface = Graphics.FromImage(img);
    }

    public Renderer() {}
  }
}