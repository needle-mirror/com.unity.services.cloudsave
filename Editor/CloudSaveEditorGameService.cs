using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.OrganizationHandler;
using UnityEditor;

namespace Unity.Services.CloudSave.Settings
{
    struct CloudSaveIdentifier : IEditorGameServiceIdentifier
    {
        public string GetKey() => "Cloud Save";
    }
    class CloudSaveEditorGameService : IEditorGameService
    {
        public string Name => "Cloud Save";
        public IEditorGameServiceIdentifier Identifier => k_Identifier;
        public bool RequiresCoppaCompliance => false;
        public bool HasDashboard => true;
        public IEditorGameServiceEnabler Enabler => null;

        static readonly CloudSaveIdentifier k_Identifier = new CloudSaveIdentifier();

        public string GetFormattedDashboardUrl()
        {
            return
                $"https://dashboard.unity3d.com/organizations/{OrganizationProvider.Organization.Key}/projects/{CloudProjectSettings.projectId}/cloud-save/about";
        }
    }
}
