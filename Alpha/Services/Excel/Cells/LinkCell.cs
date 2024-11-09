﻿using System.Diagnostics.CodeAnalysis;
using Alpha.Gui.Windows;
using Alpha.Utils;
using Hexa.NET.ImGui;
using Lumina.Excel;

namespace Alpha.Services.Excel.Cells;

public class LinkCell : Cell {
    public const string OpenInNewWindow = "Open in new window";

    private AlphaSheet target;
    private int targetRow;
    private int targetCol;
    private string text;
    private string rowColStr;

    [SetsRequiredMembers]
    public LinkCell(int row, int column, object? data, AlphaSheet target, int targetCol) {
        this.Row = row;
        this.Column = column;
        this.Data = data;
        this.target = target;
        this.targetCol = targetCol;
        this.rowColStr = $"{this.Row}_{this.Column}";

        try {
            this.targetRow = Convert.ToInt32(this.Data);
        } catch {
            // ignored
        }

        this.text = $"{this.target.Name}#{this.targetRow}##{this.rowColStr}";
    }

    public override void Draw(ExcelWindow window, bool inAnotherDraw = false) {
        if (inAnotherDraw && Util.IsKeyDown(ImGuiKey.ModAlt)) {
            window.DrawCell(this.target, this.targetRow, this.targetCol, inAnotherDraw: true);
            return;
        }

        if (ImGui.Button(this.text)) {
            window.OpenSheet(this.target, targetRow);
        }

        if (ImGui.BeginPopupContextItem(this.rowColStr)) {
            if (ImGui.MenuItem(OpenInNewWindow)) {
                window.GetExcelService().OpenNewWindow(this.target, this.targetRow);
            }

            ImGui.EndPopup();
        }

        if (ImGui.IsItemHovered()) {
            ImGui.BeginTooltip();
            window.DrawCell(this.target, this.targetRow, this.targetCol, inAnotherDraw: true);
            ImGui.EndTooltip();
        }
    }
}
