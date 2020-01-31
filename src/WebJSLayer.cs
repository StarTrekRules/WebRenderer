using System;
using System.Collections.Generic;
using WebHtml;
using Jurassic;
using Jurassic.Library;

namespace WebJS {

  public class Event {
    public string Name;
    public HtmlNode Element;

    public Event(string N, HtmlNode E) {
      Name = N;
      Element = E;
    }
  }

  public class ElementObject : ObjectInstance {
      State JSState;
      HtmlNode Element;

      /*[JSFunction(Name = "getElementById")]
      public ObjectInstance GetElementById(string id) {
          foreach (HtmlNode elem in Html.Elements) {
              if (elem.Get("id")?.Value == id) {
                  return JSState.WrapElement(elem);
              }
          }

          return null;
      }*/

      private string _InnerHTML;

      [JSProperty(Name = "innerHTML")]
      public string InnerHTML {
        get {
            return _InnerHTML;
        }

        set {
            _InnerHTML = value;

            Element.TagBody = _InnerHTML;
        }
      }

      public ElementObject(State state, HtmlNode elem) : base(state.Engine) {
          JSState = state;
          Element = elem;
          this.PopulateFunctions();
      }
  }

  public class DocumentObject : ObjectInstance {
      State JSState;
      HtmlTree Html;

      [JSFunction(Name = "getElementById")]
      public ObjectInstance GetElementById(string id) {
          foreach (HtmlNode elem in Html.Elements) {
              if (elem.Get("id")?.Value == id) {
                  return JSState.WrapElement(elem);
              }
          }

          return null;
      }

      public DocumentObject(State state, HtmlTree html) : base(state.Engine) {
          JSState = state;
          Html = html;
          this.PopulateFunctions();
      }
  }

  public class State {
    public ScriptEngine Engine = new ScriptEngine();

    public Queue<Event> EventQueue = new Queue<Event>();
    public HtmlTree Html;

    // Document methods
    public ObjectInstance WrapElement(HtmlNode element) {
        ElementObject obj = new ElementObject(this, element);

        obj["tagName"] = element.Tag;
        obj["innerHTML"] = element.TagBody;
        
        return obj;
    }

    public void ProcessEvents() {
      while (EventQueue.Count > 0) {
        Event e = EventQueue.Dequeue();

        if (e.Name == "Debug") {
          e.Element.Get("x").Value = "0";
        }

        Engine.CallGlobalFunction(e.Name);
      }
    }

    public State(HtmlTree H) {
      Html = H;
      Html.JSState = this;

      /*ScriptEngine apiscope = new ScriptEngine();

      apiscope.ForceStrictMode = true;

      apiscope.ExecuteFile("api/Document.js");*/

      Engine.Global["document"] = new DocumentObject(this, Html);

      Engine.SetGlobalFunction("log", new Func<string, int>((str) => { Console.WriteLine(str); return 0; }));
      Engine.SetGlobalFunction("test", new Action(() => {
          foreach (HtmlNode node in Html.Elements) {
              foreach (KeyValuePair<string, string> attr in node.TagAttributes) {
                  if (attr.Key == "change") {
                      node.TagBody = "Javascript touched me :(";
                  }
              }
          }
       }));
    }
  }
}