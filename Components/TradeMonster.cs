using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class TradeMonster
    {
        private readonly TradeMonsterInput _input;

        public TradeMonster(TradeMonsterInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string SubKey(string prefix, string name)
        {
            return prefix + "_" + _input.Id + "_" + name;
        }

        private static string TaskValue(TradeMonsterTask task)
        {
            switch (task)
            {
                case TradeMonsterTask.FindTrade: return "find";
                case TradeMonsterTask.CheckAddDrop: return "adddrop";
                case TradeMonsterTask.FindFreeAgent: return "findfa";
                default: return "analyze";
            }
        }

        private static string BoardValue(TradeMonsterBoard board)
        {
            switch (board)
            {
                case TradeMonsterBoard.OtherTeam: return "theirs";
                case TradeMonsterBoard.FreeAgents: return "fa";
                default: return "mine";
            }
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("trade-monster");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "trade-monster-" + _input.Id);

            if (!string.IsNullOrEmpty(_input.SettingsHtml))
                wrap.AppendHtml(_input.SettingsHtml);

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", Key("tmcurrent"))
                .Attr("value", TaskValue(_input.SelectedTask)));

            wrap.Append(RenderTasks());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("tm-message").Text(_input.Message));

            wrap.Append(RenderSteps());

            if (_input.ShowSelectionSummary)
                wrap.Append(RenderSummary());

            wrap.Append(RenderBoards());

            if (!string.IsNullOrEmpty(_input.ResultsHtml))
                wrap.Append(new HtmlTag("div")
                    .AddClass("tm-results")
                    .AppendHtml(_input.ResultsHtml));

            return wrap.ToString();
        }

        // ---- task chooser --------------------------------------------------

        private HtmlTag RenderTasks()
        {
            var grid = new HtmlTag("div").AddClass("tm-tasks");

            foreach (var task in _input.Tasks ?? new List<TradeMonsterTaskOption>())
            {
                if (task == null) continue;

                var value = TaskValue(task.Task);
                var name = SubKey("tmtask", value);

                var button = new HtmlTag("button")
                    .AddClass("tm-task")
                    .Attr("type", "button")
                    .Attr("name", name)
                    .Attr("aria-pressed", task.Task == _input.SelectedTask ? "true" : "false")
                    .Attr("onclick", "__doPostBack('" + name + "','',this.form)");

                button.Append(new HtmlTag("span").AddClass("tm-task-name").Text(task.Name));

                if (!string.IsNullOrEmpty(task.Description))
                    button.Append(new HtmlTag("span")
                        .AddClass("tm-task-desc").Text(task.Description));

                grid.Append(button);
            }

            return grid;
        }

        // ---- steps ---------------------------------------------------------

        private HtmlTag RenderSteps()
        {
            var block = new HtmlTag("div").AddClass("tm-steps");

            if (!string.IsNullOrEmpty(_input.StepsHeading))
                block.Append(new HtmlTag("h2").AddClass("tm-steps-heading").Text(_input.StepsHeading));

            var steps = _input.Steps ?? new List<TradeMonsterStep>();

            for (var i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                if (step == null) continue;

                var done = step.IsDone && !step.IsOptional;

                var row = new HtmlTag("div").AddClass("tm-step");
                if (done) row.AddClass("tm-step--done");

                var marker = new HtmlTag("span").AddClass("tm-step-n");
                if (done) marker.AppendHtml("&#10003;");
                else marker.Text((i + 1).ToString());

                row.Append(marker);
                row.Append(new HtmlTag("span").AddClass("tm-step-text").Text(step.Text));

                block.Append(row);
            }

            var go = new HtmlTag("div").AddClass("tm-go");

            var button = new Button(_input.GoButtonText)
                .WithStyle(ButtonStyle.Primary)
                .WithName(Key("tmgo"))
                .WithPostBack();

            var html = button.Render();

            if (!_input.GoEnabled)
                html = html.Replace("<button ", "<button disabled=\"disabled\" ");

            go.AppendHtml(html);
            block.Append(go);

            return block;
        }

        // ---- selection summary ---------------------------------------------

        private HtmlTag RenderSummary()
        {
            var bar = new HtmlTag("div").AddClass("tm-summary");

            bar.Append(new HtmlTag("span").AddClass("tm-summary-label").Text("Selected"));

            bar.Append(Side(_input.LeavingLabel, TradeMonsterBoard.MyTeam));
            bar.Append(Side(_input.JoiningLabel, TradeMonsterBoard.OtherTeam,
                TradeMonsterBoard.FreeAgents));

            bar.AppendHtml(new Button(_input.ClearButtonText)
                .WithStyle(ButtonStyle.Secondary)
                .WithName(Key("tmclear"))
                .WithPostBack()
                .Render());

            return bar;
        }

        private HtmlTag Side(string label, params TradeMonsterBoard[] boards)
        {
            var names = new List<string>();

            foreach (var board in _input.Boards ?? new List<TradeMonsterBoardInput>())
            {
                if (board == null) continue;
                if (!boards.Contains(board.Board)) continue;

                foreach (var player in board.Players ?? new List<TradeMonsterPlayer>())
                {
                    if (player == null || !player.IsSelected) continue;
                    if (player.DisplayPlayerInput == null) continue;

                    names.Add(player.DisplayPlayerInput.PlayerName);
                }
            }

            var side = new HtmlTag("span").AddClass("tm-summary-side");
            side.Append(new HtmlTag("span").Text(label + ": "));

            var value = new HtmlTag("b");

            if (names.Count == 0)
            {
                value.AddClass("tm-summary-none").Text(_input.NothingSelectedText);
            }
            else
            {
                value.Text(string.Join(", ", names));
            }

            side.Append(value);
            return side;
        }

        // ---- boards ---------------------------------------------------------

        private HtmlTag RenderBoards()
        {
            var grid = new HtmlTag("div").AddClass("tm-boards");

            foreach (var board in _input.Boards ?? new List<TradeMonsterBoardInput>())
            {
                if (board == null) continue;
                grid.Append(RenderBoard(board));
            }

            return grid;
        }

        private HtmlTag RenderBoard(TradeMonsterBoardInput board)
        {
            var slug = BoardValue(board.Board);
            var panel = new HtmlTag("div").AddClass("tm-board");

            var head = new HtmlTag("div").AddClass("tm-board-head");

            head.Append(new HtmlTag("span").AddClass("tm-board-title").Text(board.Title));

            if (board.TeamOptions != null && board.TeamOptions.Count > 0)
            {
                var dropdown = new Dropdown(board.TeamPlaceholder)
                    .WithName(SubKey("tmteam", slug));

                foreach (var option in board.TeamOptions)
                {
                    if (option == null) continue;
                    dropdown.AddItem(option.Text, option.Value);
                }

                if (!string.IsNullOrEmpty(board.SelectedTeamValue))
                    dropdown.WithSelectedValue(board.SelectedTeamValue);

                head.AppendHtml(dropdown.Render());
            }

            if (!string.IsNullOrEmpty(board.CountText))
                head.Append(new HtmlTag("span").AddClass("tm-board-count").Text(board.CountText));

            panel.Append(head);

            var players = board.Players ?? new List<TradeMonsterPlayer>();

            if (players.Count == 0)
            {
                panel.Append(new HtmlTag("div")
                    .AddClass("tm-board-empty")
                    .Text(string.IsNullOrEmpty(board.EmptyText)
                        ? "No players to show."
                        : board.EmptyText));

                return panel;
            }

            var table = new HtmlTag("table").AddClass("tm-table");

            var thead = new HtmlTag("thead");
            var headRow = new HtmlTag("tr");
            headRow.Append(new HtmlTag("th").AddClass("tm-act-col"));
            headRow.Append(new HtmlTag("th").Text("Player"));
            headRow.Append(new HtmlTag("th").AddClass("tm-num").Text("Value"));
            headRow.Append(new HtmlTag("th").AddClass("tm-num").Text("g"));
            thead.Append(headRow);
            table.Append(thead);

            var body = new HtmlTag("tbody");

            foreach (var player in players)
            {
                if (player == null) continue;
                body.Append(RenderPlayer(board, slug, player));
            }

            table.Append(body);
            panel.Append(table);

            return panel;
        }

        private HtmlTag RenderPlayer(TradeMonsterBoardInput board, string slug, TradeMonsterPlayer player)
        {
            var row = new HtmlTag("tr").AddClass("tm-row");

            if (!string.IsNullOrEmpty(board.ActionColorCSS))
                row.Attr("style", "--tm-act-color:" + board.ActionColorCSS + ";");

            var actCell = new HtmlTag("td").AddClass("tm-act-col");

            var boxId = SubKey("tmpick", slug + "_" + player.PlayerId);

            var label = new HtmlTag("label").AddClass("tm-act").Attr("for", boxId);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", boxId)
                .Attr("name", boxId)
                .Attr("value", "1");

            if (player.IsSelected) box.Attr("checked", "checked");

            label.Append(box);

            // both labels render, css shows the right one, so ticking the box
            // updates the wording with no postback and no script
            label.Append(new HtmlTag("span")
                .AddClass("tm-act-text tm-act-off")
                .Text(board.ActionText));

            label.Append(new HtmlTag("span")
                .AddClass("tm-act-text tm-act-on")
                .Text(board.SelectedActionText ?? board.ActionText));

            actCell.Append(label);
            row.Append(actCell);

            var nameCell = new HtmlTag("td").AddClass("tm-player");

            if (player.DisplayPlayerInput != null)
                nameCell.AppendHtml(new DisplayPlayer(player.DisplayPlayerInput).Render());

            if (player.InjuryBadge != null)
                nameCell.AppendHtml(new InjuryBadge(player.InjuryBadge).Render());

            if (!string.IsNullOrEmpty(player.MonsterBarHtml))
                nameCell.AppendHtml(player.MonsterBarHtml);

            row.Append(nameCell);

            row.Append(new HtmlTag("td").AddClass("tm-num").Text(player.ValueText ?? ""));
            row.Append(new HtmlTag("td").AddClass("tm-num").Text(player.GamesText ?? ""));

            return row;
        }
    }
}