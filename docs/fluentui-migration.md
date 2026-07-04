# Plan: Migrate Fluent UI Blazor v4 to v5

## Status (as of 2026-07-04)

**The migration is functionally complete and the entire solution builds with 0 errors** (43 cosmetic warnings remain). Phases 0, 2, 3, 4, 5, and 6 below are DONE — see [fluentui-migration-changelog.md](fluentui-migration-changelog.md) for the exact file-by-file record of what was changed and why (including several corrections discovered mid-implementation that supersede parts of this plan document).

**The only remaining/not-started work is Phase 7 (bUnit test suites)** — this is optional follow-up, not required for compilation. If resuming this migration in a later session, start there. A few low-priority cosmetic warnings (obsolete enum literals, 2 unused fields) are also still open — see the changelog's "Remaining follow-up work" section.

This plan document is left mostly as originally written (some parts were superseded by ground truth discovered during implementation — see the changelog for corrections). Use it for the original intent/reasoning; use the changelog for what actually happened.

## Decisions

- Toasts are not removed in the target v5 package, but `IToastService`/`IMessageService` as separate injectable services ARE gone — both were unified into a single `INotificationService` (`ShowSuccessToastAsync`, `ShowErrorBarAsync`, etc.). This was discovered during Phase 5 implementation; see the changelog.
- Use v5 migration helper extensions such as `ToButtonAppearance()`, `ToTextInputAppearance()`, and `ToPositioning()` where available to keep the diff smaller and reduce manual enum rewrites.
- Remove `FluentUI.Blazor.Community.Components` v1.1.0 package reference and its `@using`; no actual usage was found in the codebase.
- `FluentValidationMessage` turned out to still work unchanged in this package version — the planned migration to `FluentField` was NOT performed (and is not needed).
- Add bUnit test suites for every Web App project after the component migration lands (Phase 7 — not yet started).

## Target Version

Use `Microsoft.FluentUI.AspNetCore.Components` and `Microsoft.FluentUI.AspNetCore.Components.Icons` version `5.0.0-rc.4-26180.1`.

The Fluent UI documentation tool available in this workspace is pinned to `5.0.0.26177` and reports this target version as incompatible. Treat generated migration guidance as directionally useful, but verify exact API shape against the installed `5.0.0-rc.4-26180.1` package during implementation. The toast correction below is the known example where the docs-tool guidance was stale for this target.

## Toast Correction (superseded — see Phase 5 below for what actually happened)

Earlier planning claimed `FluentToast`, `FluentToastProvider`, and `IToastService` were removed and should be replaced with `IMessageService`/`FluentMessageBar`. That guess was wrong in one direction, and this correction was wrong in the other: the actual finding (confirmed via `dotnet-inspect` against the real installed package) is that `FluentToast`/`FluentToastProvider` components still exist, but `IToastService` and `IMessageService` do NOT exist as separate DI services — both were unified into a single `INotificationService`. All `IToastService`/`IMessageService` injections were migrated to `INotificationService` in Phase 5.

## Architecture Note

Only one host app/layout surface needs provider and navigation migration:

- `src/KITT.Web.App/KITT.Web.App/Components/App.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor`

`KITT.Cms.Web.App` and `KITT.Proposals.Web.App` are Razor Class Libraries hosted by `KITT.Web.App.Client`, so they contribute pages/components but share the single app layout and provider setup.

## Phase 0: Package and Project Setup — ✅ COMPLETE

1. Bump `Microsoft.FluentUI.AspNetCore.Components` and `.Icons` to `5.0.0-rc.4-26180.1` in:
   - `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`
   - `src/KITT.Web.App/KITT.Web.App.Client/KITT.Web.App.Client.csproj`
   - `src/KITT.Web.App/KITT.Web.App/KITT.Web.App.csproj`
2. Remove `FluentUI.Blazor.Community.Components` from `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`.
3. Remove `@using FluentUI.Blazor.Community.Components` from `src/KITT.Cms.Web.App/_Imports.razor`.
4. Run a build and use the compiler errors as the authoritative implementation worklist.

## Phase 1: Core Bootstrap — ✅ COMPLETE (folded into Phases 0 and 2)

