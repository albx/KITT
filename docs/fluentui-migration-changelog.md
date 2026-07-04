# Fluent UI Blazor v4 to v5 Migration Changelog

Tracks the actual changes applied during the migration, in the order they were completed. See [fluentui-migration.md](fluentui-migration.md) for the overall plan.

## Target version

`Microsoft.FluentUI.AspNetCore.Components` / `.Icons` bumped to `5.0.0-rc.4-26180.1` (confirmed latest on NuGet).

## Ground truth corrections discovered during implementation

Real package inspection (via the `dotnet-inspect` tool against the actual installed `5.0.0-rc.4-26180.1` package) surfaced several corrections to the original migration plan:

- `IToastService` and `IMessageService` do not exist as separate services in this build. Both are unified into a single `INotificationService` (`ShowSuccessToastAsync`, `ShowErrorToastAsync`, `ShowSuccessBarAsync`, `ShowErrorBarAsync`, `ShowMessageBarAsync`).
- `FluentToastProvider` now takes zero parameters. Toast position is configured once at startup via `AddFluentUIComponents(config => config.Toast.Position = ToastPosition.TopCenter)`.
- `FluentValidationMessage<T>` (with `For`) is unchanged and still works — it was **not** removed as the docs claimed.
- `IDialogContentComponent<T>` is removed. Replacement: `FluentDialogInstance` base class, exposing `DialogInstance` (`IDialogInstance`) and a **required** `Localizer` member (must not be hidden by a same-named field in the derived component) and an **abstract** `OnActionClickedAsync(bool primary)` method that must be implemented.
- `IDialogService.ShowPanelAsync<T>` is removed. Replacement: `ShowDrawerAsync<T>`/`ShowDialogAsync<T>`, both returning `Task<DialogResult>` directly (no more separate `await dialog.Result`).
- `IDialogService.ShowConfirmationAsync` renamed its parameters from `primaryText`/`secondaryText` to `primaryButton`/`secondaryButton`, and also now returns `Task<DialogResult>` directly.
- `Option<T>` (used to build `FluentSelect` item lists) no longer exists. Replaced with a new shared `SelectOption<T>` record in `KITT.Web.App.UI`.
- `FluentSelect` requires both `TOption` and `TValue` type arguments now.
- `FluentTextField`/`FluentNumberField`/`FluentSearch` are removed, replaced by `FluentTextInput`.
- Migration helper extensions confirmed real and in use: `Appearance.X.ToButtonAppearance()`, `FluentInputAppearance.X.ToTextInputAppearance()` / `.ToTextAreaAppearance()` (namespace `Microsoft.FluentUI.AspNetCore.Components.Migration`).

## Phase 0 — Package and project setup (complete)

- Bumped `Microsoft.FluentUI.AspNetCore.Components` and `.Icons` to `5.0.0-rc.4-26180.1` in:
  - `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`
  - `src/KITT.Web.App/KITT.Web.App.Client/KITT.Web.App.Client.csproj`
  - `src/KITT.Web.App/KITT.Web.App/KITT.Web.App.csproj`
- Removed the unused `FluentUI.Blazor.Community.Components` package reference and its `@using` from `src/KITT.Cms.Web.App/_Imports.razor`.
- `src/KITT.Web.App.UI/ServiceCollectionExtensions.cs`: removed the obsolete `options.ValidateClassNames`; added `DefaultValues` for `FluentStack` horizontal/vertical gap (preserving the old 10px default) and `config.Toast.Position = ToastPosition.TopCenter`.
- `src/KITT.Web.App.UI/Components/NavButton.razor`: fixed `FluentButton` `Appearance` type mismatch using `Appearance.Accent.ToButtonAppearance()`.
- `src/KITT.Web.App.UI/Components/Loader.razor`: renamed `FluentProgressRing` to `FluentSpinner`.
- Added `src/KITT.Web.App.UI/SelectOption.cs`: new shared `SelectOption<T>` record replacing the removed `Option<T>` type, used to build `FluentSelect` item lists.

## Phase 3 — Dialog system rearchitecture (complete)

