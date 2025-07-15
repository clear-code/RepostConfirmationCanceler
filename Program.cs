using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;

class Program
{
    static void Main()
    {
        AutomationElement desktop = AutomationElement.RootElement;
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
            Console.WriteLine("ウィンドウタイトル: " + edgeElement.Current.Name);

            PrintControlIdentifiers(edgeElement, 0);
            CancelChildConfirmationDialog(edgeElement);
        }
    }

    static void PrintControlIdentifiers(AutomationElement element, int indent)
    {
        try
        {
            string ind = new string(' ', indent * 2);
            Console.WriteLine($"{ind}- {element.Current.ControlType.ProgrammaticName} : {element.Current.Name}");

            // 子要素を再帰的に表示
            var children = element.FindAll(TreeScope.Children, Condition.TrueCondition);
            foreach (AutomationElement child in children)
            {
                PrintControlIdentifiers(child, indent + 1);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    static void CancelChildConfirmationDialog(AutomationElement element)
    {
        try
        {
            var confirmDialogCondition = new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, "フォームを再送信しますか?"),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            var confirmDialogElement = element.FindFirst(TreeScope.Descendants, confirmDialogCondition);
            if (confirmDialogElement == null)
            {
                return;
            }
            var cancelButtonCondition = new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, "キャンセル"), 
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
