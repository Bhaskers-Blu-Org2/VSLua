//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\AssignmentWithFunctionCall.lua
namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class Generated_2
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.AssignmentStatementNode);
                    {
                        t.N(SyntaxKind.VarList);
                        {
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                        }
                        t.N(SyntaxKind.AssignmentOperator);
                        t.N(SyntaxKind.ExpList);
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
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                        t.N(SyntaxKind.Comma);
                                        t.N(SyntaxKind.SimpleExpression);
                                        {
                                            t.N(SyntaxKind.String);
                                        }
                                        t.N(SyntaxKind.Comma);
                                        t.N(SyntaxKind.SimpleExpression);
                                        {
                                            t.N(SyntaxKind.String);
                                        }
                                    }
                                    t.N(SyntaxKind.CloseParen);
                                }
                            }
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
