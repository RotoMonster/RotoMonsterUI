using System.Collections.Generic;
using System.Text;
using HtmlTags;

namespace RotoMonsterUI
{
    public class GiveGetPlayerPicker
    {
        private readonly GiveGetPlayerPickerInput _input;

        public GiveGetPlayerPicker(GiveGetPlayerPickerInput input)
        {
            _input = input;
        }

        private string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private string SerializeAliasesJson(List<string> aliases)
        {
            if (aliases == null || aliases.Count == 0) return "[]";

            var sb = new StringBuilder("[");
            var wrote = 0;
            for (int i = 0; i < aliases.Count; i++)
            {
                if (string.IsNullOrEmpty(aliases[i])) continue;
                if (wrote > 0) sb.Append(",");
                sb.Append($"\"{EscapeJson(aliases[i])}\"");
                wrote++;
            }
            sb.Append("]");
            return sb.ToString();
        }

        private string SerializePlayersJson(List<DisplayPlayerInput> players)
        {
            var sb = new StringBuilder("[");
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    var p = players[i];
                    if (p == null) continue;
                    if (i > 0) sb.Append(",");
                    var posText = p.Positions != null && p.Positions.Count > 0
                        ? string.Join("/", p.Positions.ConvertAll(x => x.Abbreviation))
                        : "";
                    sb.Append("{");
                    sb.Append($"\"id\":{p.PlayerId},");
                    sb.Append($"\"name\":\"{EscapeJson(p.PlayerName)}\",");
                    sb.Append($"\"team\":\"{EscapeJson(p.TeamCode)}\",");
                    sb.Append($"\"pos\":\"{EscapeJson(posText)}\",");
                    sb.Append($"\"aliases\":{SerializeAliasesJson(p.Aliases)}");
                    sb.Append("}");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("give-get-picker");

            var columns = new HtmlTag("div").AddClass("give-get-picker-columns");
            columns.Append(RenderColumn("Give players", $"{_input.Id}-give", _input.GivePlayers, _input.AvailableGivePlayers, _input.GiveSearchPlaceholder));
            columns.Append(RenderColumn("Get players", $"{_input.Id}-get", _input.GetPlayers, _input.AvailableGetPlayers, _input.GetSearchPlaceholder));
            wrapper.Append(columns);

            if (_input.CanEdit)
            {
                var swapRow = new HtmlTag("div").AddClass("give-get-picker-swap-row");
                var swapBtn = new HtmlTag("button")
                    .AddClass("give-get-picker-swap-btn")
                    .Attr("type", "submit")
                    .Attr("name", $"{_input.Id}-swap");
                swapBtn.AppendHtml(new Icon(new IconInput { Type = IconType.RefreshRosters, Size = 14 }).Render());
                swapBtn.AppendHtml("Swap");
                swapRow.Append(swapBtn);
                wrapper.Append(swapRow);
            }

            return wrapper.ToString();
        }

        // Reused for both the Give and Get sides - only the label, id prefix, player lists, and
        // placeholder differ between them.
        private HtmlTag RenderColumn(string label, string pickerId, List<DisplayPlayerInput> players, List<DisplayPlayerInput> availablePlayers, string placeholder)
        {
            var column = new HtmlTag("div").AddClass("give-get-picker-column");

            var header = new HtmlTag("div").AddClass("give-get-picker-column-header");
            header.AppendHtml($"<span>{label} ({players.Count})</span>");
            if (_input.CanEdit && players.Count > 0)
            {
                var clearBtn = new HtmlTag("button")
                    .AddClass("give-get-picker-clear-btn")
                    .Attr("type", "submit")
                    .Attr("name", $"{pickerId}-clear")
                    .Text("Clear");
                header.Append(clearBtn);
            }
            column.Append(header);

            if (_input.CanEdit)
            {
                var searchRow = new HtmlTag("div").AddClass("give-get-picker-search-row");

                var searchBox = new SearchBox()
                    .WithId($"{pickerId}-search")
                    .WithName($"{pickerId}-search")
                    .WithPlaceholder(placeholder)
                    .Render();

                var hiddenSelected = new HtmlTag("input")
                    .Attr("type", "hidden")
                    .Attr("id", $"{pickerId}-selected")
                    .Attr("name", $"{pickerId}-selected");

                var addBtn = new HtmlTag("button")
                    .AddClass("modern-filter-btn modern-filter-btn-secondary")
                    .Attr("type", "submit")
                    .Attr("name", $"{pickerId}-add")
                    .Text("Add");

                var resultsList = new HtmlTag("ul")
                    .AddClass("completionList give-get-picker-results")
                    .Attr("id", $"{pickerId}-results")
                    .Attr("style", "display:none;");

                searchRow.AppendHtml(searchBox);
                searchRow.Append(hiddenSelected);
                searchRow.Append(addBtn);
                searchRow.Append(resultsList);
                column.Append(searchRow);

                column.AppendHtml($"<script type=\"application/json\" id=\"{pickerId}-data\">{SerializePlayersJson(availablePlayers)}</script>");
            }

            if (players.Count > 0)
            {
                var list = new HtmlTag("div").AddClass("give-get-picker-list");
                foreach (var player in players)
                {
                    var chip = new HtmlTag("div").AddClass("give-get-picker-chip");
                    chip.AppendHtml(new DisplayPlayer(player).Render());

                    if (_input.CanEdit)
                    {
                        var removeBtn = new HtmlTag("button")
                            .AddClass("give-get-picker-remove")
                            .Attr("type", "submit")
                            .Attr("name", $"{pickerId}-remove-{player.PlayerId}")
                            .Attr("aria-label", "Remove player")
                            .Text("\u00d7");
                        chip.Append(removeBtn);
                    }

                    list.Append(chip);
                }
                column.Append(list);
            }

            return column;
        }
    }
}