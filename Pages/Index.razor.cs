using Blazorise.Markdown;
using Blazorise;
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

        MarkdownAction[] MarkdownActionText = new MarkdownAction[]
        {
            MarkdownAction.Bold,
            MarkdownAction.Italic,
            MarkdownAction.Strikethrough,
            MarkdownAction.Heading1,
        };

        // block
        MarkdownAction[] MarkdownActionBlock = new MarkdownAction[]
        {
            MarkdownAction.Quote,
            MarkdownAction.Code,
            MarkdownAction.HorizontalRule,
            MarkdownAction.OrderedList,
            MarkdownAction.UnorderedList
        };
        // link ,image,table
        MarkdownAction[] MarkdownActionLink = new MarkdownAction[]
        {
            MarkdownAction.Link,
            MarkdownAction.Image,
            MarkdownAction.Table
        };
        protected override void OnInitialized()
        {
            // create skill functions
            skillFunctions.Add("analysis", _kernel.CreateSemanticFunction(File.ReadAllText("skills/analysis.txt"),maxTokens:2000));
            skillFunctions.Add("analysis_en", _kernel.CreateSemanticFunction(File.ReadAllText("skills/analysis_en.txt"), maxTokens: 2000));

            skillFunctions.Add("annotation", _kernel.CreateSemanticFunction(File.ReadAllText("skills/annotate.txt"), maxTokens: 2000));
            skillFunctions.Add("annotation_en", _kernel.CreateSemanticFunction(File.ReadAllText("skills/annotate_en.txt"), maxTokens: 2000));

            base.OnInitialized();
        }

        Task OnMarkdownValueChanged(string value)
        {
            markdownValue = value;

            return Task.CompletedTask;
        }

        async Task OnCustomButtonClicked(MarkdownButtonEventArgs eventArgs)
        {
            await _notificationService.Info("start ai annotate");
            bool containsChinese = Regex.IsMatch(markdownValue, @"[\u4e00-\u9fa5]");

            var functionEnd = containsChinese ? "" : "_en";

            isShowProgress = Visibility.Visible;
            stepText = "- ai annotate start\n";

            stepText += "- kernel build\n";
            progressValue = 20;

            stepText += "- analysis start\n";
            var funResult = await skillFunctions["analysis" + functionEnd].InvokeAsync(markdownValue);
            progressValue = 50;

            if (!funResult.ErrorOccurred)
            {
                // not valid json, console 
                if (!funResult.Result.StartsWith("{"))
                {
                    stepText += "- ai annotate error" + funResult.Result + " \n";
                    return;
                }
                stepText += "- analysis end\n" + funResult.Result + "\n";
                stepText += "- annotate start\n";

                funResult = await skillFunctions["annotation" + functionEnd].InvokeAsync("[TEXT BEGIN]" + markdownValue + "[TEXT END] [ANALYSIS BEGIN]" + funResult.Result + "[ANALYSIS END]");
                progressValue = 75;

                markdownValue = funResult.Result;
                stepText += "- annotate end \n";
                stepText += "- ai annotate end \n";
                progressValue = 100;

                await _notificationService.Success("success");
            }
            else
            {
                await _notificationService.Error("error");

                stepText += "- ai annotate error" + funResult.LastErrorDescription + " \n";
                progressValue = 100;
            }
        }
    }
}
