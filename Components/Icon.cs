namespace RotoMonsterUI
{
    public class Icon
    {
        private IconInput _input;

        public Icon(IconInput input)
        {
            _input = input;
        }

        public Icon WithSize(int size) { _input.Size = size; return this; }
        public Icon WithColor(string color) { _input.Color = color; return this; }

        private string WrapSvg(string innerPath)
        {
            return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Fill}"" stroke=""{_input.Color}"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"">{innerPath}</svg>";
        }

        public string Render()
        {
            switch (_input.Type)
            {
                case IconType.PersonSimple:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 60 60"">
                        <circle cx=""30"" cy=""10"" r=""8"" fill=""{_input.Color}""/>
                        <line x1=""30"" y1=""18"" x2=""30"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""16"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""44"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""20"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""40"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                    </svg>";

                case IconType.PersonAlert:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 60 60"">
                        <circle cx=""26"" cy=""10"" r=""8"" fill=""{_input.Color}""/>
                        <line x1=""26"" y1=""18"" x2=""26"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""27"" x2=""12"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""27"" x2=""40"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""40"" x2=""16"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""40"" x2=""36"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <circle cx=""48"" cy=""48"" r=""11"" fill=""{_input.Color}"" stroke=""white"" stroke-width=""2.5""/>
                        <text x=""48"" y=""53"" text-anchor=""middle"" font-size=""13"" font-weight=""900"" fill=""white"" font-family=""sans-serif"">!</text>
                    </svg>";

                case IconType.PersonConfirmed:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 60 60"">
                        <circle cx=""26"" cy=""10"" r=""8"" fill=""{_input.Color}""/>
                        <line x1=""26"" y1=""18"" x2=""26"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""27"" x2=""12"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""27"" x2=""40"" y2=""36"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""40"" x2=""16"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""26"" y1=""40"" x2=""36"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <circle cx=""48"" cy=""48"" r=""11"" fill=""{_input.Color}"" stroke=""white"" stroke-width=""2.5""/>
                        <text x=""48"" y=""54"" text-anchor=""middle"" font-size=""13"" font-weight=""900"" fill=""white"" font-family=""sans-serif"">✓</text>
                    </svg>";

                case IconType.PersonArmsDown:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 60 60"">
                        <circle cx=""30"" cy=""10"" r=""8"" fill=""{_input.Color}""/>
                        <line x1=""30"" y1=""18"" x2=""30"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""16"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""44"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""20"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""40"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                    </svg>";

                case IconType.PersonArmsUp:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 60 60"">
                        <circle cx=""30"" cy=""10"" r=""8"" fill=""{_input.Color}""/>
                        <line x1=""30"" y1=""18"" x2=""30"" y2=""40"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""16"" y2=""14"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""27"" x2=""44"" y2=""14"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""20"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                        <line x1=""30"" y1=""40"" x2=""40"" y2=""55"" stroke=""{_input.Color}"" stroke-width=""4"" stroke-linecap=""round""/>
                    </svg>";
                case IconType.Settings:
                    return WrapSvg(@"<path d=""M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z""/><circle cx=""12"" cy=""12"" r=""3""/>");
                case IconType.RefreshRosters:
                    return WrapSvg(@"<path d=""M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8""/><path d=""M21 3v5h-5""/><path d=""M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16""/><path d=""M8 16H3v5""/>");
                case IconType.PostponementChanceWarning:
                    return WrapSvg(@"<path d=""M4 15s1-1 4-1 5 2 8 2 4-1 4-1V3s-1 1-4 1-5-2-8-2-4 1-4 1z""/><line x1=""4"" y1=""22"" x2=""4"" y2=""15""/>");
                case IconType.Info:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""10""/><path d=""M12 16v-4""/><path d=""M12 8h.01""/>");
               case IconType.Dome:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""none"" stroke=""{_input.Color}"" stroke-width=""2.5"" stroke-linecap=""round"" stroke-linejoin=""round"">
                        <path d=""M2 22 L2 14 A10 8 0 0 1 22 14 L22 22 L2 22"" fill=""{_input.Fill}"" stroke=""{_input.Color}""/>
                    </svg>";
                case IconType.Trash:
                    return WrapSvg(@"<polyline points=""3 6 5 6 21 6""/><path d=""M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6""/><path d=""M10 11v6""/><path d=""M14 11v6""/><path d=""M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2""/>");
                case IconType.Next:
                    return WrapSvg(@"<polyline points=""9 18 15 12 9 6""/>");
                case IconType.Previous:
                    return WrapSvg(@"<polyline points=""15 18 9 12 15 6""/>");
                case IconType.LineupCard:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"">
                        <rect x=""2"" y=""1"" width=""20"" height=""22"" rx=""4"" fill=""{_input.Color}""/>
                        <line x1=""6"" y1=""7"" x2=""18"" y2=""7"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round""/>
                        <line x1=""6"" y1=""11"" x2=""18"" y2=""11"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round""/>
                        <line x1=""6"" y1=""15"" x2=""18"" y2=""15"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round""/>
                        <line x1=""6"" y1=""19"" x2=""13"" y2=""19"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round""/>
                    </svg>";
                case IconType.Rain:
                    return WrapSvg(@"<path d=""M4 14 C4 10 7 8 12 8 C17 8 20 10 20 14"" stroke-linecap=""round""/><line x1=""8"" y1=""18"" x2=""7"" y2=""21"" stroke-linecap=""round""/><line x1=""12"" y1=""18"" x2=""11"" y2=""21"" stroke-linecap=""round""/><line x1=""16"" y1=""18"" x2=""15"" y2=""21"" stroke-linecap=""round""/><path d=""M4 14 L20 14"" stroke-linecap=""round""/>");
                case IconType.LineupConfirmed:
                    return WrapSvg(@"<line x1=""10"" y1=""6"" x2=""21"" y2=""6""/><line x1=""10"" y1=""12"" x2=""21"" y2=""12""/><line x1=""10"" y1=""18"" x2=""21"" y2=""18""/><path d=""M4 6h1v4""/><path d=""M4 10h2""/><path d=""M6 18H4c0-1 2-2 2-3s-1-1.5-2-1""/>");
                case IconType.LineupNotConfirmed:
                    return WrapSvg(@"<line x1=""10"" y1=""6"" x2=""21"" y2=""6""/><line x1=""10"" y1=""12"" x2=""21"" y2=""12""/><line x1=""10"" y1=""18"" x2=""21"" y2=""18""/><path d=""M4 6h1v4""/><path d=""M4 10h2""/><path d=""M6 18H4c0-1 2-2 2-3s-1-1.5-2-1""/>");
                case IconType.Weather:
                    return WrapSvg(@"<path d=""M14 14.76V3.5a2.5 2.5 0 0 0-5 0v11.26a4.5 4.5 0 1 0 5 0z""/><circle cx=""11.5"" cy=""17.5"" r=""2.5"" fill=""#378ADD""/><line x1=""11.5"" y1=""14"" x2=""11.5"" y2=""8"" stroke-width=""2.5""/>");
                case IconType.ExportCSV:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""none"" stroke=""{_input.Color}"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"">
                        <path d=""M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z""/>
                        <polyline points=""14 2 14 8 20 8""/>
                        <text x=""12"" y=""17"" text-anchor=""middle"" font-size=""5.5"" font-weight=""bold"" stroke=""none"" fill=""{_input.Color}"" font-family=""system-ui"">CSV</text>
                    </svg>";

                case IconType.ExportExcel:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""none"" stroke=""{_input.Color}"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"">
                        <path d=""M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z""/>
                        <polyline points=""14 2 14 8 20 8""/>
                        <text x=""12"" y=""17"" text-anchor=""middle"" font-size=""5.5"" font-weight=""bold"" stroke=""none"" fill=""{_input.Color}"" font-family=""system-ui"">XLS</text>
                    </svg>";
                case IconType.Calendar:
                    return WrapSvg(@"<rect x=""3"" y=""4"" width=""18"" height=""18"" rx=""2""/><line x1=""3"" y1=""9"" x2=""21"" y2=""9""/><line x1=""8"" y1=""2"" x2=""8"" y2=""6""/><line x1=""16"" y1=""2"" x2=""16"" y2=""6""/><line x1=""7"" y1=""13"" x2=""9"" y2=""13""/><line x1=""12"" y1=""13"" x2=""14"" y2=""13""/><line x1=""17"" y1=""13"" x2=""19"" y2=""13""/><line x1=""7"" y1=""17"" x2=""9"" y2=""17""/><line x1=""12"" y1=""17"" x2=""14"" y2=""17""/>");
                case IconType.Save:
                    return WrapSvg(@"<path d=""M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z""/><polyline points=""17 21 17 13 7 13 7 21""/><polyline points=""7 3 7 8 15 8""/>");
                case IconType.RetractableDome:
                    return WrapSvg(@"<line x1=""4"" y1=""16"" x2=""20"" y2=""16""/><line x1=""4"" y1=""16"" x2=""4"" y2=""11""/><line x1=""20"" y1=""16"" x2=""20"" y2=""11""/><path d=""M4 11 A9 9 0 0 1 8.5 4.5""/><path d=""M20 11 A9 9 0 0 0 15.5 4.5""/><path d=""M7 11 A6 6 0 0 1 10 7"" stroke-width=""1""/><path d=""M17 11 A6 6 0 0 0 14 7"" stroke-width=""1""/>");
                case IconType.Injury:
                    return WrapSvg(@"<g transform=""rotate(45 12 12)""><path d=""M 7.5 4 L 16.5 4""/><path d=""M 9.5 4 L 9.5 12 L 12 15 L 12 21""/><path d=""M 14.5 4 L 14.5 12 L 12 15""/><path d=""M 9.5 8.5 L 14.5 8.5""/></g>");
                case IconType.Illness:
                    return WrapSvg(@"<rect x=""7"" y=""9"" width=""10"" height=""12"" rx=""2""/><rect x=""9"" y=""4"" width=""6"" height=""5"" rx=""1""/><path d=""M9 6h6""/><path d=""M7 14h10""/>");
                case IconType.Rest:
                    return WrapSvg(@"<path d=""M2 19v-6""/><path d=""M2 13h16a3 3 0 0 1 3 3v3""/><path d=""M2 17h19""/><rect x=""4"" y=""9"" width=""6"" height=""4"" rx=""1""/><path d=""M2 19v1""/><path d=""M21 19v1""/>");
                case IconType.Personal:
                    return WrapSvg(@"<path d=""M3 10.5 12 3l9 7.5""/><path d=""M5 9.5V20a1 1 0 0 0 1 1h4v-6h4v6h4a1 1 0 0 0 1-1V9.5""/>");
                case IconType.CoachsDecision:
                    return WrapSvg(@"<rect x=""6"" y=""4"" width=""12"" height=""17"" rx=""2""/><rect x=""9"" y=""2"" width=""6"" height=""4"" rx=""1""/><path d=""M9 11h6""/><path d=""M9 15h6""/>");
                case IconType.Dental:
                    return WrapSvg(@"<path d=""M12 21c-.6 0-1-1.4-1.4-3.4-.3-1.6-.6-2.3-1.1-2.3s-.8.7-1.1 2.3C8 19.6 7.6 21 7 21c-1.8 0-2.7-2.7-3.2-5.8C3.4 12.6 3 9.9 3 8c0-2.8 1.9-4.3 4.2-4.3 1 0 1.9.5 2.8 1 .9-.5 1.8-1 2.8-1C15.1 3.7 17 5.2 17 8c0 1.9-.4 4.6-.8 7.2-.5 3.1-1.4 5.8-3.2 5.8-.6 0-1-1.4-1.3-3-.1-.6-.3-1.1-.5-1.4""/>");
                case IconType.PossibleSuspension:
                    return WrapSvg(@"<rect x=""5"" y=""11"" width=""14"" height=""10"" rx=""2""/><path d=""M8 11V7a4 4 0 0 1 8 0v4""/>");
                case IconType.Other:
                    return WrapSvg(@"<path d=""M12 3l8 4.5v9L12 21l-8-4.5v-9L12 3z""/><path d=""M12 12v9""/><path d=""M12 12l8-4.5""/><path d=""M12 12l-8-4.5""/>");
                case IconType.TradePending:
                    return WrapSvg(@"<path d=""M3 6h18""/><path d=""M17 2l4 4-4 4""/><path d=""M21 18H3""/><path d=""M7 14l-4 4 4 4""/>");
                case IconType.Contract:
                    return WrapSvg(@"<path d=""M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8l-5-5z""/><path d=""M14 3v5h5""/><path d=""M9 13h6""/><path d=""M9 17h4""/>");
                case IconType.InjuryMaintenance:
                    return WrapSvg(@"<rect x=""3"" y=""8"" width=""18"" height=""12"" rx=""2""/><path d=""M8 8V6a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2""/><path d=""M12 12v4""/><path d=""M10 14h4""/>");
                case IconType.Warning:
                    return WrapSvg(@"<path d=""M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z""/><path d=""M12 9v4""/><path d=""M12 17h.01""/>");
                case IconType.Error:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""10""/><path d=""M15 9l-6 6""/><path d=""M9 9l6 6""/>");
                case IconType.GameTimeDecision:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""9""/><path d=""M12 7v5l3 3""/>");
                case IconType.Practiced:
                case IconType.MadeShootaround:
                    return WrapSvg(@"<rect x=""4"" y=""4"" width=""16"" height=""16"" rx=""3""/><path d=""M12 15V9""/><path d=""M8.5 12.5 12 9l3.5 3.5""/>");
                case IconType.MissedPractice:
                case IconType.MissedShootaround:
                    return WrapSvg(@"<rect x=""4"" y=""4"" width=""16"" height=""16"" rx=""3""/><path d=""M12 9v6""/><path d=""M8.5 11.5 12 15l3.5-3.5""/>");
                case IconType.Injured:
                    // Source: Font Awesome Free "user-injured", CC BY 4.0 - https://fontawesome.com/license/free
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 640 640"" fill=""{_input.Color}"">
                        <path d=""M338.7 144L430 144C419.3 119.4 400.5 99.1 377.1 86.4L338.7 144zM337.8 73.3C332 72.4 326 72 320 72C270.8 72 228.5 101.6 210 144L290.6 144L337.7 73.3zM320 312C386.3 312 440 258.3 440 192L200 192C200 258.3 253.7 312 320 312zM194.7 405.8C145.3 434.2 112 487.5 112 548.6C112 563.7 124.3 576 139.4 576L290.4 576L194.6 405.8zM239.8 388.1L282.5 464L368 464C412.2 464 448 499.8 448 544C448 555.4 445.6 566.2 441.3 576L500.5 576C515.6 576 527.9 563.7 527.9 548.6C527.9 457.7 454.2 384 363.3 384L276.4 384C263.8 384 251.5 385.4 239.7 388.1zM309.5 512L345.5 576L368 576C385.7 576 400 561.7 400 544C400 526.3 385.7 512 368 512L309.5 512z""/>
                    </svg>";
                case IconType.Suspended:
                    return WrapSvg(@"<rect x=""5"" y=""11"" width=""14"" height=""10"" rx=""2""/><path d=""M8 11V7a4 4 0 0 1 8 0v4""/><circle cx=""12"" cy=""15"" r=""1.3""/><path d=""M12 16.3v1.7""/>");
                case IconType.OutForSeason:
                    return WrapSvg(@"<path d=""M8 3h8l5 5v8l-5 5H8l-5-5V8l5-5z""/><path d=""M9 9l6 6""/><path d=""M15 9l-6 6""/>");
                case IconType.NewContract:
                    return WrapSvg(@"<path d=""M8 6c-1.2 2-2 4.2-2 7a6 6 0 0 0 12 0c0-2.8-.8-5-2-7""/><path d=""M9 3h6l-1.5 3h-3z""/><path d=""M12 9.5v6""/><path d=""M10.3 11c0-1 .8-1.5 1.7-1.5s1.7.5 1.7 1.5-.8 1.5-1.7 1.5-1.7.5-1.7 1.5.8 1.5 1.7 1.5 1.7-.5 1.7-1.5""/>");
                case IconType.FreeAgent:
                    // Source: Font Awesome Free "dove", CC BY 4.0 - https://fontawesome.com/license/free
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 640 640"" fill=""{_input.Color}"">
                        <path d=""M560 128C573.2 128 580.7 143.1 572.8 153.6L544 192L544 368C544 447.5 479.5 512 400 512L288 512L241.7 558.3C231.3 568.7 215.2 570.7 202.6 563.1L105.5 504.9C88.5 494.7 90.5 469.4 108.9 462L224 416C87.8 375.1 71.5 233.8 86 159.7C89.6 141.9 109.3 135.4 125.3 144.2L384 288L384 208C384 163.8 419.8 128 464 128L560 128zM464 184C450.7 184 440 194.7 440 208C440 221.3 450.7 232 464 232C477.3 232 488 221.3 488 208C488 194.7 477.3 184 464 184zM246.5 54.4C258.9 40.7 279.8 45.5 289 61.5L345.4 159.8C339.6 174.2 336.2 189.9 336 206.3L202.1 132C212.2 100.4 229.1 73.6 246.5 54.4z""/>
                    </svg>";
                case IconType.Note:
                    return WrapSvg(@"<path d=""M4 4h13l3 3v13H4z""/><path d=""M17 4v3h3""/>");
                case IconType.NewTeam:
                    return WrapSvg(@"<rect x=""2"" y=""9"" width=""11"" height=""7""/><path d=""M13 11h4l3 3v2h-7z""/><circle cx=""7"" cy=""18"" r=""1.5""/><circle cx=""17"" cy=""18"" r=""1.5""/>");
                case IconType.BreakoutCandidate:
                    return WrapSvg(@"<path d=""M4 4v16h16""/><path d=""M4 15l4-4 3 3 6-7""/>");
                case IconType.BustCandidate:
                    return WrapSvg(@"<path d=""M4 4v16h16""/><path d=""M4 9l4 4 3-3 6 7""/>");
                case IconType.PositionBattle:
                    return WrapSvg(@"<path d=""M4 20l14-14""/><path d=""M5 17h3v3""/><path d=""M20 20L6 6""/><path d=""M19 17h-3v3""/>");
                case IconType.TwoWayPlayer:
                    // Source: Font Awesome Free "user-friends" / "people", CC BY 4.0 - https://fontawesome.com/license/free
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 640 640"" fill=""{_input.Color}"">
                        <path d=""M96 192C96 130.1 146.1 80 208 80C269.9 80 320 130.1 320 192C320 253.9 269.9 304 208 304C146.1 304 96 253.9 96 192zM32 528C32 430.8 110.8 352 208 352C305.2 352 384 430.8 384 528L384 534C384 557.2 365.2 576 342 576L74 576C50.8 576 32 557.2 32 534L32 528zM464 128C517 128 560 171 560 224C560 277 517 320 464 320C411 320 368 277 368 224C368 171 411 128 464 128zM464 368C543.5 368 608 432.5 608 512L608 534.4C608 557.4 589.4 576 566.4 576L421.6 576C428.2 563.5 432 549.2 432 534L432 528C432 476.5 414.6 429.1 385.5 391.3C408.1 376.6 435.1 368 464 368z""/>
                    </svg>";
                case IconType.NoBackToBack:
                    return WrapSvg(@"<circle cx=""10"" cy=""7"" r=""3""/><path d=""M5 20c0-4 2-7 5-7s5 3 5 7""/><path d=""M17 8l4 4""/><path d=""M21 8l-4 4""/>");
                case IconType.TankCandidate:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 100 84"" fill=""none"">
                        <rect x=""56"" y=""33"" width=""38"" height=""6"" rx=""1"" fill=""{_input.Color}""/>
                        <path d=""M26 44 V22 H40 V29 H58 V44"" fill=""none"" stroke=""{_input.Color}"" stroke-width=""5"" stroke-linejoin=""round"" stroke-linecap=""round""/>
                        <path d=""M24 44 H70 A16 16 0 0 1 70 76 H24 A16 16 0 0 1 24 44 Z"" fill=""none"" stroke=""{_input.Color}"" stroke-width=""5""/>
                        <circle cx=""28"" cy=""64"" r=""5.5"" fill=""{_input.Color}""/>
                        <circle cx=""42"" cy=""64"" r=""5.5"" fill=""{_input.Color}""/>
                        <circle cx=""56"" cy=""64"" r=""5.5"" fill=""{_input.Color}""/>
                        <circle cx=""70"" cy=""64"" r=""5.5"" fill=""{_input.Color}""/>
                    </svg>";
                case IconType.Sleeper:
                    return WrapSvg(@"<path d=""M4 6h8l-8 8h8""/><path d=""M15 14h4l-4 4h4""/>");
                case IconType.ManifestoArticlePlayer:
                    return WrapSvg(@"<path d=""M7 4a2 2 0 1 0 0 4h10V4z""/><path d=""M17 20a2 2 0 1 0 0-4H7v4z""/><path d=""M7 8v8""/><path d=""M17 8v8""/>");
                case IconType.WaiverWire:
                    // Source: Font Awesome Free "reel", CC BY 4.0 - https://fontawesome.com/license/free
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Color}"">
                        <rect x=""5"" y=""4"" width=""14"" height=""2"" rx=""0.5""/>
                        <path d=""M7 6h2v1H7V6Zm8 0h2v1h-2V6Z""/>
                        <rect x=""6.5"" y=""8"" width=""11"" height=""2"" rx=""1""/>
                        <rect x=""6.5"" y=""11"" width=""12.5"" height=""2"" rx=""1""/>
                        <rect x=""6.5"" y=""14"" width=""11"" height=""2"" rx=""1""/>
                        <path d=""M7 17h2v1H7v-1Zm8 0h2v1h-2v-1Z""/>
                        <rect x=""5"" y=""18"" width=""14"" height=""2"" rx=""0.5""/>
                    </svg>";
                case IconType.SpotStart:
                    return WrapSvg(@"<circle cx=""9"" cy=""7"" r=""3""/><path d=""M4 20c0-4 2-7 5-7s5 3 5 7""/><path d=""M17 8v4""/><path d=""M15 10h4""/>");

                case IconType.Kebab:
                    return WrapSvg(@"<circle cx=""12"" cy=""5"" r=""1.5""/><circle cx=""12"" cy=""12"" r=""1.5""/><circle cx=""12"" cy=""19"" r=""1.5""/>");
case IconType.Basketball:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""10""/><line x1=""12"" y1=""2"" x2=""12"" y2=""22""/><line x1=""2"" y1=""12"" x2=""22"" y2=""12""/><path d=""M4.93 4.93C8.5 8 8.5 16 4.93 19.07""/><path d=""M19.07 4.93C15.5 8 15.5 16 19.07 19.07""/>");
case IconType.PersonMinus:
    return WrapSvg(@"<path d=""M14 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2""/><circle cx=""8.5"" cy=""7"" r=""4""/><line x1=""17"" y1=""11"" x2=""23"" y2=""11""/>");
case IconType.Baseball:
    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""10""/><path d=""M6.3 4.5C8.5 7 8.5 17 6.3 19.5""/><path d=""M17.7 4.5C15.5 7 15.5 17 17.7 19.5""/>");
                case IconType.Football:
                    return WrapSvg(@"<path d=""M2 12c3-5 6.5-7.5 10-7.5S19 7 22 12c-3 5-6.5 7.5-10 7.5S5 17 2 12z""/><line x1=""9"" y1=""12"" x2=""15"" y2=""12""/><line x1=""10.5"" y1=""10.5"" x2=""10.5"" y2=""13.5""/><line x1=""12"" y1=""10.5"" x2=""12"" y2=""13.5""/><line x1=""13.5"" y1=""10.5"" x2=""13.5"" y2=""13.5""/>");
case IconType.HockeyPuck:
    return WrapSvg(@"<ellipse cx=""12"" cy=""9"" rx=""8"" ry=""3.5""/><path d=""M4 9v5c0 1.9 3.6 3.5 8 3.5s8-1.6 8-3.5V9""/>");
case IconType.Robot:
    return WrapSvg(@"<rect x=""4"" y=""8"" width=""16"" height=""12"" rx=""2""/><line x1=""12"" y1=""4"" x2=""12"" y2=""8""/><circle cx=""12"" cy=""3"" r=""1""/><circle cx=""9"" cy=""13"" r=""1""/><circle cx=""15"" cy=""13"" r=""1""/><line x1=""9"" y1=""17"" x2=""15"" y2=""17""/>");
case IconType.WandMagic:
    return WrapSvg(@"<line x1=""4"" y1=""20"" x2=""14"" y2=""10""/><polyline points=""15 3 16 6 19 7 16 8 15 11 14 8 11 7 14 6""/><line x1=""20"" y1=""13"" x2=""20"" y2=""17""/><line x1=""18"" y1=""15"" x2=""22"" y2=""15""/>");
case IconType.Lock:
    return WrapSvg(@"<rect x=""3"" y=""11"" width=""18"" height=""11"" rx=""2""/><path d=""M7 11V7a5 5 0 0 1 10 0v4""/>");
case IconType.Envelope:
    return WrapSvg(@"<rect x=""2"" y=""4"" width=""20"" height=""16"" rx=""2""/><polyline points=""2 6 12 13 22 6""/>");
case IconType.Twitter:
    return WrapSvg(@"<path d=""M23 3a10.9 10.9 0 0 1-3.14 1.53 4.48 4.48 0 0 0-7.86 3v1A10.66 10.66 0 0 1 3 4s-4 9 5 13a11.64 11.64 0 0 1-7 2c9 5 20 0 20-11.5a4.5 4.5 0 0 0-.08-.83A7.72 7.72 0 0 0 23 3z""/>");
case IconType.UserGear:
    return WrapSvg(@"<path d=""M14 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2""/><circle cx=""8.5"" cy=""7"" r=""4""/><circle cx=""18"" cy=""15"" r=""3""/><line x1=""18"" y1=""10.5"" x2=""18"" y2=""12""/><line x1=""18"" y1=""18"" x2=""18"" y2=""19.5""/><line x1=""13.5"" y1=""15"" x2=""15"" y2=""15""/><line x1=""21"" y1=""15"" x2=""22.5"" y2=""15""/>");
case IconType.Box:
    return WrapSvg(@"<path d=""M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z""/><polyline points=""3.3 7 12 12 20.7 7""/><line x1=""12"" y1=""22"" x2=""12"" y2=""12""/>");
case IconType.Newspaper:
    return WrapSvg(@"<path d=""M4 22h14a2 2 0 0 0 2-2V4a2 2 0 0 0-2-2H8a2 2 0 0 0-2 2v16a2 2 0 0 1-2 2z""/><path d=""M4 22a2 2 0 0 1-2-2v-9a2 2 0 0 1 2-2h2""/><line x1=""10"" y1=""7"" x2=""16"" y2=""7""/><line x1=""10"" y1=""11"" x2=""16"" y2=""11""/><line x1=""10"" y1=""15"" x2=""14"" y2=""15""/>");
                case IconType.DragHandle:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Color}"">
                        <circle cx=""9"" cy=""6"" r=""1.6""/><circle cx=""15"" cy=""6"" r=""1.6""/>
                        <circle cx=""9"" cy=""12"" r=""1.6""/><circle cx=""15"" cy=""12"" r=""1.6""/>
                        <circle cx=""9"" cy=""18"" r=""1.6""/><circle cx=""15"" cy=""18"" r=""1.6""/>
                    </svg>";

                case IconType.Favorite:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"">
                        <polygon points=""12,2 15.09,8.26 22,9.27 17,14.14 18.18,21.02 12,17.77 5.82,21.02 7,14.14 2,9.27 8.91,8.26"" fill=""#F5B301"" stroke=""#B37A00"" stroke-width=""1"" stroke-linejoin=""round""/>
                    </svg>";
                case IconType.FavoriteOutline:
                    return WrapSvg(@"<polygon points=""12,2 15.09,8.26 22,9.27 17,14.14 18.18,21.02 12,17.77 5.82,21.02 7,14.14 2,9.27 8.91,8.26""/>");

                case IconType.LimitedMinutes:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""9""/><path d=""M12 7v5h4""/>");
                case IconType.IsTeamUpdate:
                    return WrapSvg(@"<path d=""M3 11v2a2 2 0 0 0 2 2h1l2 4h2l-1.5-4H11l7 3V6l-7 3H6a2 2 0 0 0-2 2z""/><path d=""M11 9v6""/>");
                case IconType.Edit:
                    return WrapSvg(@"<path d=""M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4Z""/>");
                case IconType.ExternalLink:
                    return WrapSvg(@"<path d=""M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6""/><polyline points=""15 3 21 3 21 9""/><line x1=""10"" y1=""14"" x2=""21"" y2=""3""/>");
                case IconType.Close:
                    return WrapSvg(@"<line x1=""18"" y1=""6"" x2=""6"" y2=""18""/><line x1=""6"" y1=""6"" x2=""18"" y2=""18""/>");
                case IconType.ChatBubble:
                    return WrapSvg(@"<path d=""M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z""/>");
                case IconType.ArrowUp:
                    return WrapSvg(@"<polyline points=""6 15 12 9 18 15""/>");
                case IconType.Filter:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Color}"">
                        <polygon points=""3,4 21,4 14,13 14,20 10,18 10,13""/>
                    </svg>";
                case IconType.UnofficialTag:
                    return WrapSvg(@"<rect width=""8"" height=""4"" x=""8"" y=""2"" rx=""1"" ry=""1""/><path d=""M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2""/><path d=""M10 12a2 2 0 1 1 3 1.73c-.6.35-1 1-1 1.77""/><line x1=""12"" y1=""18"" x2=""12.01"" y2=""18""/>");
                case IconType.Verified:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 22 22""><path d=""M20.396 11c-.018-.646-.215-1.275-.57-1.816-.354-.54-.852-.972-1.438-1.246.223-.607.27-1.264.14-1.897-.131-.634-.437-1.218-.882-1.687-.47-.445-1.053-.75-1.687-.882-.633-.13-1.29-.083-1.897.14-.273-.587-.704-1.086-1.245-1.44S11.647 1.62 11 1.604c-.646.017-1.273.213-1.813.568s-.969.854-1.24 1.44c-.608-.223-1.267-.272-1.902-.14-.635.13-1.22.436-1.69.882-.445.47-.749 1.055-.878 1.688-.13.633-.08 1.29.144 1.896-.587.274-1.087.705-1.443 1.245-.356.54-.555 1.17-.574 1.817.02.647.218 1.276.574 1.817.356.54.856.972 1.443 1.245-.224.606-.274 1.263-.144 1.896.13.634.433 1.218.877 1.688.47.443 1.054.747 1.687.878.633.132 1.29.084 1.897-.136.274.586.705 1.084 1.246 1.439.54.354 1.17.551 1.816.569.647-.016 1.276-.213 1.817-.567s.972-.854 1.245-1.44c.604.239 1.266.296 1.903.164.636-.132 1.22-.447 1.68-.907.46-.46.776-1.044.908-1.681s.075-1.299-.165-1.903c.586-.274 1.084-.705 1.439-1.246.354-.54.551-1.17.569-1.816zM9.662 14.85l-3.429-3.428 1.293-1.302 2.072 2.072 4.4-4.794 1.347 1.246z"" fill=""{_input.Color}""/></svg>";
                case IconType.Success:
                     return WrapSvg(@"<path d=""M22 11.08V12a10 10 0 1 1-5.93-9.14""/><polyline points=""22 4 12 14.01 9 11.01""/>");
                case IconType.ArrowDown:
                    return WrapSvg(@"<polyline points=""6 9 12 15 18 9""/>");
                case IconType.Plus:
                    return WrapSvg(@"<line x1=""12"" y1=""5"" x2=""12"" y2=""19""/><line x1=""5"" y1=""12"" x2=""19"" y2=""12""/>");
                case IconType.Moon:
                    return WrapSvg(@"<path d=""M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z""/>");
                case IconType.Sun:
                    return WrapSvg(@"<circle cx=""12"" cy=""12"" r=""5""/><line x1=""12"" y1=""1"" x2=""12"" y2=""3""/><line x1=""12"" y1=""21"" x2=""12"" y2=""23""/><line x1=""4.22"" y1=""4.22"" x2=""5.64"" y2=""5.64""/><line x1=""18.36"" y1=""18.36"" x2=""19.78"" y2=""19.78""/><line x1=""1"" y1=""12"" x2=""3"" y2=""12""/><line x1=""21"" y1=""12"" x2=""23"" y2=""12""/><line x1=""4.22"" y1=""19.78"" x2=""5.64"" y2=""18.36""/><line x1=""18.36"" y1=""5.64"" x2=""19.78"" y2=""4.22""/>");
                case IconType.Ai:
                    return WrapSvg(@"<path d=""M12 2v3""/><rect x=""4"" y=""7"" width=""16"" height=""12"" rx=""3""/><circle cx=""9"" cy=""13"" r=""1.3""/><circle cx=""15"" cy=""13"" r=""1.3""/><path d=""M2 12v3""/><path d=""M22 12v3""/>");
                case IconType.Gemini:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Color}""><path d=""M12 2 Q13 11 22 12 Q13 13 12 22 Q11 13 2 12 Q11 11 12 2 Z""/></svg>";
                case IconType.ChromeExtension:
                    return WrapSvg(@"<path d=""M 4.2 6.6 H 9.5 A 2.4 2.4 0 1 1 14.3 6.6 H 17.4 V 11.9 A 2.4 2.4 0 1 1 17.4 16.7 V 19.8 H 14.3 A 2.4 2.4 0 1 0 9.5 19.8 H 4.2 Z"" />");
                case IconType.Grok:
                    return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{_input.Size}"" height=""{_input.Size}"" viewBox=""0 0 24 24"" fill=""{_input.Color}"" fill-rule=""evenodd""><path d=""M9.27 15.29l7.978-5.897c.391-.29.95-.177 1.137.272.98 2.369.542 5.215-1.41 7.169-1.951 1.954-4.667 2.382-7.149 1.406l-2.711 1.257c3.889 2.661 8.611 2.003 11.562-.953 2.341-2.344 3.066-5.539 2.388-8.42l.006.007c-.983-4.232.242-5.924 2.75-9.383.06-.082.12-.164.179-.248l-3.301 3.305v-.01L9.267 15.292M7.623 16.723c-2.792-2.67-2.31-6.801.071-9.184 1.761-1.763 4.647-2.483 7.166-1.425l2.705-1.25a7.808 7.808 0 00-1.829-1A8.975 8.975 0 005.984 5.83c-2.533 2.536-3.33 6.436-1.962 9.764 1.022 2.487-.653 4.246-2.34 6.022-.599.63-1.199 1.259-1.682 1.925l7.62-6.815""/></svg>";
                default:
                    return "";
            }
        }
    }
}