Depends on Phase 0. The `<FluentProviders>` wrapper (item 2 below) was actually added during Phase 2 implementation rather than as a standalone step — see the changelog.

1. In `src/KITT.Web.App.UI/ServiceCollectionExtensions.cs`, keep `AddFluentUIComponents(...)` and add Fluent default values for `FluentStack` gaps to preserve v4's implicit 10px spacing:

   ```csharp
   config.DefaultValues.For<FluentStack>().Set(p => p.HorizontalGap, "10px");
   config.DefaultValues.For<FluentStack>().Set(p => p.VerticalGap, "10px");
   ```

2. Add `<FluentProviders>` around the layout content in `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`, wrapping the `<FluentLayout>...</FluentLayout>` block.
3. Confirm the existing Fluent reboot CSS import still resolves after the package bump:

   ```css
   @import '/_content/Microsoft.FluentUI.AspNetCore.Components/css/reboot.css';
   ```

4. No custom `FluentComponentBase` subclasses, `FluentDesignTheme`, or `FluentDesignSystemProvider` usage was found, so no migration action is expected there.

## Phase 2: Layout and Navigation — ✅ COMPLETE

Depends on Phase 1.

1. Update `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`:
   - Keep `<FluentToastProvider />`; Fluent toast is not removed in the target v5 package — but the component now takes **zero parameters** (the `Position` attribute was removed from the tag; toast position is configured once via `AddFluentUIComponents(config => config.Toast.Position = ...)` in Phase 0 instead).
   - `<FluentDialogProvider />` and `<FluentTooltipProvider />` are still needed/valid in v5 — confirmed by a clean build, left unchanged.
   - `FluentHeader`, `FluentFooter`, and `FluentBodyContent` **do not exist at all** in v5 (confirmed via `dotnet-inspect find` — zero results) — replaced with plain `<header>`/`<footer>`/`<div class="body-content">` HTML elements, preserving existing CSS class hooks.
   - Replaced `FluentLabel` usages with `Typo`/`Color` parameters with `FluentText As="TextTag.H1"/"H2" Color="Color.Default"` (the new v5 typography component; `TextTag` enum has `H1`-`H6`/`Paragraph`/`Pre`/`Span`).
   - Added the `<FluentProviders>` wrapper around the whole layout (this had been deferred from Phase 1).
   - `FluentProgress` → `FluentProgressBar` for the non-interactive prerender fallback.

2. Rewrote `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor`:
   - `FluentNavMenu` to `FluentNav` (`Width="250"` int → `Width="250px"` string; no `Expanded`/`Title`/`Collapsible`/`CollapsedChildNavigation`/`CustomToggle` params exist on `FluentNav` at all — all dropped)
   - `FluentNavGroup` to `FluentNavCategory` (`Icon` → `IconRest`)
   - `FluentNavLink` to `FluentNavItem` (`Icon`/`IconColor` → `IconRest`)
   - Hamburger `FluentIcon Color="Color.Fill"` → `Color="Color.Default"`

3. Rebuilt `src/KITT.Web.App/KITT.Web.App.Client/Components/LoginDisplay.razor`, because `FluentProfileMenu` was removed with no direct replacement. Used a `FluentButton` (`Appearance="ButtonAppearance.Transparent"`) containing a `FluentAvatar` (`Initials`/`Name`) that toggles a `FluentPopover` (`AnchorId` + `@bind-Opened`) showing the user's name/email and a "Sign out" button.

## Phase 3: Dialog System Rearchitecture — ✅ COMPLETE

Affected files:

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor`
- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor.cs`
- `src/KITT.Cms.Web.App/Components/ContentForm.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Settings/Channels.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Index.razor.cs`
- `src/KITT.Proposals.Web.App/Pages/Index.razor.cs`
- `src/KITT.Proposals.Web.App/Components/ProposalDetailDialog.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageComposer.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor.cs`

Changes:

