using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.CloudSave.Settings
{
    class CloudSaveSettingsProvider : EditorGameServiceSettingsProvider
    {
        const string k_Title = "Cloud Save";
        const string k_GoToDashboardContainer = "dashboard-button-container";
        const string k_GoToDashboardBtn = "dashboard-link-button";

        static readonly CloudSaveEditorGameService k_GameService = new CloudSaveEditorGameService();

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new CloudSaveSettingsProvider(SettingsScope.Project);
        }

        protected override IEditorGameService EditorGameService => k_GameService;
        protected override string Title => k_Title;
        protected override string Description => "Cloud Save is easy to use, flexible, and works for any kind of player data. Once integrated, players can log in and have an amazing experience - anywhere, across multiple devices.";

        public CloudSaveSettingsProvider(SettingsScope scopes)
            : base(GenerateProjectSettingsPath(k_Title), scopes) {}

        protected override VisualElement GenerateServiceDetailUI()
        {
            var containerVisualElement = new VisualElement();

            // No settings for Cloud Save at the moment

            return containerVisualElement;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            SetDashboardButton(rootElement);
        }

        static void SetDashboardButton(VisualElement rootElement)
        {
            rootElement.Q(k_GoToDashboardContainer).style.display = DisplayStyle.Flex;
            var goToDashboard = rootElement.Q(k_GoToDashboardBtn);

            if (goToDashboard != null)
            {
                var clickable = new Clickable(() =>
                {
                    Application.OpenURL(k_GameService.GetFormattedDashboardUrl());
                });
                goToDashboard.AddManipulator(clickable);
            }
        }
    }
}
