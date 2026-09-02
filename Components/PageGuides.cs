namespace RotoMonsterUI
{
    /// <summary>
    /// The page guides, one method per page.
    ///
    /// They live here rather than being pasted into each page so the wording
    /// stays in one place and every page ends up with the same shape: what the
    /// page is for, how to start, the settings you set once, then what to do
    /// while you are using it.
    ///
    /// Each returns the PageGuide rather than the rendered html, so a caller
    /// can still add a section or change the id before rendering.
    /// </summary>
    public static class PageGuides
    {
        /// <summary>
        /// The walkthrough that points at the actual controls, in the order
        /// someone uses them. Pass the same Id you gave DraftMonsterOptions,
        /// since the step targets are built from it.
        ///
        /// Deliberately six steps. A tour long enough to feel like homework
        /// gets skipped, and the guide covers the rest.
        /// </summary>
        public static Tour DraftMonsterTour(string optionsId = "draftMonster",
            string tourId = "draft-monster-tour")
        {
            var settingsId = optionsId + "settings";

            return new Tour(tourId)
                .AddStep("dmconnectblock_" + optionsId,
                    "Start here. Enter the slot you're drafting from and press Connect, "
                    + "then picks come in from your draft room on their own.",
                    TooltipPosition.Bottom)

                .AddStep("dmduring_" + optionsId,
                    "These are the ones you'll touch mid draft. Hide drafted players "
                    + "keeps the board to who is still available.",
                    TooltipPosition.Bottom)

                .AddStep("mspanel_" + settingsId + "_values",
                    "Whose projections to use, and whether to value players per game or "
                    + "across the season. Worth setting once before you draft.",
                    TooltipPosition.Bottom)

                .AddStep("mspanel_" + settingsId + "_standings",
                    "Roto, H2H, or both. The page shows where you'd finish under "
                    + "whichever you pick.",
                    TooltipPosition.Bottom)

                .AddStep("mspositions_" + settingsId,
                    "Filter the board to a position. All turns the filters off.",
                    TooltipPosition.Bottom)

                .AddStep("mscolumns_" + settingsId,
                    "Opens the column picker and the custom value builder. Change these "
                    + "any time without losing your draft.",
                    TooltipPosition.Bottom);
        }

        public static PageGuide DraftMonster(string id = "draft-monster-guide")
        {
            return new PageGuide(id)
                .WithTitle("Draft Monster")
                .WithTourId("draft-monster-tour")
                .WithPurpose(
                    "Draft Monster follows your draft as it happens and works out what "
                    + "each remaining player would do to your team, rather than just "
                    + "ranking them in the abstract.")

                // Open by default - someone reading the guide for the first time
                // almost always wants this one, and the rest are reference.
                .AddSection("Getting started",
                    "<p>Enter the slot you are drafting from at the top and press "
                    + "<strong>Connect</strong>. That is how the page knows when you "
                    + "are up.</p>"
                    + "<p>Picks come in from your draft room on their own, so there is "
                    + "nothing to enter here as the draft goes on.</p>"
                    + "<p>The reversal options only apply if your league uses them. If "
                    + "you are not sure, leave them off.</p>",
                    true)

                .AddSection("How players are valued",
                    "<p><strong>Projections</strong> picks whose numbers to use, and "
                    + "whether to value players per game or across the whole season.</p>"
                    + "<p>Per game favours productivity, total favours players who stay "
                    + "healthy and play often. Worth setting once before you draft and "
                    + "then leaving alone.</p>")

                .AddSection("Punting a category",
                    "<p>If you are giving up on a category, mark it under "
                    + "<strong>Punting</strong>. Everything on the page is then valued "
                    + "as though that category does not count, so a player who is only "
                    + "good at it stops looking good.</p>"
                    + "<p>The weight box next to each one is optional. Leave it blank "
                    + "for a normal punt.</p>")

                .AddSection("Standings",
                    "<p>Choose <strong>Roto</strong>, <strong>H2H</strong>, or both, "
                    + "and the page shows where you would finish under each. Turning "
                    + "both on shows both sets of columns.</p>")

                .AddSection("While you are drafting",
                    "<p><strong>Hide drafted players</strong> keeps the board to who is "
                    + "still available, which is usually what you want mid draft.</p>"
                    + "<p><strong>Highlight drafted since import</strong> marks anyone "
                    + "taken since the last refresh, so you can see what went in the "
                    + "last few picks.</p>")

                .AddSection("Changing the table",
                    "<p><strong>Choose columns</strong> opens the column picker and the "
                    + "custom value builder. The positions row filters the board, and "
                    + "All turns the filters off.</p>"
                    + "<p>These can be changed at any point without losing your "
                    + "draft.</p>");
        }
    }
}