1. Replaced `IDialogContentComponent<T>` implementations with the v5 `FluentDialogInstance` base class. This class has a **required member `Localizer`** (renaming any same-named injected localizer to e.g. `LocalizerFor` was necessary to avoid a CS9031/CS0114 conflict) and an **abstract `OnActionClickedAsync(bool primary)` method** that must be implemented (dispatches to the existing Save/Close logic).
2. Removed `FluentDialogBody` and `FluentDialogFooter`; replaced with plain markup. Footer action buttons are now framework-rendered from `DialogOptions.Footer`, configured via an `OnInitializeDialog(header, footer)` override on the content component (keeps localized labels colocated with the component).
3. Replaced `DialogParameters<T>` with `DialogOptions`:
   - `Title` to `Header = { Title = ... }` — **note**: `DialogOptions.Header`/`.Footer` are get-only properties; use the nested-initializer form, NOT `Header = new DialogOptionsHeader { ... }` (that causes a CS0200 read-only-property error).
   - right-side panel alignment to `Alignment = DialogAlignment.End`
   - kept `Width` where applicable
4. `IDialogService.ShowPanelAsync<T>` is removed; replaced with `ShowDrawerAsync<T>`/`ShowDialogAsync<T>`, both of which return `Task<DialogResult>` **directly** (no more separate two-step `await dialog.Result`). `ShowConfirmationAsync` also changed: parameters renamed `primaryText`/`secondaryText` → `primaryButton`/`secondaryButton`, and it also now returns `Task<DialogResult>` directly instead of an intermediate reference.

## Phase 4: Forms and Inputs — ✅ COMPLETE

