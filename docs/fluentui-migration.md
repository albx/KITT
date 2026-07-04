# Plan: Migrate Fluent UI Blazor v4 to v5

## Decisions

- Toasts are not removed in the target v5 package. Keep existing `FluentToastProvider` and `IToastService` usage unless the actual package build exposes a smaller API adjustment.
- Use v5 migration helper extensions such as `ToButtonAppearance()`, `ToTextInputAppearance()`, and `ToPositioning()` where available to keep the diff smaller and reduce manual enum rewrites.
- Remove `FluentUI.Blazor.Community.Components` v1.1.0 package reference and its `@using`; no actual usage was found in the codebase.
- Fully migrate all `FluentValidationMessage` usages to `FluentField` now. Do not defer partial form migration.
- Add bUnit test suites for every Web App project after the component migration lands.

## Target Version

Use `Microsoft.FluentUI.AspNetCore.Components` and `Microsoft.FluentUI.AspNetCore.Components.Icons` version `5.0.0-rc.4-26180.1`.

The Fluent UI documentation tool available in this workspace is pinned to `5.0.0.26177` and reports this target version as incompatible. Treat generated migration guidance as directionally useful, but verify exact API shape against the installed `5.0.0-rc.4-26180.1` package during implementation. The toast correction below is the known example where the docs-tool guidance was stale for this target.

## Toast Correction

Earlier planning claimed `FluentToast`, `FluentToastProvider`, and `IToastService` were removed and should be replaced with `IMessageService`/`FluentMessageBar`. That is incorrect for `5.0.0-rc.4-26180.1`.

Only secondary toast content components/types are removed: `CommunicationToast`, `ConfirmationToast`, `ProgressToast`, and their `*Content` types. Those were not found in this repo, so no toast migration is required beyond a build-time sanity check.

## Architecture Note

Only one host app/layout surface needs provider and navigation migration:

- `src/KITT.Web.App/KITT.Web.App/Components/App.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor`

`KITT.Cms.Web.App` and `KITT.Proposals.Web.App` are Razor Class Libraries hosted by `KITT.Web.App.Client`, so they contribute pages/components but share the single app layout and provider setup.

## Phase 0: Package and Project Setup

1. Bump `Microsoft.FluentUI.AspNetCore.Components` and `.Icons` to `5.0.0-rc.4-26180.1` in:
   - `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`
   - `src/KITT.Web.App/KITT.Web.App.Client/KITT.Web.App.Client.csproj`
   - `src/KITT.Web.App/KITT.Web.App/KITT.Web.App.csproj`
2. Remove `FluentUI.Blazor.Community.Components` from `src/KITT.Web.App.UI/KITT.Web.App.UI.csproj`.
3. Remove `@using FluentUI.Blazor.Community.Components` from `src/KITT.Cms.Web.App/_Imports.razor`.
4. Run a build and use the compiler errors as the authoritative implementation worklist.

## Phase 1: Core Bootstrap

Depends on Phase 0.

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

## Phase 2: Layout and Navigation

Depends on Phase 1.

1. Update `src/KITT.Web.App/KITT.Web.App.Client/Layout/MainLayout.razor`:
   - Keep `<FluentToastProvider Position="ToastPosition.TopCenter" />`; Fluent toast is not removed in the target v5 package.
   - Verify `ToastPosition.TopCenter` still exists after the package update.
   - Verify whether `<FluentDialogProvider />` and `<FluentTooltipProvider />` still exist or are needed in v5 before removing them.
   - Replace `FluentLabel` usages with `Typo` or `Color` parameters with `FluentText`, because `FluentLabel` is now input-label focused and no longer supports those typography parameters.

2. Rewrite `src/KITT.Web.App/KITT.Web.App.Client/Layout/NavMenu.razor`:
   - `FluentNavMenu` to `FluentNav`
   - `FluentNavGroup` to `FluentNavCategory`
   - `FluentNavLink` to `FluentNavItem`
   - `Width="250"` to `Width="250px"`
   - Drop removed parameters such as `Title`, `Collapsible`, `CollapsedChildNavigation`, `CustomToggle`, and `IconColor`.
   - Use `IconRest`/`IconActive` instead of `Icon` where appropriate.

3. Rebuild `src/KITT.Web.App/KITT.Web.App.Client/Components/LoginDisplay.razor`, because `FluentProfileMenu` was removed. Use `FluentPopover`, `FluentAvatar`, and `FluentButton` to preserve the profile-menu behavior.

## Phase 3: Dialog System Rearchitecture

Depends on Phase 1 and can run in parallel with Phase 2.

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

1. Replace `IDialogContentComponent<T>` implementations with the new v5 `FluentDialogInstance` pattern. Pull exact v5 API details before implementation, because the migration overview confirms the type change but not every member required by the new base class.
2. Remove `FluentDialogBody` and `FluentDialogFooter`; replace them with plain markup or layout components while keeping the existing action buttons.
3. Replace `DialogParameters<T>` with `DialogOptions`:
   - `Title` to `Header = new DialogOptionsHeader { Title = ... }`
   - right-side panel alignment to `Alignment = DialogAlignment.End`
   - keep `Width` where applicable
4. Re-check `dialog.Result`, `result.Cancelled`, and `DialogResult.Ok(...)`/`DialogResult.Cancel()` usage against the v5 `IDialogInstance`/result API while implementing.

