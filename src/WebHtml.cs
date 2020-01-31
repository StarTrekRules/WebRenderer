using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebJS;
using WebCSS;
using WebRenderer;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebHtml {

  public class HtmlAttribute {
    public string Name;
    public string Value;

    public HtmlAttribute(string N, string V) {
      Name = N;
      Value = V;
    }
  }

  // Use anglesharp for html parsing!
  // An html element
  public class HtmlNode {
    public string Tag;
    public Dictionary<string, string> TagAttributes = new Dictionary<string, string>();
    public string TagBody;
    public List<HtmlNode> Children = new List<HtmlNode>();

    public void SetAttributes(INamedNodeMap nodemap) {
        foreach (IAttr atr in nodemap) {
            TagAttributes.Add(atr.Name, atr.Value);
        }
    }

    public HtmlAttribute Get(string attr) {
      if (! TagAttributes.ContainsKey(attr))
        return null;

      return new HtmlAttribute(attr, TagAttributes[attr]);
    }

    public IEnumerator<HtmlNode> GetEnumerator() {
      return Children.GetEnumerator();
    }
  }

  public class HtmlLayout : HtmlTree {

    public const int TextHeight = 55;
    public int LineY = 0;
    public int LineX = 0;
    private Renderer _Rend;

    public void LeftPadToPosition(int left) {
        if (LineX != left) {
            LineX = left;
            _Rend.Term_Fill(LineX, ColorEnum.Black);
        }
    }

    public new void Render(Renderer rend, bool terminal = false) {
      _Rend = rend;

      if (terminal)
        rend.Term_Clear();
      else
        rend.Clear();

      LineY = 0;
      LineX = 0;

      int lasty = -1;

      foreach (HtmlNode elem in Elements) {
          HtmlAttribute x = elem.Get("x");
          HtmlAttribute y = elem.Get("y");
          CSSElement csselem = null;
          
          if (csselem == null) {
              HtmlAttribute style = elem.Get("style");

              if (style != null)
                csselem = StyleContainer.ParseStyleString(style.Value);
          }

          if (CSS != null && csselem == null) {
            csselem = CSS.Get(elem.Tag);
          }
          int lf = -1;

          if (csselem != null) {
              Style left = csselem.Get("left");

              if (left != null) {
                  lf = int.Parse(left.Value);
              }
          }

          if (LineY != lasty) {
              lasty = LineY;

              LeftPadToPosition(lf);
          }

          if (elem.Tag == "RECT" && (x != null && y != null)) {
            int xi = int.Parse(x.Value);
            int yi = int.Parse(y.Value);
            
            if (! terminal)
                rend.DrawRect(new Rectangle(xi, yi, 200, 200), GetColor(csselem.Get(elem.Tag)));
            continue;
          }
          
          if (elem.Tag == "P") {

            if (terminal) {
                rend.Term_DrawString(elem.TagBody, new PointF(0, 0));
                continue;
            }

            rend.DrawString(elem.TagBody, new PointF(LineX, LineY));
            LineX += (int) rend.Measure(elem.TagBody).Width;
          }

          if (elem.Tag.StartsWith("H") && elem.Tag.Length == 2) {
              rend.Term_DrawString("<BOLD>" + elem.TagBody, new Point(0, 0));
              rend.Term_DrawString("\n\n", new PointF(0, 0));
          }

          if (elem.Tag == "IMG") {
              if (terminal) {
                  WebImage image = new WebImage(elem);

                  for (int i = 0; i < image.SizeY; i++) {
                      LeftPadToPosition(lf); // Pad to the left style property
                      
                    // Get pixel(character and color) data and draw it
                    foreach (WebImage.PixelData pixel in image.GetRow(i)) {
                        rend.Term_DrawString("" + pixel.Character, new PointF(0, 0), pixel.Color);
                    }

                    if (i != image.SizeY - 1) {
                        rend.Term_DrawString("\n", new PointF(0, 0));
                        LineY += 1;
                        LineX = 0;
                    }
                  }
              }
          }

          if (elem.Tag == "B") {

            if (csselem.Get("background") != null) {

                if (terminal) {
                    LineX += elem.TagBody.Length;
                    rend.Term_DrawString(elem.TagBody, new PointF(0, 0), GetColor(csselem.Get("background")), GetColor(csselem.Get("color")));
                    continue;
                }

                SizeF bounds = rend.Measure(elem.TagBody, rend.Bold);

                rend.DrawRect(new Rectangle(LineX, LineY, (int) bounds.Width, (int) bounds.Height), GetColor(csselem.Get("background")));
            }

            if (terminal) {
                rend.Term_DrawString(elem.TagBody, new PointF(0, 0), ColorEnum.White, GetColor(csselem.Get("color")));
                LineX += elem.TagBody.Length;
                continue;
            }

            rend.DrawString(elem.TagBody, new PointF(LineX, LineY), rend.Bold, GetColor(csselem.Get("color")));
            
            LineX += (int) rend.Measure(elem.TagBody).Width;
          }

          if (elem.Tag == "SCRIPT") {
              JSState.Engine.Execute(elem.TagBody);
          }

          if (elem.Tag == "BR") {

            if (terminal) {
                LineX = 0;
                LineY += 1;
                rend.Term_DrawString("\n", new PointF(0, 0));
                continue;
            }

            LineX = 0;
            LineY += (int) rend.Measure("A").Height;
          }
      }
    }

    public HtmlLayout(string html) : base(html) {}
  }

  // Legacy class left over from before I decided to use anglesharp, decided to keep the progress and build on top of it
  public class HtmlTree {
    public List<HtmlNode> Elements = new List<HtmlNode>();
    public HtmlNode Root;
    private static readonly Regex HtmlRegex = new Regex(@"(?:<\\?(.*|.*?)([^>]*)(?:(?:\/>|>)?((?:.|\n)*?)(?:<\/\1\/?>)))|(?:<\/?(.*)\/?>)");
    private static readonly Regex TagRegex = new Regex("(?:[\\s]+)? ([^=]+)=\"([^\"]+)\"");

    public State JSState;

    public StyleContainer CSS;

    // Return a color from a style
    public ColorEnum GetColor(Style st) {
      if (st != null) {
        return st.Value == "red" ? ColorEnum.Red : ColorEnum.Black;
      }

      return ColorEnum.Black;
    }

    // Rendering (UNUSED, See HtmlLayout)
    public void Render(Renderer rend) {
      rend.Clear();
      
      foreach (HtmlNode elem in Elements) {
          HtmlAttribute x = elem.Get("x");
          HtmlAttribute y = elem.Get("y");
          CSSElement csselem = null;
          
          if (csselem == null) {
              HtmlAttribute style = elem.Get("style");

              if (style != null)
                csselem = StyleContainer.ParseStyleString(style.Value);
          }

          if (CSS != null && csselem == null) {
            csselem = CSS.Get(elem.Tag);
          }

          if (elem.Tag == "RECT" && (x != null && y != null)) {
            int xi = int.Parse(x.Value);
            int yi = int.Parse(y.Value);
            
            rend.DrawRect(new Rectangle(xi, yi, 200, 200), GetColor(csselem.Get(elem.Tag)));
          }
        }
    }

    private void PopulateChildren(IElement elem, HtmlNode node) {
        foreach (var el in elem.Children) {
            HtmlNode child = new HtmlNode();

            child.Tag = el.TagName;
            child.TagBody = el.Text();

            child.SetAttributes(el.Attributes);

            PopulateChildren(el, child);

            node.Children.Add(child);
        }
    }

    private void Parse(string html, HtmlNode parent = null) {
      // https://regexr.com/4hr80 if you want the regex I ingeniously created

      IBrowsingContext context = BrowsingContext.New(Configuration.Default);

      IDocument doc = context.OpenAsync(req => req.Content(html)).Result;

      foreach (var el in doc.All) {
        HtmlNode node = new HtmlNode();
        node.SetAttributes(el.Attributes);
        node.Tag = el.TagName;
        node.TagBody = el.Text();

        Elements.Add(node);
      }

      HtmlNode root = new HtmlNode();

      root.Tag = doc.Body.TagName;
      root.TagBody = doc.Body.Text();

      PopulateChildren(doc.Body, root);

      Root = root;

    }

    public HtmlTree(string html) {
      Parse(html);
    }

  }
}