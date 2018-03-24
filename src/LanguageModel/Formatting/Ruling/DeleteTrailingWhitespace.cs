﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using LanguageService.Shared;

namespace LanguageService.Formatting.Ruling
{
    internal class DeleteTrailingWhitespace : SimpleRule
    {
        internal DeleteTrailingWhitespace() : base(
                new RuleDescriptor(TokenRange.All, TokenRange.All),
                new List<Func<FormattingContext, bool>> { Rules.TokensAreNotOnSameLine },
                RuleAction.Delete)
        {
        }

        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            Token nextToken = formattingContext.NextToken.Token;
            // flag for the end of file token where whitespace should be deleted
            List<TextEditInfo> edits = this.GetEdits(nextToken, nextToken.Kind == SyntaxKind.EndOfFile);

            return edits;
        }

        private List<TextEditInfo> GetEdits(Token token, bool isEndOfFile)
        {
            Validation.Requires.NotNull(token, nameof(token));

            List<TextEditInfo> edits = new List<TextEditInfo>();

            int start = token.FullStart;
            int length = 0;
            var leadingTrivia = token.LeadingTrivia;
            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                length = leadingTrivia[i].Text.Length;
                if (this.IsNewLineAfterSpace(i, leadingTrivia) ||
                    (isEndOfFile && this.IsSpaceBeforeEndOfFile(i, leadingTrivia)))
                {
                    edits.Add(new TextEditInfo(new Range(start, length), string.Empty));
                }

                start += length;
            }

            return edits;
        }

        private bool IsNewLineAfterSpace(int index, List<Trivia> triviaList)
        {
            if (index + 1 >= triviaList.Count)
            {
                return false;
            }

            return triviaList[index].Type == SyntaxKind.Whitespace
                && triviaList[index + 1].Type == SyntaxKind.Newline;
        }

        private bool IsSpaceBeforeEndOfFile(int index, List<Trivia> triviaList)
        {
            if (triviaList == null || index >= triviaList.Count)
            {
                return false;
            }

            return triviaList[index].Type == SyntaxKind.Whitespace;
        }
    }
}
