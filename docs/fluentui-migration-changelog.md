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

## Remaining phases (not started)

- **Phase 2** — Layout and navigation (`MainLayout.razor`, `NavMenu.razor`, `LoginDisplay.razor`).
- **Phase 5** — Toast sanity pass / `INotificationService` migration for the remaining `IToastService`/`IMessageService` call sites: `Import.razor.cs`, `Schedule.razor.cs`, `StreamingDetail.razor.cs`, `Streamings/Index.razor.cs`, `Proposals/Index.razor.cs`.
- **Phase 6** — Remaining component sweep (`FluentAnchor`, `FluentButton` appearance enums, `FluentCard`, icons, `FluentDataGrid` renames).
- **Phase 7** — New bUnit test suites for the 5 Web App projects.