Rewrote every `IDialogContentComponent<T>` dialog to the new `FluentDialogInstance` base class, and every `IDialogService.ShowPanelAsync`/`ShowConfirmationAsync` call site to the new API shape.

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor` / `.razor.cs`: `@inherits FluentDialogInstance`, removed `FluentDialogBody`/`FluentDialogFooter` and manual Save/Close buttons, added `OnInitializeDialog` (sets localized footer button labels) and `OnActionClickedAsync` (dispatches to Save/Close), replaced `IToastService` with `INotificationService`.
- `src/KITT.Cms.Web.App/Pages/Settings/Channels.razor.cs`: replaced `ShowPanelAsync`/`DialogParameters<T>` with `ShowDrawerAsync`/`DialogOptions`; fixed `ShowConfirmationAsync` (renamed `primaryText` to `primaryButton`, removed the now-unnecessary second `await dialog.Result`); replaced `IToastService` with `INotificationService`.
- `src/KITT.Proposals.Web.App/Components/ProposalDetailDialog.razor` / `.razor.cs`: `@inherits FluentDialogInstance`; renamed the injected `Localizer` to `LocalizerFor` to avoid hiding the base class's required `Localizer` member; implemented `OnActionClickedAsync`/`OnInitializeDialog` (single Close action, secondary button hidden).
- `src/KITT.Proposals.Web.App/Pages/Index.razor.cs`: `OpenProposalDetailAsync` updated to `ShowDrawerAsync`/`DialogOptions`.
- `src/KITT.Cms.Web.App/Pages/Streamings/Index.razor.cs`: fixed `ShowConfirmationAsync` parameter names and removed the redundant `await confirm.Result`.
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor` / `.razor.cs`: `@inherits FluentDialogInstance`; renamed `Localizer` to `LocalizerFor`; replaced `IToastService` with `INotificationService`; updated `FluentTextArea` to the v5 shape (`Appearance` via `.ToTextAreaAppearance()`, `Height` instead of `Rows`).
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageComposer.razor`: updated the `ShowPanelAsync` call site to `ShowDrawerAsync`/`DialogOptions`.

**Build status after Phase 3**: error count dropped from 47 to 38; zero remaining errors are dialog-related. Remaining errors are all `FluentSelect`/`FluentTextField`/`Option<>` (Phase 4) and `IToastService`/`IMessageService` (Phase 5) in files not yet touched.

## Phase 4 — Forms and inputs (complete)

Fixed every remaining `FluentSelect` missing-`TValue` error, `FluentTextField`/`FluentTextArea` removal, `Option<T>` removal, and `FluentGridItem`/`FluentGrid` casing/spacing issue.

- Added `src/KITT.Web.App.UI/SelectOption.cs` usage across the codebase as the `Option<T>` replacement (see Phase 0 — the type was added there).
- `src/KITT.Cms.Web.App/Components/ContentForm.razor`: 3x `FluentTextField` → `FluentTextInput` (SEO Title/Description/Keywords fields).
- `src/KITT.Cms.Web.App/Components/ScheduleForm.razor`: `FluentGridItem` casing (`xs`/`md` → `Xs`/`Md`), `FluentGrid Spacing="3"` added to all 4 grids, 3x `FluentTextField` → `FluentTextInput`, the hosting-channel prefix converted from slot-based `<FluentLabel Slot="start">` to `<StartTemplate>`, `FluentTextArea` `Rows="10"` → `Height="10em"`.
- `src/KITT.Cms.Web.App/Components/StreamingForm.razor`: same pattern as ScheduleForm, plus 2x `FluentSelect` (Twitch/YouTube channel pickers) given `TOption="ChannelModel" TValue="string"` and `Appearance="ListAppearance.FilledDarker"`.
- `src/KITT.Cms.Web.App/Pages/Streamings/Index.razor` + `.razor.cs`: 2x `FluentSelect` (`TOption="SelectOption<...>" TValue="string"`), 1x `FluentTextField` → `FluentTextInput`, `FluentGridItem`/`FluentGrid` casing/spacing, and `Option<T>` → `SelectOption<T>` in the code-behind (added `using KITT.Web.App.UI;`).
- `src/KITT.Cms.Web.App/Pages/Streamings/StreamingDetail.razor`: same pattern as StreamingForm (2x `FluentSelect`, 4x `FluentTextField`, `FluentTextArea`, grid casing/spacing).
- `src/KITT.Cms.Web.App/Pages/Settings/Channels.razor`: `FluentGridItem` casing + `FluentGrid Spacing="3"`.
- `src/KITT.Proposals.Web.App/Pages/Index.razor` + `.razor.cs`: 3x `FluentSelect` (`TOption="UI.SelectOption<...>" TValue="string"`), 1x `FluentTextField` → `FluentTextInput`, grid casing/spacing, and `Option<T>` → `UI.SelectOption<T>` in the code-behind (already had the `UI` alias).
- `src/KITT.Web.App/KITT.Web.App.Client/Pages/Home.razor`: `FluentGridItem` casing + `FluentGrid Spacing="3"`.

**Build status after Phase 4**: error count dropped from 38 to 8. All 8 remaining errors are `IToastService`/`IMessageService` not found (Phase 5 scope, in `Import.razor.cs`, `Schedule.razor.cs`, `StreamingDetail.razor.cs`, `Streamings/Index.razor.cs`, `Proposals/Index.razor.cs`). No new errors were introduced by touching `FluentButton`/`FluentAnchor` `Appearance` usages in the same files — those remain a Phase 6 sweep item regardless of build-error visibility.

## Phase 5 — Toast service migration, plus fixing hidden defects from Phases 3/4 (complete)

Replaced the remaining `IToastService`/`IMessageService` injections with the unified `INotificationService` in `Import.razor.cs`, `Schedule.razor.cs`, `StreamingDetail.razor.cs`, `Streamings/Index.razor.cs`, `Proposals/Index.razor.cs` (constructor/property rename, `ShowSuccess`/`ShowError` → `ShowSuccessToastAsync`/`ShowErrorToastAsync`, `ShowMessageBarAsync(msg, MessageIntent.Error, section)` → `ShowErrorBarAsync(section, message: msg)`).

**Important discovery**: applying this fix alone caused the build error count to jump from 8 to **59** — not fewer. This happened because the Roslyn compiler suppresses many additional real diagnostics in a file/type once a "fundamental" error (like an unresolvable constructor parameter type) exists in it. Removing that one blocking error in each file revealed a batch of previously-hidden errors. Root causes found and fixed:

- Missing `@using Microsoft.FluentUI.AspNetCore.Components.Migration` for the `.ToTextInputAppearance()` / `.ToTextAreaAppearance()` extension methods introduced in Phase 4 — added the `using` to `_Imports.razor` in `KITT.Cms.Web.App`, `KITT.Proposals.Web.App`, and `KITT.Web.App.Client` instead of per-file.
- `DialogOptions.Header` and `.Footer` are **get-only** properties. The Phase 3 code incorrectly wrote `Header = new DialogOptionsHeader { Title = x }` (CS0200 read-only assignment). Fixed to the correct nested-initializer form `Header = { Title = x }` in `Channels.razor.cs` (both call sites), `Proposals/Index.razor.cs`, and `MessageComposer.razor`.
- A missed `IDialogService.ShowConfirmationAsync` call site in `src/KITT.Cms.Web.App/Components/ContentForm.razor.cs` still used the old `primaryText`/`secondaryText` parameter names and the old two-await `.Result` pattern — fixed to `primaryButton`/`secondaryButton` and the direct `DialogResult` return, matching the Phase 3 pattern applied elsewhere.
- A genuine Phase 4 gap: `FluentDatePicker.Appearance` is typed `TextInputAppearance` (fixed with `.ToTextInputAppearance()`) while `FluentTimePicker.Appearance` is typed `ListAppearance` (no helper extension exists for this conversion — used the literal `ListAppearance.FilledDarker`, consistent with `FluentSelect`). These were left untouched in the original Phase 4 pass and only surfaced once the toast errors were cleared. Fixed in `ScheduleForm.razor`, `StreamingForm.razor`, `Pages/Streamings/StreamingDetail.razor`.

**Build status after Phase 5 (and the fixes above)**: error count is now 21, and **100% of the remaining errors are Phase 6 scope** — `FluentButton`/`FluentAnchor` `Appearance="Appearance.X"` type mismatches (needs the `.ToButtonAppearance()` sweep) across `Channels.razor`, `ContentForm.razor`, `StreamingDetail.razor`, `Streamings/Index.razor`, `Proposals/Index.razor`; and a `FluentLabel Weight="FontWeight.Bold"` error in `ProposalDetailDialog.razor` (`FontWeight` doesn't exist as a type — needs the real `LabelWeight` enum). A handful of secondary "lambda not convertible to delegate type" errors are expected to resolve automatically once the button appearance mismatches are fixed.

## Phase 6 — Button/Anchor appearance sweep + FluentLabel fix (complete)

- `FluentButton Appearance="Appearance.Accent"`/`"Appearance.Outline"` → `Appearance="Appearance.Accent.ToButtonAppearance()"` (mechanical sweep) across `ContentForm.razor`, `Channels.razor`, `Streamings/Index.razor`, `StreamingDetail.razor`, `Proposals/Index.razor`, and `MessageComposer.razor` (Web.App.Client).
- All 7 button-styled `FluentAnchor` usages converted to `FluentAnchorButton` (same `Href`/`Appearance` shape) in `StreamingStats.razor`, `Import.razor`, `Streamings/Index.razor` (x2), `Schedule.razor`, `StreamingDetail.razor`, `ProposalsStats.razor`.
- `src/KITT.Proposals.Web.App/Components/ProposalDetailDialog.razor`: `FluentLabel Weight="FontWeight.Bold"` → `Weight="LabelWeight.Semibold"` (the real enum only has `Regular`/`Semibold`, no `Bold`/no `FontWeight` type at all).

**Build status after Phase 6**: `KITT.Cms.Web.App` and `KITT.Proposals.Web.App` now build with **zero errors** (warnings only — mostly `CS0618` obsolete-value warnings for `Color.Accent`/`Color.Fill` literals and `FluentProgress`, intentionally left for a later cleanup pass since they don't block compilation). Only **3 errors remain in the entire solution**, all in `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor` (`bind-Expanded` on `FluentNavMenu`/`FluentNavGroup`) — squarely Phase 2 scope (`FluentNavMenu` → `FluentNav` rewrite).

## Phase 2 — Layout and navigation (complete) — 🎉 last phase, brings the whole solution to 0 build errors

- `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor`: `FluentNavMenu` → `FluentNav` (`Width="250"` int → `Width="250px"` string; dropped `Collapsible`, `Title`, `CustomToggle`, `CollapsedChildNavigation`, and the top-level `@bind-Expanded` — none of these exist on `FluentNav`), `FluentNavGroup` → `FluentNavCategory` (`Icon` → `IconRest`), `FluentNavLink` → `FluentNavItem` (`Icon`/`IconColor` → `IconRest`). Hamburger icon `Color="Color.Fill"` → `Color="Color.Default"`.
- `src/KITT.Web.App/KITT.Web.App.Client/Components/LoginDisplay.razor`: `FluentProfileMenu` (fully removed, no direct replacement) rebuilt as a custom `FluentButton` (`Appearance="ButtonAppearance.Transparent"`) containing a `FluentAvatar` (`Initials`/`Name`), which toggles a `FluentPopover` (`AnchorId` + `@bind-Opened`) showing the user's name/email and a "Sign out" button.
- `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`:
  - Confirmed via `dotnet-inspect find` that `FluentHeader`, `FluentFooter`, and `FluentBodyContent` **do not exist at all** in v5 — replaced with plain `<header>`/`<footer>`/`<div class="body-content">` HTML elements, preserving the existing CSS class hooks.
  - Added the `<FluentProviders>` wrapper around the whole layout (the Phase 1 requirement that had been deferred until now).
  - Removed `FluentToastProvider`'s `Position` attribute — the component takes zero parameters now; position is configured once via `AddFluentUIComponents` (done in Phase 0).
  - `FluentLabel Typo="Typography.H1"/"H2" Color="Color.Fill"` → `FluentText As="TextTag.H1"/"H2" Color="Color.Default"` (the new v5 typography component).
  - `FluentProgress` → `FluentProgressBar` (non-interactive prerender fallback).

**Build status after Phase 2**: `dotnet build` on the entire solution (all `src`/`tests` projects, including `KITT.AppHost`) completes with **0 errors** — only 43 warnings remain, all either `CS0618` (obsolete-but-still-functional enum values like `Appearance.Accent`/`Color.Fill`/`FluentProgress`, intentionally left as a low-priority cosmetic cleanup item) or `CS0414` (a couple of now-unused private fields left over from the dialog rearchitecture in Phase 3).

## 🎉 Migration status: complete (compiles cleanly)

The Fluent UI Blazor v4 → v5 migration is functionally complete. The entire solution builds with zero errors.

## Remaining follow-up work (optional, not blocking)

- **Phase 7** — New bUnit test suites for the 5 Web App projects (`KITT.Web.App.UI`, `KITT.Web.App.Client`, `KITT.Web.App`, `KITT.Cms.Web.App`, `KITT.Proposals.Web.App`).
- Cosmetic cleanup of remaining `CS0618` obsolete-enum warnings (`Appearance.Accent`/`Outline`/`Lightweight`, `Color.Accent`/`Color.Fill`, `FluentProgress`) — functional as-is, but could be swept to the new enum values for consistency.
- Remove the 2 now-unused private fields flagged by `CS0414` (`MessageEditorDialog.sending`; check for others).
- Manual/visual QA pass: run the app and verify dialogs, forms, navigation, and the profile menu render and behave correctly, since several components were rebuilt from scratch (`LoginDisplay`, dialog footers) rather than mechanically translated.