Main affected files include:

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor`
- `src/KITT.Cms.Web.App/Components/ScheduleForm.razor`
- `src/KITT.Cms.Web.App/Components/StreamingForm.razor`
- `src/KITT.Cms.Web.App/Components/ContentForm.razor`
- `src/KITT.Cms.Web.App/Pages/Streamings/StreamingDetail.razor`
- `src/KITT.Proposals.Web.App/Pages/Index.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor`

Changes:

1. Replaced `FluentTextField` with `FluentTextInput`:
   - `TextFieldType` to `TextInputType`
   - `FluentInputAppearance.Filled` to `.ToTextInputAppearance()` (helper extension, requires `@using Microsoft.FluentUI.AspNetCore.Components.Migration` — added globally to each project's `_Imports.razor` rather than per-file)
   - one special case: v4's slot-based child content (`<FluentLabel Slot="start">text</FluentLabel>` inside `FluentTextField`) became `<StartTemplate>text</StartTemplate>` inside `FluentTextInput`
2. Updated `FluentTextArea`:
   - `FluentInputAppearance` to `.ToTextAreaAppearance()`
   - Replaced `Rows` with `Height` (e.g. `Height="10em"`), because `Rows` was removed
3. Updated `FluentSelect`:
   - Added the second generic parameter `TValue` (paired with `TValue="string"` whenever `OptionValue` returns a string)
   - Replaced `@bind-SelectedOption` with `@bind-Value` / `Value`+`ValueChanged`
   - Replaced old `Appearance` enum usage with `ListAppearance.FilledDarker`
4. **`FluentValidationMessage` did NOT need migration** — contrary to the original plan (and contrary to the docs tool), it still works completely unchanged in this package version. The planned `FluentField` wrapping was not performed.
5. `FluentDatePicker`/`FluentTimePicker` needed their own appearance fix, discovered only after other blocking errors were cleared: `FluentDatePicker.Appearance` is `TextInputAppearance` (`.ToTextInputAppearance()`), while `FluentTimePicker.Appearance` is `ListAppearance` (no helper extension exists — used the literal `ListAppearance.FilledDarker`).
6. `Option<T>` (used to build `FluentSelect` item lists) no longer exists in the package at all — replaced with a new shared `SelectOption<T>` record added to `KITT.Web.App.UI`, reused across the Cms and Proposals apps.
7. Renamed all `FluentGridItem` breakpoint parameters to PascalCase:
   - `xs` to `Xs`
   - `sm` to `Sm`
   - `md` to `Md`
   - `lg` to `Lg`
   - `xl` to `Xl`
   - `xxl` to `Xxl`

8. Added explicit `Spacing="3"` to `FluentGrid` usages that relied on the v4 default spacing, because the v5 default is `0`.

## Phase 5: Toast Migration to `INotificationService` — ✅ COMPLETE (supersedes the original "no migration needed" assumption)

Depends on Phase 1.

The original plan assumed no toast migration was needed. **This was wrong**: `IToastService` and `IMessageService` do not exist as separate injectable services in the actual installed package — both were unified into a single `INotificationService` (`ShowSuccessToastAsync`, `ShowErrorToastAsync`, `ShowSuccessBarAsync`, `ShowErrorBarAsync`, `ShowMessageBarAsync`). All of the following were migrated (constructor/property rename + call-site rename, e.g. `toastService.ShowSuccess(msg)` → `await notificationService.ShowSuccessToastAsync(msg)`):

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Settings/Channels.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Import.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Index.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Schedule.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/StreamingDetail.razor.cs`
- `src/KITT.Proposals.Web.App/Pages/Index.razor.cs`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor.cs`

**Important process lesson learned here**: fixing this alone caused the visible build error count to jump from 8 to 59 before it was brought back down, because Roslyn suppresses many additional real diagnostics in a file once a "fundamental" error (like an unresolvable constructor parameter type) exists in it. Clearing the last blocking error in a file can reveal a batch of previously-hidden errors from earlier phases. See the changelog for the specific hidden defects this uncovered (missing `Migration` namespace import, `DialogOptions.Header` read-only misuse, a missed `ShowConfirmationAsync` call site, and the `FluentDatePicker`/`FluentTimePicker` gap from Phase 4).

## Phase 6: Remaining Component Sweep — ✅ COMPLETE

1. `FluentButton`/`FluentAnchorButton`: converted old `Appearance` enum usage to `ButtonAppearance` via `.ToButtonAppearance()` across `ContentForm.razor`, `Channels.razor`, `Streamings/Index.razor`, `StreamingDetail.razor`, `Proposals/Index.razor`, and `MessageComposer.razor`.
2. `FluentAnchor`: all 7 button-styled usages (`Appearance="Appearance.Accent"` + `Href=`) replaced with `FluentAnchorButton` (no plain-link-styled `FluentAnchor` usages were found, so `FluentLink` was not needed).
3. `FluentLabel Weight="FontWeight.Bold"` → `Weight="LabelWeight.Semibold"` (the real enum only has `Regular`/`Semibold`, there is no `Bold` value and no `FontWeight` type at all) in `ProposalDetailDialog.razor`.
4. `FluentProgressRing` renamed to `FluentSpinner` (done in Phase 0 for the shared `Loader.razor`).
5. `FluentCard`, `FluentDataGrid`, and `FluentTooltip` renamed-parameter items in the original plan were **not encountered** in this codebase (no `AreaRestricted`/`MinimalStyle`/`ColumnOptionsLabels`/`Position` usages exist) — no action was needed for those.
6. A full color-enum sweep (`Color.Fill`/`Color.Accent`/`Color.Neutral` → `Color.Default`/`Color.Primary`/etc.) was **not** performed — these remain as `CS0618` obsolete-but-functional warnings (see the changelog's follow-up list).

## Phase 7: bUnit Test Suites for Web App Projects — ⚪ NOT STARTED (the only remaining phase)

No working UI test project currently exists. `tests/KITT.Web.App.Test` is a stale empty folder with only `bin` and `obj`, no `.csproj`, and it is not referenced by `LemonBot.slnx`. No `bunit` package usage was found in the repo.

Create one xUnit + bUnit test project for each Web App project. Follow existing test-project conventions:

- `net10.0`
- `Microsoft.NET.Sdk`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
- `coverlet.collector`
- `<Using Include="Xunit" />`
- Add the `bunit` package
- Add `Moq` where clients/services need fakes
- Register each test project in `LemonBot.slnx`

New test projects:

1. `tests/KITT.Web.App.UI.Test/KITT.Web.App.UI.Test.csproj`
   - References `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`.
   - Cover `Loader.razor`, `NavButton.razor`, and `AddDefaultServices()` DI registration.

2. `tests/KITT.Web.App.Client.Test/KITT.Web.App.Client.Test.csproj`
   - References `src/KITT.Web.App/KITT.Web.App.Client/KITT.Web.App.Client.csproj`.
   - Cover migrated `NavMenu.razor`, rebuilt `LoginDisplay.razor`, `MessageEditorDialog.razor`, and `MessageComposer.razor`.

3. `tests/KITT.Web.App.Test/KITT.Web.App.Test.csproj`
   - Recreate this from scratch, replacing the stale empty folder.
   - References `src/KITT.Web.App/KITT.Web.App/KITT.Web.App.csproj`.
   - Cover `App.razor` at smoke-test level and verify the app host renders without throwing.

4. `tests/KITT.Cms.Web.App.Test/KITT.Cms.Web.App.Test.csproj`
   - References `src/KITT.Cms.Web.App/KITT.Cms.Web.App.csproj`.
   - Cover `ChannelFormPanel.razor`, `Channels.razor`, `ScheduleForm.razor`, and `StreamingForm.razor` with field binding, validation, and dialog action smoke tests.

5. `tests/KITT.Proposals.Web.App.Test/KITT.Proposals.Web.App.Test.csproj`
   - References `src/KITT.Proposals.Web.App/KITT.Proposals.Web.App.csproj`.
   - Cover `Index.razor` filters/sort/pagination rendering and `ProposalDetailDialog.razor` content/close action.

For each bUnit `TestContext`, register Fluent UI services with `Services.AddFluentUIComponents()` and localization with `Services.AddLocalization()` so components resolving Fluent configuration/localizers render correctly. Write these tests against the post-migration v5 component shape, not the old v4 tags.

## Verification

1. Run the build task after each phase and fix compiler errors iteratively. **Done for Phases 0-6** \u2014 the entire solution now builds with 0 errors (43 cosmetic warnings). Remaining: run again after Phase 7 adds new test projects.
2. Run the existing test suites:
   - `tests/KITT.Core.Test`
   - `tests/KITT.Cms.Web.Api.Test`
3. Run the new bUnit suites created in Phase 7. **Not yet done \u2014 Phase 7 has not started.**
4. Manually smoke test (**not yet done** \u2014 recommended before considering the migration fully verified, since `LoginDisplay` and the dialog footers were rebuilt from scratch rather than mechanically translated):
   - navigation links and category expansion
   - add/edit channel panel
   - validation display
   - success toasts and error message bars
   - streaming import/schedule/detail forms
   - proposals filters and proposal detail dialog
   - message composer dialog
   - login/profile menu
5. Visually check `FluentGrid` and `FluentStack` spacing on the main CMS and Proposals pages, since v5 changes default spacing behavior. **Not yet done.**

## Further Considerations / Lessons Learned (for resuming this migration later)

1. The `mcp_microsoft_flu_*` documentation tools are pinned to a different package version (`5.0.0.26177`) than the actual target (`5.0.0-rc.4-26180.1`) and reported incompatibility. Several of their claims turned out to be wrong for the real target version (toast removal, `FluentValidationMessage` removal). **Always verify component API shape against the actual installed package** using the `dotnet-inspect` tool (`dnx dotnet-inspect -y -- member <Type> --package Microsoft.FluentUI.AspNetCore.Components@5.0.0-rc.4-26180.1`) rather than trusting docs-tool output alone.
2. **Error-cascade suppression is a real hazard**: once a file has one \"fundamental\" compile error (e.g. an unresolvable constructor parameter type), Roslyn can suppress many other real diagnostics in that same file. Don't treat a reduced build-error count as proof that untouched code in the same file is correct \u2014 rebuild and re-triage after every batch of fixes, especially right after removing what looks like the \"last\" error in a file.
3. `DialogOptions.Header`/`.Footer` are get-only properties \u2014 use `Header = { Title = x }` nested-initializer syntax, not `Header = new DialogOptionsHeader { Title = x }`.
4. `FluentDialogInstance` requires implementing `OnActionClickedAsync(bool primary)` (abstract) and must not have its `Localizer` member hidden by a same-named field in the derived class.
5. When resuming with Phase 7, remember: `tests/KITT.Web.App.Test` is a stale empty folder (delete `bin`/`obj` first); no `bunit` package exists anywhere in the repo yet; register new test projects in `LemonBot.slnx`.
