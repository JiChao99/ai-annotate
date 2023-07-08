using Blazorise;
using Blazorise.Markdown;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;

namespace AIAnnotate.Pages
{
    public partial class Index
    {
        [Inject] private INotificationService _notificationService { get; set; } = null!;
        [Inject] private IKernel _kernel { get; set; } = null!;

        string markdownValue = "Input your text, and click AI √ ";
        string stepText = string.Empty;
        Dictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction> skillFunctions = new();
        int progressValue = 0;
        Visibility isShowProgress = Visibility.Invisible;

        readonly MarkdownAction[] MarkdownActionText = new MarkdownAction[]
        {
            MarkdownAction.Bold,
            MarkdownAction.Italic,
            MarkdownAction.Strikethrough,
            MarkdownAction.Heading1,
        };

        // block
        readonly MarkdownAction[] MarkdownActionBlock = new MarkdownAction[]
        {
            MarkdownAction.Quote,
            MarkdownAction.Code,
            MarkdownAction.HorizontalRule,
            MarkdownAction.OrderedList,
            MarkdownAction.UnorderedList
        };
        // link ,image,table
        readonly MarkdownAction[] MarkdownActionLink = new MarkdownAction[]
        {
            MarkdownAction.Link,
            MarkdownAction.Image,
            MarkdownAction.Table
        };
        protected override void OnInitialized()
        {
            // create skill functions
            skillFunctions.Add("analysis", _kernel.CreateSemanticFunction(File.ReadAllText("skills/analysis.txt"), maxTokens: 2000, temperature: 0.5, topP: 0.8));
            skillFunctions.Add("analysis_en", _kernel.CreateSemanticFunction(File.ReadAllText("skills/analysis_en.txt"), maxTokens: 2000, temperature: 0.5, topP: 0.8));

            skillFunctions.Add("annotation", _kernel.CreateSemanticFunction(File.ReadAllText("skills/annotate.txt"), maxTokens: 2000, temperature: 0.5, topP: 0.8));
            skillFunctions.Add("annotation_en", _kernel.CreateSemanticFunction(File.ReadAllText("skills/annotate_en.txt"), maxTokens: 2000, temperature: 0.5, topP: 0.8));

            base.OnInitialized();
        }

        Task OnMarkdownValueChanged(string value)
        {
            markdownValue = value;

            return Task.CompletedTask;
        }

        async Task OnCustomButtonClicked(MarkdownButtonEventArgs eventArgs)
        {
            if (eventArgs.Name != "annotate")
            {
                return;
            }
            // processing
            if (progressValue > 0 && progressValue < 100)
            {
                await _notificationService.Info("Processing...");
                return;
            }
            await _notificationService.Info("start AI annotate");
            bool containsChinese = Regex.IsMatch(markdownValue, @"[\u4e00-\u9fa5]");

            var functionEnd = containsChinese ? "" : "_en";

            isShowProgress = Visibility.Visible;
            stepText = "- AI annotate start\n";

            stepText += "- kernel build\n";
            progressValue = 20;

            stepText += "- analysis start\n";
            var funResult = await skillFunctions["analysis" + functionEnd].InvokeAsync(markdownValue);
            progressValue = 50;

            if (!funResult.ErrorOccurred)
            {
                // not valid json, console 
                if (!funResult.Result.Contains("words"))
                {
                    stepText += "- AI annotate error" + funResult.Result + " \n";
                    return;
                }
                stepText += "- analysis end\n" + funResult.Result + "\n";
                stepText += "- annotate start\n";

                funResult = await skillFunctions["annotation" + functionEnd].InvokeAsync("文本信息：" + markdownValue + "分析信息：" + funResult.Result);
                progressValue = 75;

                markdownValue = funResult.Result;
                stepText += "- annotate end \n";
                stepText += "- AI annotate end \n";
                progressValue = 100;

                await _notificationService.Success("success");
            }
            else
            {
                await _notificationService.Error("error");

                stepText += "- AI annotate error" + funResult.LastErrorDescription + " \n";
                progressValue = 100;
            }
        }
    }
}
