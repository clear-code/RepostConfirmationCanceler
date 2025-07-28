using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace RepostConfirmationCanceler
{
    internal static class ConfirmationDialogCanceler
    {
        internal static void WatchEdgeDialog(RuntimeContext context)
        {
            AutomationElement desktop = AutomationElement.RootElement;
            while (!context.IsEndTime)
            {
                //Edgeのプロセスのうち、メインウィンドウがあるものに絞り込み
                var edges = Process.GetProcessesByName("msedge").Where(_ => _.MainWindowHandle != IntPtr.Zero);
                foreach (var edge in edges)
                {
                    var targetPid = edge.Id;
                    var windowCondition = new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, targetPid),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
                    var edgeElement = desktop.FindFirst(TreeScope.Children, windowCondition);
                    if (edgeElement == null)
                    {
                        continue;
                    }
                    PrintControlIdentifiers(context, edgeElement, 0);
                    CancelEdgeDialog(context, edgeElement);
                }
                Task.Delay(1000).Wait();
            }
        }

        [Conditional("DEBUG")]
        internal static void PrintControlIdentifiers(RuntimeContext context, AutomationElement element, int indent)
        {
            try
            {
                string ind = new string(' ', indent * 2);
                Console.WriteLine($"{ind}- {element.Current.ControlType.ProgrammaticName} : {element.Current.Name}");

                var children = element.FindAll(TreeScope.Children, Condition.TrueCondition);
                foreach (AutomationElement child in children)
                {
                    PrintControlIdentifiers(context, child, indent + 1);
                }
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex);
            }
        }

        internal static void CancelEdgeDialog(RuntimeContext context, AutomationElement edgeElement)
        {
            try
            {
                var confirmDialogNameCondition = new OrCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "Resubmit the form?"),
                    new PropertyCondition(AutomationElement.NameProperty, "フォームを再送信しますか?"));
                var confirmDialogCondition = new AndCondition(
                    confirmDialogNameCondition,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
                var confirmDialogElement = edgeElement.FindFirst(TreeScope.Descendants, confirmDialogCondition);
                if (confirmDialogElement == null)
                {
                    return;
                }
                context.Logger.Log($"Found confirmation dialog: {confirmDialogElement.Current.Name}");
                var cancelButtonNameCondition = new OrCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "Cancel"),
                    new PropertyCondition(AutomationElement.NameProperty, "キャンセル"));
                var cancelButtonCondition = new AndCondition(
                    cancelButtonNameCondition,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                var cancelButtonElement = confirmDialogElement.FindFirst(TreeScope.Descendants, cancelButtonCondition);
                if (cancelButtonElement == null)
                {
                    return;
                }
                InvokePattern cancelButton = cancelButtonElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                if (cancelButton == null)
                {
                    return;
                }
                cancelButton.Invoke();
                context.Logger.Log($"Dialog canceled: {confirmDialogElement.Current.Name}");
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex);
            }
        }
    }
}
