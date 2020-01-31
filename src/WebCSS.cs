using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WebCSS {

  public class Style {
    public string Name;
    public string Value;

    public Style(string N, string V) {
      Name = N;
      Value = V;
    }
  }

  public class CSSElement {
    public string Name;
    public List<Style> Styles = new List<Style>();

    public Style Get(string style) {
      return Styles.Find(s => s.Name == style.ToLower());
    }

    public CSSElement(string N) {
      Name = N;
    }
  }

  public class StyleContainer {
    private static readonly Regex TopRegex = new Regex(@"(.+) +?\{([^\}]+)?\}");
    private static readonly Regex StyleRegex = new Regex(@"(?:\s+)?([^:]+):\s*([^\s;]+);?");

    public List<CSSElement> Elements = new List<CSSElement>();

    public CSSElement Get(string elem) {
      return Elements.Find(e => e.Name == elem.ToLower());
    }

    public bool Exists(string elem) {
      return Get(elem) != null;
    }

    private void ParseStyles(string css) {
      foreach (Match elem in TopRegex.Matches(css)) {
        CSSElement styleelement = new CSSElement(elem.Groups[1].ToString());

        foreach (Match styles in StyleRegex.Matches(elem.Groups[2].ToString())) {
          styleelement.Styles.Add(new Style(styles.Groups[1].ToString(), styles.Groups[2].ToString()));
        }

        Elements.Add(styleelement);
      }
    }

    public static CSSElement ParseStyleString(string css) {
      CSSElement result = new CSSElement(null);

      foreach (Match styles in StyleRegex.Matches(css)) {
          result.Styles.Add(new Style(styles.Groups[1].ToString(), styles.Groups[2].ToString()));
      }

      return result;
    }

    public StyleContainer(string css) {
      ParseStyles(css);
    }
  }
}