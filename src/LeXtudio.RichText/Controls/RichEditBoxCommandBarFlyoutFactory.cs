// Default CommandBarFlyout factory for RichEditBox.SelectionFlyout /
// ContextFlyout. Mirrors WinUI Gallery's TextCommandBarFlyout shape:
//   primary strip: Bold / Italic / Underline (icon-over-label toggle buttons)
//   primary tail: native "..." overflow chevron (provided by CommandBarFlyout)
//   secondary list: Cut, Copy, Paste, Undo, Redo, Select All
//
// Users can extend either band the same way the WinUI Gallery sample does:
//   var f = (CommandBarFlyout)editor.ContextFlyout;
//   f.Opening += (s, e) => f.PrimaryCommands.Add(new AppBarButton { ... });
//
// State (IsChecked toggles, IsEnabled commands, Visibility for inapplicable
// secondary items) refreshes on Opening from the live document selection.

using System;
using System.Collections.Generic;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace LeXtudio.UI.Xaml.Controls;

internal static class RichEditBoxCommandBarFlyoutFactory
{
    // Sentinel tag attached to a marker FrameworkElement we install on the
    // CommandBarFlyout's overflow content. The RichEditBox uses it during
    // popup-sweep to recognize our flyout's own popup and skip closing it.
    internal const string OwnPopupSentinel = "LeXtudio.RichEditBox.OwnFlyoutRoot";

    // Uno desktop's FontIcon does not resolve Segoe MDL2 Assets glyph code
    // points reliably (the icon renders blank), but SymbolIcon — backed by
    // WinUI's Symbol enum — works because the framework ships the assets
    // for it. The Symbol enum already exposes every command icon we need.

    public static CommandBarFlyout Create(RichEditBox owner)
    {
        var flyout = new CommandBarFlyout
        {
            Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
            // Do NOT set AlwaysExpanded=true: that hides the "..." overflow
            // chevron. Right-click invocation opens the flyout in expanded
            // state automatically (both primary strip and secondary list
            // visible), and the chevron stays available so the user can
            // collapse back to just the primary strip if they want — exactly
            // like the WinUI Gallery RichEditBox.
            ShowMode = FlyoutShowMode.Standard,
        };

        var refreshers = new List<Action>();

        var bold = BuildToggle("Bold", Symbol.Bold, "Ctrl+B",
            isChecked: () => owner.Document.Selection.CharacterFormat.Bold == FormatEffect.On,
            isEnabled: () => !owner.IsReadOnly,
            action: owner.ToggleSelectionBold,
            refreshers: refreshers);

        var italic = BuildToggle("Italic", Symbol.Italic, "Ctrl+I",
            isChecked: () => owner.Document.Selection.CharacterFormat.Italic == FormatEffect.On,
            isEnabled: () => !owner.IsReadOnly,
            action: owner.ToggleSelectionItalic,
            refreshers: refreshers);

        var underline = BuildToggle("Underline", Symbol.Underline, "Ctrl+U",
            isChecked: () => owner.Document.Selection.CharacterFormat.Underline != UnderlineType.None,
            isEnabled: () => !owner.IsReadOnly,
            action: owner.ToggleSelectionUnderline,
            refreshers: refreshers);

        flyout.PrimaryCommands.Add(bold);
        flyout.PrimaryCommands.Add(italic);
        flyout.PrimaryCommands.Add(underline);

        var cut = BuildAction("Cut", Symbol.Cut, "Ctrl+X",
            isEnabled: () => !owner.IsReadOnly && owner.Document.Selection.Length > 0,
            action: owner.CutSelection,
            refreshers: refreshers);

        var copy = BuildAction("Copy", Symbol.Copy, "Ctrl+C",
            isEnabled: () => owner.Document.Selection.Length > 0,
            action: owner.CopySelection,
            refreshers: refreshers);

        var paste = BuildAction("Paste", Symbol.Paste, "Ctrl+V",
            isEnabled: () => !owner.IsReadOnly && owner.Document.CanPaste(),
            action: owner.PasteFromClipboard,
            refreshers: refreshers);

        var undo = BuildAction("Undo", Symbol.Undo, "Ctrl+Z",
            isEnabled: () => !owner.IsReadOnly && owner.Document.CanUndo(),
            action: () => owner.Document.Undo(),
            refreshers: refreshers);

        var redo = BuildAction("Redo", Symbol.Redo, "Ctrl+Y",
            isEnabled: () => !owner.IsReadOnly && owner.Document.CanRedo(),
            action: () => owner.Document.Redo(),
            refreshers: refreshers);

        // Select All stays visible regardless of content (WinUI parity). On
        // an empty box it is a no-op, but it remains in the list so users
        // know the command is part of the editor's surface.
        var selectAll = BuildAction("Select All", symbol: null, accelerator: "Ctrl+A",
            isEnabled: () => true,
            action: owner.SelectAll,
            refreshers: refreshers);

        flyout.SecondaryCommands.Add(cut);
        flyout.SecondaryCommands.Add(copy);
        flyout.SecondaryCommands.Add(paste);
        flyout.SecondaryCommands.Add(undo);
        flyout.SecondaryCommands.Add(redo);
        flyout.SecondaryCommands.Add(selectAll);

        flyout.Opening += (_, _) =>
        {
            foreach (var r in refreshers) r();
            LogDiagnostic($"Opening primary={flyout.PrimaryCommands.Count} secondary={flyout.SecondaryCommands.Count} readOnly={owner.IsReadOnly} selLen={owner.Document.Selection.Length}");
        };
        flyout.Opened += (_, _) => LogDiagnostic("Opened");
        flyout.Closed += (_, _) => LogDiagnostic("Closed");

        return flyout;
    }

    private static AppBarToggleButton BuildToggle(
        string label,
        Symbol symbol,
        string accelerator,
        Func<bool> isChecked,
        Func<bool> isEnabled,
        Action action,
        List<Action> refreshers)
    {
        var button = new AppBarToggleButton
        {
            Label = label,
            Icon = new SymbolIcon(symbol),
            KeyboardAcceleratorTextOverride = accelerator,
        };
        button.Click += (_, _) =>
        {
            action();
            foreach (var r in refreshers) r();
        };
        refreshers.Add(() =>
        {
            button.IsChecked = isChecked();
            button.IsEnabled = isEnabled();
        });
        return button;
    }

    private static AppBarButton BuildAction(
        string label,
        Symbol? symbol,
        string accelerator,
        Func<bool> isEnabled,
        Action action,
        List<Action> refreshers)
    {
        var button = new AppBarButton
        {
            Label = label,
            KeyboardAcceleratorTextOverride = accelerator,
        };
        if (symbol is Symbol s)
        {
            button.Icon = new SymbolIcon(s);
        }
        button.Click += (_, _) => action();
        refreshers.Add(() =>
        {
            bool enabled = isEnabled();
            button.IsEnabled = enabled;
            // Hide inapplicable secondary commands rather than dimming them,
            // matching how WinUI's gallery flyout shows only actionable rows.
            button.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        });
        return button;
    }

    private static readonly string s_diagnosticLogPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "LeXtudio.RichText.Flyout.log");

    private static void LogDiagnostic(string message)
    {
        try
        {
            System.IO.File.AppendAllText(s_diagnosticLogPath, $"{System.DateTimeOffset.Now:O} {message}{System.Environment.NewLine}");
        }
        catch
        {
        }
    }
}
