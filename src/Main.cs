using System;
using System.IO;
using System.Drawing;
using WebRenderer;
using WebHtml;
using WebCSS;
using WebJS;

// F1
// Open Shell
// bash Setup.sh
// bash Run.sh

class MainClass {
  public static void Main (string[] args) {

    if (args.Length > 0 && args[0] == "-t") {
        Renderer termrend = new Renderer();

        StyleContainer termcont = new StyleContainer(new StreamReader("Test.css").ReadToEnd());
        HtmlLayout termhtml = new HtmlLayout(new StreamReader("Test.html").ReadToEnd());

        termhtml.CSS = termcont;

        State termstate = new State(termhtml);

        termhtml.Render(termrend, true);
        termrend.Term_Render();
        
        // Render any script changes
        termhtml.Render(termrend, true);
        termrend.Term_Render();
        
        Console.WriteLine();
        return;
    }

    Bitmap map = new Bitmap(600, 600);

    Renderer rend = new Renderer(map);

    StyleContainer cont = new StyleContainer(new StreamReader("Test.css").ReadToEnd());
    HtmlLayout html = new HtmlLayout(new StreamReader("Test.html").ReadToEnd());

    html.CSS = cont;
    
    State jsstate = new State(html);

    html.Render(rend);

    jsstate.ProcessEvents();
    
    html.Render(rend);

    // Save rendered frame to file
    rend.Save("Output.png");
  }
}