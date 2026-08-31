using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class UserLeagues
    {
        private readonly UserLeaguesInput _input;

        public UserLeagues(UserLeaguesInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string RowKey(string prefix, string value)
        {
            return prefix + "_" + _input.Id + "_" + value;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("user-leagues");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "user-leagues-" + _input.Id);

            if (!string.IsNullOrEmpty(_input.Heading))
                wrap.Append(new HtmlTag("h1")
                    .AddClass("user-leagues-heading")
                    .Text(_input.Heading));

            var active = ActiveTab();

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", Key("ultab"))
                .Attr("value", active != null ? active.ProviderName : ""));

            wrap.Append(RenderTabs(active));

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div")
                    .AddClass("user-leagues-message")
                    .Text(_input.Message));

            if (active != null)
                wrap.Append(RenderPanel(active));

            var results = RenderResults();
            if (results != null) wrap.Append(results);

            if (_input.ShowCreateCustom)
                wrap.Append(RenderCreateCustom());

            wrap.Append(new HtmlTag("script").AppendHtml(Script()));

            return wrap.ToString();
        }

        private UserLeaguesTab ActiveTab()
        {
            var tabs = _input.Tabs ?? new List<UserLeaguesTab>();

            if (!string.IsNullOrEmpty(_input.SelectedTab))
            {
                var match = tabs.FirstOrDefault(t => t != null && t.ProviderName == _input.SelectedTab);
                if (match != null) return match;
            }

            return tabs.FirstOrDefault(t => t != null);
        }

        private HtmlTag RenderTabs(UserLeaguesTab active)
        {
            var strip = new HtmlTag("div").AddClass("user-leagues-tabs");

            foreach (var tab in _input.Tabs ?? new List<UserLeaguesTab>())
            {
                if (tab == null) continue;

                var name = RowKey("ultabgo", tab.ProviderName);
                var isActive = active != null && tab.ProviderName == active.ProviderName;

                var button = new HtmlTag("button")
                    .AddClass("user-leagues-tab")
                    .Attr("type", "button")
                    .Attr("name", name)
                    .Attr("onclick", "__doPostBack('" + name + "','',this.form)");

                if (isActive) button.AddClass("user-leagues-tab--active");

                button.Append(new HtmlTag("span")
                    .AddClass("user-leagues-tab-name")
                    .Text(tab.ProviderName));

                button.Append(new HtmlTag("span")
                    .AddClass("user-leagues-tab-count"
                        + (tab.IsConnected || tab.IsCustom ? "" : " user-leagues-tab-count--off"))
                    .Text(TabCountText(tab)));

                strip.Append(button);
            }

            return strip;
        }

        private static string TabCountText(UserLeaguesTab tab)
        {
            var leagues = tab.Leagues ?? new List<UserLeagueRow>();
            var total = leagues.Count;

            if (!tab.IsConnected && !tab.IsCustom)
                return tab.NotConnectedText;

            if (tab.IsCustom || !tab.SupportsBulkImport)
                return total + (total == 1 ? " league" : " leagues");

            var imported = leagues.Count(l => l != null && l.IsImported);
            return imported + " of " + total + " imported";
        }

        private HtmlTag RenderPanel(UserLeaguesTab tab)
        {
            var panel = new HtmlTag("div").AddClass("user-leagues-panel");

            if (!tab.IsConnected && !tab.IsCustom)
            {
                panel.Append(RenderConnect(tab));
                return panel;
            }

            if (!tab.IsCustom)
                panel.Append(RenderConnectionBar(tab));

            if (!string.IsNullOrEmpty(tab.ErrorMessage))
            {
                var error = new HtmlTag("div").AddClass("user-leagues-error").Text(tab.ErrorMessage);

                if (tab.NeedsReauthorization)
                    error.Append(new HtmlTag("div")
                        .Text("You will need to connect " + tab.ProviderName + " again."));

                panel.Append(error);
            }

            var leagues = tab.Leagues ?? new List<UserLeagueRow>();

            if (leagues.Count == 0)
            {
                if (string.IsNullOrEmpty(tab.ErrorMessage))
                    panel.Append(new HtmlTag("p")
                        .AddClass("user-leagues-empty")
                        .Text("No " + tab.ProviderName + " leagues found."));
            }
            else
            {
                panel.Append(RenderLead(tab));
                panel.Append(RenderTable(tab));

                var actions = RenderTableActions(tab);
                if (actions != null) panel.Append(actions);
            }

            if (tab.ShowManualEntry)
                panel.Append(RenderManualEntry(tab));

            return panel;
        }

        private HtmlTag RenderConnect(UserLeaguesTab tab)
        {
            var block = new HtmlTag("div").AddClass("user-leagues-connect");

            block.Append(new HtmlTag("div")
                .AddClass("user-leagues-connect-title")
                .Text(tab.ProviderName + " is not connected"));

            if (!string.IsNullOrEmpty(tab.ConnectLead))
                block.Append(new HtmlTag("p").AddClass("user-leagues-lead").Text(tab.ConnectLead));

            var row = new HtmlTag("div").AddClass("user-leagues-connect-row");

            if (!string.IsNullOrEmpty(tab.ConnectLinkUrl))
            {
                row.Append(new HtmlTag("a")
                    .AddClass("modern-filter-btn modern-filter-btn-secondary")
                    .Attr("href", tab.ConnectLinkUrl)
                    .Attr("target", "_blank")
                    .Text(string.IsNullOrEmpty(tab.ConnectLinkText) ? "Open" : tab.ConnectLinkText));
            }

            foreach (var field in tab.ConnectFields ?? new List<UserLeagueConnectField>())
            {
                if (field == null || string.IsNullOrEmpty(field.FieldName)) continue;

                var fieldId = "ulfield_" + _input.Id + "_" + tab.ProviderName + "_" + field.FieldName;

                row.Append(new HtmlTag("input")
                    .AddClass("user-leagues-input")
                    .Attr("type", field.IsPassword ? "password" : "text")
                    .Attr("id", fieldId)
                    .Attr("name", fieldId)
                    .Attr("placeholder", field.Placeholder));
            }

            var name = RowKey("ulconnect", tab.ProviderName);

            row.Append(new HtmlTag("button")
                .AddClass("modern-filter-btn modern-filter-btn-primary")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(_input.ConnectText));

            block.Append(row);

            if (!string.IsNullOrEmpty(tab.ConnectHelpHtml))
                block.Append(new HtmlTag("div")
                    .AddClass("user-leagues-help")
                    .AppendHtml(tab.ConnectHelpHtml));

            return block;
        }

        private HtmlTag RenderConnectionBar(UserLeaguesTab tab)
        {
            var bar = new HtmlTag("div").AddClass("user-leagues-connection");

            var status = new HtmlTag("span").AddClass("user-leagues-connection-status");
            status.Append(new HtmlTag("span").AddClass("user-leagues-dot"));
            status.Append(new HtmlTag("span").Text(tab.ProviderName + " connected"));
            bar.Append(status);

            var name = RowKey("uldisconnect", tab.ProviderName);
            bar.Append(new HtmlTag("button")
                .AddClass("user-leagues-linkbtn")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(_input.DisconnectText));

            return bar;
        }

        private HtmlTag RenderLead(UserLeaguesTab tab)
        {
            var lead = new HtmlTag("p").AddClass("user-leagues-lead");

            if (tab.IsCustom)
            {
                lead.Text("Leagues you created yourself, or that came from a provider "
                          + "we no longer connect to. They can't be re-imported, but you "
                          + "can still edit or remove them.");
                return lead;
            }

            if (!tab.SupportsBulkImport)
            {
                lead.Text("Your " + tab.ProviderName + " leagues.");
                return lead;
            }

            var selectable = Selectable(tab);

            lead.Text(selectable > 0
                ? "Tick the leagues you want, then choose " + _input.ImportButtonText + "."
                : "All of your " + tab.ProviderName + " leagues are imported.");

            return lead;
        }

        private static int Selectable(UserLeaguesTab tab)
        {
            return (tab.Leagues ?? new List<UserLeagueRow>())
                .Count(l => l != null && !l.IsImported);
        }

        private bool ShowChecks(UserLeaguesTab tab)
        {
            return !tab.IsCustom && tab.SupportsBulkImport && Selectable(tab) > 0;
        }

        private HtmlTag RenderTable(UserLeaguesTab tab)
        {
            var table = new HtmlTag("table").AddClass("user-leagues-table");
            var showChecks = ShowChecks(tab);

            var head = new HtmlTag("thead");
            var headRow = new HtmlTag("tr");

            if (showChecks)
                headRow.Append(new HtmlTag("th").AddClass("user-leagues-check"));

            headRow.Append(new HtmlTag("th").Text("League"));
            headRow.Append(new HtmlTag("th").AddClass("user-leagues-id").Text("League ID"));
            headRow.Append(new HtmlTag("th").Text("Your team"));
            headRow.Append(new HtmlTag("th").AddClass("user-leagues-status").Text("Status"));
            headRow.Append(new HtmlTag("th").AddClass("user-leagues-action").Text("Actions"));

            head.Append(headRow);
            table.Append(head);

            var body = new HtmlTag("tbody");

            foreach (var league in tab.Leagues ?? new List<UserLeagueRow>())
            {
                if (league == null) continue;
                body.Append(RenderRow(tab, league, showChecks));
            }

            table.Append(body);
            return table;
        }

        private HtmlTag RenderRow(UserLeaguesTab tab, UserLeagueRow league, bool showChecks)
        {
            var row = new HtmlTag("tr")
                .AddClass(league.IsImported ? "user-leagues-row--done" : "user-leagues-row--pick");

            if (showChecks)
            {
                var cell = new HtmlTag("td").AddClass("user-leagues-check");

                if (!league.IsImported)
                {
                    var boxId = RowKey("ulpick", league.ProviderLeagueId);

                    cell.Append(new HtmlTag("input")
                        .Attr("type", "checkbox")
                        .Attr("id", boxId)
                        .Attr("name", boxId)
                        .Attr("value", "1")
                        .Attr("aria-label", "Import " + league.Title));
                }

                row.Append(cell);
            }

            var nameCell = new HtmlTag("td").AddClass("user-leagues-name");

            if (league.IsImported && !string.IsNullOrEmpty(league.EditUrl))
                nameCell.Append(new HtmlTag("a").Attr("href", league.EditUrl).Text(league.Title));
            else
                nameCell.Text(league.Title);

            row.Append(nameCell);

            row.Append(MutedOrValue("user-leagues-id", league.ProviderLeagueId, true));
            row.Append(MutedOrValue("user-leagues-team", league.MyTeamTitle, false));

            var statusCell = new HtmlTag("td").AddClass("user-leagues-status");

            if (!league.IsImported)
                statusCell.Append(new HtmlTag("span").AddClass("user-leagues-pill").Text("Available"));
            else if (league.IsTracked)
                statusCell.Append(new HtmlTag("span")
                    .AddClass("user-leagues-pill user-leagues-pill--done").Text("Tracked"));
            else
                statusCell.Append(new HtmlTag("span").AddClass("user-leagues-pill").Text("Not tracked"));

            if (league.NotAtProvider)
                statusCell.Append(new HtmlTag("div")
                    .AddClass("user-leagues-muted")
                    .Text("Not at " + tab.ProviderName));

            row.Append(statusCell);

            var actionCell = new HtmlTag("td").AddClass("user-leagues-action");

            if (league.IsImported)
            {
                actionCell.Append(RowButton("ultrack", league.UserLeagueId,
                    league.IsTracked ? "Untrack" : "Track", false));

                if (!string.IsNullOrEmpty(league.EditUrl))
                    actionCell.Append(new HtmlTag("a")
                        .AddClass("user-leagues-linkbtn")
                        .Attr("href", league.EditUrl)
                        .Text("Edit"));

                actionCell.Append(RowButton("ulremove", league.UserLeagueId, "Remove", true));
            }

            row.Append(actionCell);

            return row;
        }

        private static HtmlTag MutedOrValue(string cssClass, string value, bool mono)
        {
            var cell = new HtmlTag("td").AddClass(cssClass);

            if (string.IsNullOrEmpty(value))
            {
                cell.Append(new HtmlTag("span").AddClass("user-leagues-muted").AppendHtml("&mdash;"));
                return cell;
            }

            if (mono)
                cell.Append(new HtmlTag("span").AddClass("user-leagues-mono").Text(value));
            else
                cell.Text(value);

            return cell;
        }

        private HtmlTag RowButton(string prefix, string id, string text, bool danger)
        {
            var name = RowKey(prefix, id);

            var button = new HtmlTag("button")
                .AddClass("user-leagues-linkbtn")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(text);

            if (danger) button.AddClass("user-leagues-linkbtn--danger");

            return button;
        }

        private HtmlTag RenderTableActions(UserLeaguesTab tab)
        {
            if (!ShowChecks(tab)) return null;

            var selectable = Selectable(tab);
            var actions = new HtmlTag("div").AddClass("user-leagues-actions");

            var name = RowKey("ulimport", tab.ProviderName);

            actions.Append(new HtmlTag("button")
                .AddClass("modern-filter-btn modern-filter-btn-primary")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(_input.ImportButtonText));

            actions.Append(new HtmlTag("button")
                .AddClass("user-leagues-linkbtn user-leagues-selectall")
                .Attr("type", "button")
                .Attr("data-select-text", _input.SelectAllText)
                .Attr("data-clear-text", _input.ClearAllText)
                .Text(_input.SelectAllText));

            actions.Append(new HtmlTag("span")
                .AddClass("user-leagues-hint")
                .Text(selectable + (selectable == 1 ? " league" : " leagues") + " available to import"));

            return actions;
        }

        private HtmlTag RenderManualEntry(UserLeaguesTab tab)
        {
            var block = new HtmlTag("div").AddClass("user-leagues-manual");

            block.Append(new HtmlTag("h6").Text(tab.ManualEntryHeading));

            if (!string.IsNullOrEmpty(tab.ManualEntryHelpHtml))
                block.Append(new HtmlTag("p")
                    .AddClass("user-leagues-muted")
                    .AppendHtml(tab.ManualEntryHelpHtml));

            var row = new HtmlTag("div").AddClass("user-leagues-manual-row");

            var fieldName = "ulmanualid_" + _input.Id + "_" + tab.ProviderName;

            row.Append(new HtmlTag("input")
                .AddClass("user-leagues-input")
                .Attr("type", "text")
                .Attr("id", fieldName)
                .Attr("name", fieldName)
                .Attr("placeholder", tab.ManualEntryPlaceholder));

            var name = RowKey("ulmanual", tab.ProviderName);

            row.Append(new HtmlTag("button")
                .AddClass("modern-filter-btn modern-filter-btn-secondary")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text("Import"));

            block.Append(row);
            return block;
        }

        private HtmlTag RenderResults()
        {
            var results = _input.ImportResults;
            if (results == null || results.Count == 0) return null;

            var panel = new HtmlTag("div").AddClass("user-leagues-panel user-leagues-panel--results");
            panel.Append(new HtmlTag("h5").Text("Import results"));

            var table = new HtmlTag("table").AddClass("user-leagues-table");

            var head = new HtmlTag("thead");
            var headRow = new HtmlTag("tr");
            headRow.Append(new HtmlTag("th").Text("League"));
            headRow.Append(new HtmlTag("th").Text("Result"));
            head.Append(headRow);
            table.Append(head);

            var body = new HtmlTag("tbody");

            foreach (var entry in results)
            {
                if (entry == null) continue;

                var row = new HtmlTag("tr");

                row.Append(new HtmlTag("td")
                    .AddClass("user-leagues-name")
                    .Text(string.IsNullOrEmpty(entry.Title) ? entry.ProviderLeagueId : entry.Title));

                var cell = new HtmlTag("td");

                if (entry.Imported)
                {
                    cell.Append(new HtmlTag("span")
                        .AddClass("user-leagues-pill user-leagues-pill--done").Text("Imported"));

                    if (entry.MissingPlayerCount > 0)
                        cell.Append(new HtmlTag("span")
                            .AddClass("user-leagues-muted")
                            .Text(" " + entry.MissingPlayerCount + " players not matched"));

                    foreach (var warning in entry.Warnings ?? new List<string>())
                        cell.Append(new HtmlTag("div").AddClass("user-leagues-muted").Text(warning));
                }
                else if (entry.Skipped)
                {
                    cell.Append(new HtmlTag("span").AddClass("user-leagues-muted").Text(entry.Message));
                }
                else
                {
                    cell.Append(new HtmlTag("span")
                        .AddClass("user-leagues-pill user-leagues-pill--failed").Text("Failed"));
                    cell.Append(new HtmlTag("div").AddClass("user-leagues-muted").Text(entry.Message));
                }

                row.Append(cell);
                body.Append(row);
            }

            table.Append(body);
            panel.Append(table);

            return panel;
        }

        private HtmlTag RenderCreateCustom()
        {
            var panel = new HtmlTag("div").AddClass("user-leagues-panel");

            panel.Append(new HtmlTag("h5").Text(_input.CreateCustomHeading));
            panel.Append(new HtmlTag("p")
                .AddClass("user-leagues-muted")
                .Text(_input.CreateCustomLead));

            var name = Key("ulcustom");

            panel.Append(new HtmlTag("button")
                .AddClass("modern-filter-btn modern-filter-btn-secondary")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(_input.CreateCustomButtonText));

            return panel;
        }

        private string Script()
        {
            var scope = "#user-leagues-" + _input.Id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    root.querySelectorAll('.user-leagues-row--pick').forEach(function (row) {
        row.addEventListener('click', function (e) {
            var tag = e.target.tagName;
            if (tag === 'INPUT' || tag === 'BUTTON' || tag === 'A' || tag === 'LABEL') return;

            var box = row.querySelector('input[type=checkbox]');
            if (box) box.checked = !box.checked;
        });
    });

    root.querySelectorAll('.user-leagues-selectall').forEach(function (toggle) {
        toggle.addEventListener('click', function () {
            var boxes = root.querySelectorAll('tbody input[type=checkbox]');
            if (!boxes.length) return;

            var selectAll = Array.prototype.some.call(boxes, function (box) {
                return !box.checked;
            });

            boxes.forEach(function (box) { box.checked = selectAll; });
            toggle.textContent = selectAll
                ? toggle.getAttribute('data-clear-text')
                : toggle.getAttribute('data-select-text');
        });
    });
})();
";
        }
    }
}