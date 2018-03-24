﻿// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using LanguageService;
    using LanguageService.Formatting;
    using LanguageService.Formatting.Options;
    using LanguageService.Shared;
    using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
    using Microsoft.VisualStudio.Text;
    using Xunit;

    internal static class Tester
    {
        private static EditorUtils.EditorHostFactory factory = new EditorUtils.EditorHostFactory();
        private static EditorUtils.EditorHost host = factory.CreateEditorHost();

        internal struct PasteInfo
        {
            internal string PasteString { get; }
            internal int Start { get; }
            internal int Length { get; }
            internal PasteInfo(string pasteString, int start, int length)
            {
                this.PasteString = pasteString;
                this.Start = start;
                this.Length = length;
            }
        }


        internal static string FormatOnPaste(string original, PasteInfo pasteInfo)
        {
            var buffer = host.CreateTextBuffer(original);
            var prePastSnapshot = buffer.CurrentSnapshot;
            var bufferEdit = buffer.CreateEdit();
            bufferEdit.Replace(pasteInfo.Start, pasteInfo.Length, pasteInfo.PasteString);
            var bufferApplied = bufferEdit.Apply();
            SnapshotSpan? span = EditorUtilities.GetPasteSpan(prePastSnapshot, bufferApplied);

            if (span != null)
            {
                SnapshotSpan newSpan = (SnapshotSpan)span;
                var featureContainer = new LuaFeatureContainer();
                SourceText sourceText = new SourceText(bufferApplied.GetText());
                Range range = new Range(newSpan.Start.Position, newSpan.Length);
                List<TextEditInfo> edits = featureContainer.Formatter.Format(sourceText, range,
                    new FormattingOptions(new List<DisableableRules>(), 4, 4, false));
                var pastedBufferEdit = buffer.CreateEdit();
                foreach (TextEditInfo edit in edits)
                {
                    pastedBufferEdit.Replace(edit.Start, edit.Length, edit.ReplacingWith);
                }
                var pasteApplied = pastedBufferEdit.Apply();
                return pasteApplied.GetText();
            }

            return original;
        }


        private static string Format(string original, uint tabSize = 4, uint indentSize = 4, bool usingTabs = false, FormattingOptions formattingOptions = null)
        {
            LuaFeatureContainer featureContainer = new LuaFeatureContainer();
            Range range = new Range(0, original.Length);

            if (formattingOptions == null)
            {
                formattingOptions = new FormattingOptions(new List<DisableableRules>(), tabSize, indentSize, usingTabs);
            }

            List<TextEditInfo> textEdits = featureContainer.Formatter.Format(new SourceText(original), range, formattingOptions);

            var buffer = host.CreateTextBuffer(original);
            var edit = buffer.CreateEdit();

            foreach (var textEdit in textEdits)
            {
                edit.Replace(textEdit.Start, textEdit.Length, textEdit.ReplacingWith);
            }

            var applied = edit.Apply();

            return applied.GetText();
        }

        private static string Format(string original, FormattingOptions formattingOptions)
        {
            return Format(original, 4, 4, false, formattingOptions);
        }

        private static int SmartIndent(string text, int lineNumber)
        {
            LuaFeatureContainer featureContainer = new LuaFeatureContainer();

            var buffer = host.CreateTextBuffer(text);
            var lineSnapshot = buffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber);
            int position = lineSnapshot.Start.Position;

            return featureContainer.Formatter.SmartIndent(new SourceText(text), position);
        }

        internal static void FormattingTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        internal static void FormattingTest(string original, string expected1, string expected2)
        {
            string actual1 = Tester.Format(original);
            //string actual2 = Tester.Format(original, IndentStyle.Relative);
            Assert.Equal(expected1, actual1);
            //Assert.Equal(expected2, actual2);
        }

        internal static void FormattingTest(string original, string expected, FormattingOptions formattingOptions)
        {
            string actual = Format(original, formattingOptions);
            Assert.Equal(expected, actual);
        }

        internal static void SmartIndentationTest(string text, int lineNumber, int expectedIndent)
        {
            int actualIndent = Tester.SmartIndent(text, lineNumber);
            Assert.Equal(expectedIndent, actualIndent);
        }

        internal static void SpacesAndTabsTest(string text, uint tabSize, uint indentSize, int expectedTabs, int expectedSpaces, bool usingTabs)
        {
            string formatted = Tester.Format(text, tabSize, indentSize, usingTabs);

            string[] splits = formatted.Split(')');
            string split = splits[splits.Length - 1];
            split = split.Substring(2);
            int spaces = 0;
            int tabs = 0;
            foreach (char c in split)
            {
                if (c == ' ' || c == '\t')
                {
                    if (c == ' ')
                    {
                        spaces++;
                    }
                    else
                    {
                        tabs++;
                    }

                    continue;
                }
                break;
            }

            Assert.Equal(expectedSpaces, spaces);
            Assert.Equal(expectedTabs, tabs);
        }
    }
}
