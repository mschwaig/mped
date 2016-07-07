using HeuristicLab.PluginInfrastructure;

namespace at.mschwaig.mped.hl.plugin
{
    [Plugin("mped_cs", "Implementation of the Multi-parameterized Edit Distance", "3.3.13.0")]
    [PluginFile("mped_cs.dll", PluginFileType.Assembly)]
    public class Plugin : PluginBase
    {
    }
}