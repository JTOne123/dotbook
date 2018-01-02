using System.Collections.Generic;
using System.Text;
using DotBook.Processing;

namespace DotBook.Backend
{
    internal class HtmlFormatter : StringFormatterBase
    {
        protected override string Extension => ".html";

        // highlight.js related stuff
        // TODO Remove hardcode and make it configurable
        protected string includes = "<link rel=\"stylesheet\" " +
            "href=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/default.min.css\">" +
            "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/highlight.min.js\"></script>";
        
        protected string styles = @"
        pre code.hljs {
            display: block;
        }

        code.hljs {
            display: inline;
        }";

        protected string script = @"
            var code = document.getElementsByTagName('code');
            for (i = 0; i < code.length; i++) hljs.highlightBlock(code[i]);
        ";

        protected override void Start(Entity entity)
        {
            base.Start(entity);
            var title = entity.Base?.FullName ?? "Index";
            WriteLine("<!DOCTYPE HTML>");
            WriteLine($"<html><head><title>{title}</title>");
            WriteLine("<meta charset=\"utf-8\">");
            WriteLine(includes);
            WriteLine($"<style>{styles}</style>");
            WriteLine("</head><body>");
        }

        protected override string Result()
        {
            WriteLine($"<script>{script}</script>");
            WriteLine("</body></html>");
            return base.Result();
        }

        protected override StringFormatterBase Code(string code)
        {
            WriteLine($"<pre><code>{code}</code></pre>");
            return this;
        }

        protected override StringFormatterBase CodeInline(string code)
        {
            WriteLine($"<code>{code}</code>");
            return this;
        }

        protected override StringFormatterBase Header(string title, int level = 1)
        {
            WriteLine($"<h{level}>{title}</h{level}>");
            return this;
        }

        protected override StringFormatterBase HorizontalRule()
        {
            WriteLine($"<hr>");
            return this;
        }

        protected override StringFormatterBase Image(string url, string title = "")
        {
            WriteLine($"<img src=\"{url}\" alt=\"{title}\">");
            return this;
        }

        protected override StringFormatterBase Link(string title, string url)
        {
            WriteLine($"<a href=\"{url}\">{title}</a>");
            return this;
        }

        protected override StringFormatterBase LinkList(IDictionary<string, string> links)
        {
            WriteLine("<ul>");
            foreach (var pair in links) LinkListItem(pair.Key, pair.Value);
            WriteLine("</ul>");
            return this;
        }

        protected override StringFormatterBase LinkListItem(string title, string url)
        {
            WriteLine($"<li><a href=\"{url}\">{title}</a></li>");
            return this;
        }

        protected override StringFormatterBase List(IEnumerable<string> items, ListStyle style = ListStyle.Bullet)
        {
            var tag = style == ListStyle.Bullet ? "ul" : "ol";
            WriteLine($"<{tag}>");
            foreach (var item in items) WriteLine($"<li>{item}</li>");
            WriteLine($"</{tag}>");
            return this;
        }

        protected override StringFormatterBase ParagraphEnd()
        {
            WriteLine("</p>");
            return this;
        }

        protected override StringFormatterBase ParagraphStart()
        {
            WriteLine("<p>");
            return this;
        }

        protected override StringFormatterBase Table(List<string> header, List<List<string>> rows)
        {
            WriteLine("<table>");
            WriteLine("<thead><tr>");
            foreach(var cell in header)
                WriteLine($"<td>{cell}</td>");
            WriteLine("</tr></thead>");
            WriteLine("<tbody>");
            foreach (var row in rows)
            {
                WriteLine("<tr>");
                foreach(var cell in row)
                    WriteLine($"<td>{row}</td>");
                WriteLine("</tr>");
            }
            WriteLine("</tbody>");
            WriteLine("</table>");
            return this;
        }

        protected override StringFormatterBase Text(string text, TextStyle style = TextStyle.Normal)
        {
            var result = text;
            if (style.HasFlag(TextStyle.Bold)) result = $"<b>{result}</b>";
            if (style.HasFlag(TextStyle.Italic)) result = $"<i>{result}</i>";
            Write(result);
            return this;
        }
    }
}