## Phase 4: Forms and Inputs

Depends on Phase 1 and can run in parallel with Phases 2 and 3.

Main affected files include:

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor`
- `src/KITT.Cms.Web.App/Components/ScheduleForm.razor`
- `src/KITT.Cms.Web.App/Components/StreamingForm.razor`
- `src/KITT.Cms.Web.App/Components/ContentForm.razor`
- `src/KITT.Cms.Web.App/Pages/Streamings/StreamingDetail.razor`
- `src/KITT.Proposals.Web.App/Pages/Index.razor`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor`

Changes:

1. Replace `FluentTextField` with `FluentTextInput`:
   - `TextFieldType` to `TextInputType`
   - `FluentInputAppearance.Filled` to the v5 text-input appearance equivalent, preferably via helper extension where available
2. Update `FluentTextArea`:
   - Convert `FluentInputAppearance` to the v5 `TextAreaAppearance` equivalent
   - Replace `Rows` with `Height`, because `Rows` was removed
3. Update `FluentSelect`:
   - Add the second generic parameter `TValue`
   - Replace `@bind-SelectedOption` with `@bind-Value`
   - Replace old `Appearance` enum usage with `ListAppearance`
4. Replace every `FluentValidationMessage` with `FluentField` wrapping the related input:

   ```razor
   <FluentField Label="..." Required="true" ValidationFor="@(() => model.Property)">
       <FluentTextInput @bind-Value="model.Property" />
   </FluentField>
   ```

5. Rename all `FluentGridItem` breakpoint parameters to PascalCase:
   - `xs` to `Xs`
   - `sm` to `Sm`
   - `md` to `Md`
   - `lg` to `Lg`
   - `xl` to `Xl`
   - `xxl` to `Xxl`

6. Add explicit `Spacing="3"` to `FluentGrid` usages that relied on the v4 default spacing, because the v5 default is `0`.

## Phase 5: Toast Sanity Check

Depends on Phase 1.

No toast-to-message-service migration is required. Keep the existing `IToastService.ShowSuccess(...)` usage in:

- `src/KITT.Cms.Web.App/Components/ChannelFormPanel.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Settings/Channels.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Import.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Index.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/Schedule.razor.cs`
- `src/KITT.Cms.Web.App/Pages/Streamings/StreamingDetail.razor.cs`
- `src/KITT.Proposals.Web.App/Pages/Index.razor.cs`
- `src/KITT.Web.App/KITT.Web.App.Client/Components/MessageEditorDialog.razor.cs`

Actions:

1. Build after the package bump and verify `IToastService`, `FluentToastProvider`, and `ToastPosition.TopCenter` still compile.
2. Do not replace success toasts with `IMessageService`.
3. While touching `src/KITT.Proposals.Web.App/Pages/Index.razor.cs` for other migration work, optionally remove old commented-out `ToastService` code as cleanup only; it is not required for the v5 migration.

## Phase 6: Remaining Component Sweep

Depends on Phase 1 and should run after Phases 2 to 5.

1. `FluentButton`: convert old `Appearance` enum usage to `ButtonAppearance`, using helper extensions where possible.
2. `FluentAnchor`: replace with `FluentLink` for normal links or `FluentAnchorButton` for button-styled links.
3. `FluentCard`: remove `AreaRestricted` and `MinimalStyle`; use `CardAppearance` where needed.
4. `FluentIcon`: audit color usage. Default color now inherits from `currentColor`; replace old `Color.Accent`, `Color.Fill`, and related values with v5 equivalents.
5. `FluentDataGrid`: update renamed parameters and enums if used, including `ColumnOptionsLabels`, `ColumnResizeLabels`, `ColumnSortLabels`, `SortDirection`, and `Align`.
6. `FluentProgressRing`: rename to `FluentSpinner`; update `Stroke` to `Size`.
7. `FluentTooltip`: update `Position`/`TooltipPosition` to `Positioning`, using helper extensions where available.
8. Run a final color enum sweep for remaining `Color.Fill`, `Color.Accent`, and `Color.Neutral` literals.

## Phase 7: bUnit Test Suites for Web App Projects

Depends on Phases 2 through 6 being functionally complete.

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

1. Run the build task after each phase and fix compiler errors iteratively.
2. Run the existing test suites:
   - `tests/KITT.Core.Test`
   - `tests/KITT.Cms.Web.Api.Test`
3. Run the new bUnit suites created in Phase 7.
4. Manually smoke test:
   - navigation links and category expansion
   - add/edit channel panel
   - validation display
   - success toasts and error message bars
   - streaming import/schedule/detail forms
   - proposals filters and proposal detail dialog
   - message composer dialog
   - login/profile menu
5. Visually check `FluentGrid` and `FluentStack` spacing on the main CMS and Proposals pages, since v5 changes default spacing behavior.

## Further Considerations

1. Pull exact v5 documentation/API samples for `FluentDialogInstance`, `IDialogInstance`, and dialog results before Phase 3 implementation.
2. Verify whether `FluentDialogProvider` and `FluentTooltipProvider` are still needed in v5 during Phase 2.
3. Because the available docs tool is not on the exact target package version, treat every removed/renamed component claim as a hypothesis to confirm against the actual `5.0.0-rc.4-26180.1` package and build output.
