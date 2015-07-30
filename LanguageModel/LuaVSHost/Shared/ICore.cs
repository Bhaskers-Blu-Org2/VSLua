﻿using System;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LuaLanguageService.Formatting;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal interface ICore
    {
        GlobalEditorOptions GlobalEditorOptions { get; }
        IServiceProvider ServiceProvider { get; }
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
        SourceTextCache SourceTextCache { get; }
        LuaFeatureContainer FeatureContainer { get; }
        UserSettings UserSettings { get; }
    }
}
