using PluginConfig.API;
using PluginConfig.API.Fields;
using UnityEngine;

namespace UltraYoshiSkulls
{
    public class YoshiConfig
    {
        private PluginConfigurator config;

        // Our toggle fields
        private BoolField redSkull;
        private BoolField blueSkull;
        private BoolField soap;
        private BoolField torch;
        private BoolField book;
        private BoolField rocket;

        public void Awake()
        {
            config = PluginConfigurator.Create("Ultra Yoshis", "io.selim.ultrayoshis");

            redSkull = new BoolField(config.rootPanel, "Replace Red Skulls with PinkYoshi", "yoshi.redskull", true);
            blueSkull = new BoolField(config.rootPanel, "Replace Blue Skulls with BlueYoshi", "yoshi.blueskull", true);
            soap = new BoolField(config.rootPanel, "Replace Soap with Yoshi", "yoshi.soap", true);
            torch = new BoolField(config.rootPanel, "Replace Torches with GoldenYoshi", "yoshi.torch", true);
            book = new BoolField(config.rootPanel, "Replace Books with Yoshi", "yoshi.book", true);
            rocket = new BoolField(config.rootPanel, "Replace Rockets with GreyYoshi", "yoshi.rocket", true);
        }

        // Easy access properties for our patches to check
        public bool IsRedSkullDisabled => !redSkull.value;
        public bool IsBlueSkullDisabled => !blueSkull.value;
        public bool IsSoapDisabled => !soap.value;
        public bool IsTorchDisabled => !torch.value;
        public bool IsBookDisabled => !book.value;
        public bool IsRocketDisabled => !rocket.value;
    }
}