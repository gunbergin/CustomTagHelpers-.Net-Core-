using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CustomTagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    //[HtmlTargetElement("tag-name")]
    //public class FormTagHelper : TagHelper
    //{
    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {

    //    }
    //}

    public enum InputFormat
    {
        text = 1,
        number = 2
    }

    [HtmlTargetElement("form")]
    public class FormCustom : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public FormCustom(IUrlHelperFactory factory)
        {
            urlHelperFactory = factory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContextData);
            output.Attributes.SetAttribute("action", urlHelper.Action(
                ViewContextData.RouteData.Values["action"].ToString(), ViewContextData.RouteData.Values["controller"].ToString()));
        }
    }

 
    public class TextboxTagHelper : TagHelper
    {
        private readonly IHtmlGenerator htmlGenerator;
        private readonly HtmlEncoder htmlEncoder;

        public TextboxTagHelper(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder)
        {
            this.htmlGenerator = htmlGenerator;
            this.htmlEncoder = htmlEncoder;
        }

        private const string ForAttributeName = "asp-for";

        public string Class { get; set; } = "font-size-16 ocean-blue text-uppercase";
        public bool Isreadonly { get; set; }

        public InputFormat Input { get; set; }
      
        public int maxlenght { get; set; }

        public string InfoText { get; set; }

        public string LabelText { get; set; }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "form-group");


            var inputParent = new TagBuilder("div");
            inputParent.AddCssClass("floating-label");

            using (var writer = new StringWriter())
            {
                WriteLabel(writer);
                WriteInput(writer);
                WriteValidation(writer);
                inputParent.InnerHtml.AppendHtml(writer.ToString());


                output.Content.AppendHtml(inputParent);

                if (!string.IsNullOrWhiteSpace(InfoText))
                {
                    output.Content.AppendHtml("<br/>");
                    var smalltext = new TagBuilder("small");
                    var br = new TagBuilder("br");
                    smalltext.AddCssClass("Info-text");
                    smalltext.InnerHtml.Append(InfoText);

                    output.Content.AppendHtml(smalltext);


                }
            }
        }

        private void WriteLabel(TextWriter writer)
        {
            var tagBuilder = htmlGenerator.GenerateLabel(
              ViewContext,
              For.ModelExplorer,
              For.Name,
              labelText: LabelText,
              htmlAttributes: new { @class = "Flabel-text" });
            tagBuilder.WriteTo(writer, htmlEncoder);
        }

        private void WriteInput(TextWriter writer)
        {
            var tagBuilder = htmlGenerator.GenerateTextBox(
              ViewContext,
              For.ModelExplorer,
              For.Name,
              For.Model,
              format: null,
              htmlAttributes: new { @class = $"form-control {Class}" });

            switch (Input)
            {
                case InputFormat.text:
                    tagBuilder.MergeAttribute("onkeypress", "return onlyAlphabets(event,this);");
                    break;
                case InputFormat.number:
                    tagBuilder.MergeAttribute("onkeypress", "return isNumber(event)");
                    break;
            }

            if (Isreadonly)
            {
                tagBuilder.MergeAttribute("readonly", "");
            }

            if (maxlenght > 0)
            {
                tagBuilder.MergeAttribute("maxlength", maxlenght.ToString());
            }

            tagBuilder.WriteTo(writer, htmlEncoder);
        }

        private void WriteValidation(TextWriter writer)
        {
            var tagBuilder = htmlGenerator.GenerateValidationMessage(
              ViewContext,
              For.ModelExplorer,
              For.Name,
              message: null,
              tag: null,
              htmlAttributes: new { @class = "text-danger" });

            tagBuilder.WriteTo(writer, htmlEncoder);
        }
    }

    public class CheckBoxTagHelper : TagHelper
    {
        private readonly IHtmlGenerator htmlGenerator;
        private readonly HtmlEncoder htmlEncoder;

        public CheckBoxTagHelper(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder)
        {
            this.htmlGenerator = htmlGenerator;
            this.htmlEncoder = htmlEncoder;
        }

        private const string ForAttributeName = "asp-for";

        public string Class { get; set; }

        public string Text { get; set; }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "form-check pb-3  d-flex align-items-center");


            var inputParent = new TagBuilder("span");
            inputParent.AddCssClass("checkbox");


            using (var writer = new StringWriter())
            {

                WriteInput(writer);
                inputParent.WriteTo(writer, htmlEncoder);
                var label = WriteLabel(writer);

                output.Content.AppendHtml(label);
            }
        }

        private TagBuilder WriteLabel(TextWriter writer)
        {
            // var tagBuilder = htmlGenerator.GenerateLabel(
            //   ViewContext,
            //   For.ModelExplorer,
            //   Value,
            //   labelText: Text ?? Value,
            //   htmlAttributes: new { @class = "checkmarkContainer funds-perfomance-legend-text ahe-color-black ahe-margin-left-s" });
            //tagBuilder.InnerHtml.AppendHtml(writer.ToString());

            TagBuilder tb = new TagBuilder("label");

            //Assign Css Class  
            tb.AddCssClass("checkboxContainer font-size-16 black opacity-8 ml-1");


            tb.InnerHtml.AppendHtml(Text);
            tb.InnerHtml.AppendHtml(writer.ToString());
            return tb;

        }

        private void WriteInput(TextWriter writer)
        {
            var tagBuilder = htmlGenerator.GenerateCheckBox(
              ViewContext,
              For.ModelExplorer,
              For.Name,
              isChecked: false,
              htmlAttributes: new { @class = $"{Class}" });

            tagBuilder.WriteTo(writer, htmlEncoder);
        }


    }

    public class RadioButtonTagHelper : TagHelper
    {
        private readonly IHtmlGenerator htmlGenerator;
        private readonly HtmlEncoder htmlEncoder;

        public RadioButtonTagHelper(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder)
        {
            this.htmlGenerator = htmlGenerator;
            this.htmlEncoder = htmlEncoder;
        }

        private const string ForAttributeName = "asp-for";

        public string Class { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "form-check d-flex align-items-center");


            var inputParent = new TagBuilder("span");
            inputParent.AddCssClass("checkmark");


            using (var writer = new StringWriter())
            {

                WriteInput(writer);
                inputParent.WriteTo(writer, htmlEncoder);
                var label = WriteLabel(writer);

                output.Content.AppendHtml(label);
            }
        }

        private TagBuilder WriteLabel(TextWriter writer)
        {
            // var tagBuilder = htmlGenerator.GenerateLabel(
            //   ViewContext,
            //   For.ModelExplorer,
            //   Value,
            //   labelText: Text ?? Value,
            //   htmlAttributes: new { @class = "checkmarkContainer funds-perfomance-legend-text ahe-color-black ahe-margin-left-s" });
            //tagBuilder.InnerHtml.AppendHtml(writer.ToString());

            TagBuilder tb = new TagBuilder("label");

            //Assign Css Class  
            tb.AddCssClass("checkmarkContainer input-text ahe-color-black ahe-margin-left-s");


            tb.InnerHtml.Append(Text ?? Value);
            tb.InnerHtml.AppendHtml(writer.ToString());
            return tb;

        }

        private void WriteInput(TextWriter writer)
        {
            var tagBuilder = htmlGenerator.GenerateRadioButton(
              ViewContext,
              For.ModelExplorer,
              For.Name,
              value: Value,
              isChecked: false,
              htmlAttributes: new { @class = $"radiobuttongroup {Class}" });

            tagBuilder.WriteTo(writer, htmlEncoder);
        }


    }
}
