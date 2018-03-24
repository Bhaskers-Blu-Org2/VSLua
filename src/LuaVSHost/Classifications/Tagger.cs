﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageService;
using LanguageService.Classification;
using LanguageService.Shared;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    internal class Tagger : DisposableObject, ITagger<ClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private IStandardClassificationService standardClassifications;
        private IClassificationTypeRegistryService classificationTypeRegistry;
        private ISingletons singletons;
        private static Dictionary<Classification, IClassificationType> vsClassifications;
        private ITextBuffer buffer;
        private CancellationTokenSource cancellationTokenSource;
        private bool doParserRelatedColorization;

        private Dictionary<SnapshotSpan, ClassificationTag> parserTags;

        internal Tagger(ITextBuffer buffer, IStandardClassificationService standardClassifications, IClassificationTypeRegistryService classificationTypeRegistry, ISingletons singletons)
        {
            Requires.NotNull(standardClassifications, nameof(standardClassifications));
            Requires.NotNull(classificationTypeRegistry, nameof(classificationTypeRegistry));
            Requires.NotNull(singletons, nameof(singletons));

            this.buffer = buffer;
            this.singletons = singletons;

            this.standardClassifications = standardClassifications;
            this.classificationTypeRegistry = classificationTypeRegistry;
            this.singletons = singletons;

            vsClassifications =
                vsClassifications == null ?
                InitializeDictionary(this.standardClassifications, this.classificationTypeRegistry) :
                vsClassifications;

            this.buffer.Changed += this.OnBufferChanged;
            this.UpdateParserRelatedClassifications(this.buffer.CurrentSnapshot, new CancellationToken());
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            bool useParser = this.doParserRelatedColorization;
            this.doParserRelatedColorization = false;
            return this.GetTags(spans, useParser);
        }

        private IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans, bool useParser)
         {
            if (spans.Count < 0)
            {
                yield break;
            }

            List<Range> ranges = new List<Range>();

            foreach (SnapshotSpan span in spans)
            {
                ranges.Add(new Range(span.Start.Position, span.Length));
            }

            ITextSnapshot snapshot = this.buffer.CurrentSnapshot;
            SourceText sourceText = this.singletons.SourceTextCache.Get(snapshot);

            if (useParser)
            {
                this.parserTags = new Dictionary<SnapshotSpan, ClassificationTag>();
                foreach (TagInfo tagInfo in this.singletons.FeatureContainer.Colourizer.ColorizeParserTokens(sourceText))
                {
                    IClassificationType classification = this.standardClassifications.Other;
                    vsClassifications.TryGetValue(tagInfo.Classification, out classification);
                    SnapshotSpan snapshotSpan = new SnapshotSpan(spans[0].Snapshot, tagInfo.Start, tagInfo.Length);
                    this.parserTags.Add(snapshotSpan, new ClassificationTag(classification));
                }
            }

            foreach (TagInfo tagInfo in this.singletons.FeatureContainer.Colourizer.ColorizeLexerTokens(sourceText, ranges))
                {
                SnapshotSpan tokenSpan = new SnapshotSpan(snapshot, tagInfo.Start, tagInfo.Length);
                    IClassificationType classification = this.standardClassifications.Other;
                vsClassifications.TryGetValue(tagInfo.Classification, out classification);

                    yield return new TagSpan<ClassificationTag>(tokenSpan, new ClassificationTag(classification));
                }

            if (this.parserTags != null)
            {
                foreach (SnapshotSpan snapshotSpan in this.parserTags.Keys)
                {
                    yield return new TagSpan<ClassificationTag>(snapshotSpan, this.parserTags[snapshotSpan]);
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this.buffer != null)
            {
                this.buffer.Changed -= this.OnBufferChanged;
            }

            base.DisposeManagedResources();
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            this.UpdateLexerRelatedClassifications(e);

            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            this.UpdateParserRelatedClassifications(e.After, this.cancellationTokenSource.Token);
        }

        private void UpdateLexerRelatedClassifications(TextContentChangedEventArgs e)
        {
            int position = e.Changes.Min(c => c.OldPosition);
            this.TagsChanged?.Invoke(this,
                new SnapshotSpanEventArgs(EditorUtilities.CreateSnapshotSpan(e.Before, position, e.Before.Length - position)));
        }

        private void UpdateParserRelatedClassifications(ITextSnapshot snapshot, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(Constants.Colourization.ParserUpdateDelay);

                if (token.IsCancellationRequested)
                {
                    return;
            }

                this.doParserRelatedColorization = true;
                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }, token);
        }

        private static Dictionary<Classification, IClassificationType> InitializeDictionary(IStandardClassificationService standardClassifications, IClassificationTypeRegistryService classificationTypeRegistry)
        {
            return new Dictionary<Classification, IClassificationType>()
            {
                { Classification.Comment, standardClassifications.Comment },
                { Classification.Keyword, standardClassifications.Keyword },
                { Classification.KeyValue, standardClassifications.Literal },
                { Classification.Operator, standardClassifications.Operator },
                { Classification.Number, standardClassifications.NumberLiteral },
                { Classification.Punctuation, classificationTypeRegistry.GetClassificationType("punctuation") },
                { Classification.StringLiteral, standardClassifications.StringLiteral },
                { Classification.Bracket, classificationTypeRegistry.GetClassificationType("punctuation") },
                { Classification.Global, classificationTypeRegistry.GetClassificationType(Constants.Colourization.GlobalName) },
                { Classification.Local, classificationTypeRegistry.GetClassificationType(Constants.Colourization.LocalName) },
                { Classification.ParameterReference, classificationTypeRegistry.GetClassificationType(Constants.Colourization.ParamName) },
                { Classification.Field, classificationTypeRegistry.GetClassificationType(Constants.Colourization.FieldName) }
            };
        }
    }
}
