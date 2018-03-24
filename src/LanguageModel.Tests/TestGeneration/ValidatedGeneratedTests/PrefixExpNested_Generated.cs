// Copyright (c) Microsoft. All rights reserved.

namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class PrefixExpNested_Generated
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.FunctionCallStatementNode);
                    {
                        t.N(SyntaxKind.FunctionCallExp);
                        {
                            t.N(SyntaxKind.DotVar);
                            {
                                t.N(SyntaxKind.FunctionCallExp);
                                {
                                    t.N(SyntaxKind.DotVar);
                                    {
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                        t.N(SyntaxKind.Dot);
                                        t.N(SyntaxKind.Identifier);
                                    }
                                    t.N(SyntaxKind.ParenArg);
                                    {
                                        t.N(SyntaxKind.OpenParen);
                                        t.N(SyntaxKind.ExpList);
                                        {
                                            t.N(SyntaxKind.SquareBracketVar);
                                            {
                                                t.N(SyntaxKind.NameVar);
                                                {
                                                    t.N(SyntaxKind.Identifier);
                                                }
                                                t.N(SyntaxKind.OpenBracket);
                                                t.N(SyntaxKind.SimpleExpression);
                                                {
                                                    t.N(SyntaxKind.Number);
                                                }
                                                t.N(SyntaxKind.CloseBracket);
                                            }
                                        }
                                        t.N(SyntaxKind.CloseParen);
                                    }
                                }
                                t.N(SyntaxKind.Dot);
                                t.N(SyntaxKind.Identifier);
                            }
                            t.N(SyntaxKind.ParenArg);
                            {
                                t.N(SyntaxKind.OpenParen);
                                t.N(SyntaxKind.ExpList);
                                t.N(SyntaxKind.CloseParen);
                            }
                        }
                        t.N(SyntaxKind.Colon);
                        t.N(SyntaxKind.Identifier);
                        t.N(SyntaxKind.TableConstructorArg);
                        {
                            t.N(SyntaxKind.OpenCurlyBrace);
                            t.N(SyntaxKind.FieldList);
                            t.N(SyntaxKind.CloseCurlyBrace);
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
